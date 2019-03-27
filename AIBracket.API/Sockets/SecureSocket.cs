using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace AIBracket.API.Sockets
{
    public class SecureSocket : ISocket
    {
        private const int buff_size = 1024;

        private SslStream _socket;

        public bool IsReady => _payloads.Count > 0;

        public bool IsConnected => _socket.CanRead && _socket.CanWrite;

        private bool _isWebsocket, _first;

        private byte[] _readbuffer, _writebuffer;

        private List<string> _payloads = new List<string>();

        private string _name, _ppayload;

        public string Name {
            get
            {
                return _name ?? (_name = "Spectator" + new Random().Next(1, 12800));
            }
            set
            {
                if (value != null)
                {
                    _name = value;
                }
            }
        }

        public SecureSocket(SslStream client)
        {
            _socket = client;
            _first = true;
            _readbuffer = new byte[buff_size];
            _writebuffer = new byte[buff_size];
            _socket.BeginRead(_readbuffer, 0, buff_size, new AsyncCallback(ReadCallback), _socket);
        }

        public void Disconnect()
        {
            _socket.Close();
        }

        public string ReadData()
        {
            if (IsReady)
            {
                var message = _payloads[0];
                _payloads.RemoveAt(0);
                return message;
            }
            else
            {
                return "";
            }
        }

        public void WriteData(string data)
        {
            byte[] bytes;
            if (_isWebsocket)
            {
                bytes = GetEncodedData(data);
            }
            else
            {
                bytes = Encoding.ASCII.GetBytes(data);
            }
            try
            {
                _socket.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(WriteCallback), _socket);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error receiving data from client: " + e.Message);
            }
        }

        private byte[] GetEncodedData(string buffer)
        {
            var barray = new List<byte>();
            barray.Add(0x81);
            if(buffer.Length > 0xFFFF)
            {
                barray.Add(127);
                barray.AddRange(BitConverter.GetBytes((Int32)buffer.Length));
            }
            else if (buffer.Length > 0xFF)
            {
                barray.Add(126);
                barray.AddRange(BitConverter.GetBytes((Int16)buffer.Length));
            }else
            {
                barray.Add((byte)buffer.Length);
            }
            barray.AddRange(Encoding.ASCII.GetBytes(buffer));
            return barray.ToArray();
        }

        private byte[] GetDecodedData(Byte[] buffer)
        {
            String incomingData = String.Empty;
            Byte secondByte = buffer[1];
            Int32 dataLength = secondByte & 127;
            Int32 indexFirstMask = 2;
            if (dataLength == 126)
                indexFirstMask = 4;
            else if (dataLength == 127)
                indexFirstMask = 10;
            IEnumerable<Byte> keys = buffer.Skip(indexFirstMask).Take(4);
            Int32 indexFirstDataByte = indexFirstMask + 4;

            Byte[] decoded = new Byte[buffer.Length - indexFirstDataByte];
            for (Int32 i = indexFirstDataByte, j = 0; i < buffer.Length; i++, j++)
            {
                decoded[j] = (Byte)(buffer[i] ^ keys.ElementAt(j % 4));
            }

            return decoded;
        }

        private void WriteCallback(IAsyncResult ar)
        {
            SslStream stream = (SslStream)ar.AsyncState;
            try
            {
                stream.EndWrite(ar);
            }catch(Exception e)
            {
                Console.WriteLine("Error writing data to socket: " + e.Message);
                stream.Dispose();
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            SslStream stream = (SslStream)ar.AsyncState;
            int byteCount = -1;
            try
            {
                byteCount = stream.EndRead(ar);
                var newbuff = new byte[byteCount];
                if(byteCount == 0)
                {
                    stream.Dispose();
                    return;
                }
                Buffer.BlockCopy(_readbuffer, 0, newbuff, 0, byteCount);
                if (_isWebsocket)
                {
                    var message = GetDecodedData(newbuff);
                    if((newbuff[0] & 0x80) == 0)
                    {
                        Console.WriteLine("Received continuation frame");
                        _ppayload += Encoding.ASCII.GetString(newbuff);
                    }
                    else
                    {
                        Console.WriteLine("Received finish frame");
                        switch(newbuff[0] & 0x0F)
                        {
                            case 0x01: //text
                                _ppayload += Encoding.ASCII.GetString(newbuff);
                                _payloads.Add(_ppayload);
                                _ppayload = "";
                                break;
                            case 0x00: //continue
                                _ppayload += Encoding.ASCII.GetString(newbuff);
                                Console.WriteLine("Received continuation frame in finish frame");
                                break;
                            case 0x02:
                                Console.WriteLine("Received binary frame");
                                break;
                            case 0x08:
                                Console.WriteLine("Closing connection");
                                stream.Dispose();
                                return;
                            case 0x09:
                                Buffer.BlockCopy(newbuff, 1, _writebuffer, 1, byteCount - 1);
                                _writebuffer[0] = 0x8A;
                                stream.BeginWrite(_writebuffer, 0, byteCount, new AsyncCallback(WriteCallback), stream);
                                break;
                            case 0xA:
                                Console.WriteLine("Pong");
                                break;
                            default:
                                Console.WriteLine("Unknown opcode: " + (newbuff[0] & 0x0F));
                                stream.Dispose();
                                return;
                        }
                        
                    }
                }
                else
                {
                    var message = Encoding.ASCII.GetString(newbuff);
                    if (_first)
                    {
                        if(message.StartsWith("GET ")){
                            _isWebsocket = true;
                            const string eol = "\r\n"; // HTTP/1.1 defines the sequence CR LF as the end-of-line marker

                            byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + eol
                                + "Connection: Upgrade" + eol
                                + "Upgrade: websocket" + eol
                                + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                                    System.Security.Cryptography.SHA1.Create().ComputeHash(
                                        Encoding.UTF8.GetBytes(
                                            new System.Text.RegularExpressions.Regex("Sec-WebSocket-Key: (.*)").Match(message).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                                        )
                                    )
                                ) + eol
                                + eol);
                            stream.BeginWrite(response, 0, response.Length, new AsyncCallback(WriteCallback), stream);
                            _first = false;
                            stream.BeginRead(_readbuffer, 0, buff_size, new AsyncCallback(ReadCallback), stream);
                            return;
                        }
                    }
                    _payloads.Add(message);
                }

                _first = false;
                stream.BeginRead(_readbuffer, 0, buff_size, new AsyncCallback(ReadCallback), stream);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading data from sslsocket: " + e.Message);
                return;
            }
        }
    }
}
