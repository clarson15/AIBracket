using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Linq;

namespace AIBracket.API.Sockets
{
    public class InsecureSocket : ISocket
    {
        private const int buff_size = 1024;

        private TcpClient _socket;

        public bool IsReady => _payloads.Count > 0;

        public bool IsConnected => _socket != null && _socket.Connected && _socket.Client.Connected;

        private bool _isWebsocket, _first;

        private byte[] _readbuffer, _writebuffer;

        private List<string> _payloads = new List<string>();

        private string _name, _ppayload, _target;

        private int readCount;

        private DateTime lastCheck;

        public string Name
        {
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

        public string Target
        {
            get => _target;
            set => _target = value;
        }

        public InsecureSocket(TcpClient client)
        {
            _socket = client;
            _first = true;
            _readbuffer = new byte[buff_size];
            _writebuffer = new byte[buff_size];
            lastCheck = DateTime.Now;
            readCount = 0;
            try
            {
                _socket.Client.BeginReceive(_readbuffer, 0, buff_size, 0, new AsyncCallback(ReadCallback), _socket);
            }
            catch
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": Client disconnected");
                Disconnect();
            }
        }

        public void Disconnect()
        {
            _socket.Close();
        }

        public string ReadData()
        {
            if (IsReady)
            {
                var message = _payloads[0].Trim();
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
                bytes = EncodeMessageToSend(data);
            }
            else
            {
                bytes = Encoding.ASCII.GetBytes(data);
            }
            try
            {
                _socket.Client.BeginSend(bytes, 0, bytes.Length, 0, new AsyncCallback(WriteCallback), _socket);
            }
            catch
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": Client disconnected");
                Disconnect();
            }
        }

        private static byte[] EncodeMessageToSend(string message)
        {
            Byte[] response;
            Byte[] bytesRaw = Encoding.UTF8.GetBytes(message);
            Byte[] frame = new Byte[10];

            Int32 indexStartRawData = -1;
            Int32 length = bytesRaw.Length;

            frame[0] = (Byte)129;
            if (length <= 125)
            {
                frame[1] = (Byte)length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = (Byte)126;
                frame[2] = (Byte)((length >> 8) & 255);
                frame[3] = (Byte)(length & 255);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = (Byte)127;
                frame[2] = (Byte)((length >> 56) & 255);
                frame[3] = (Byte)((length >> 48) & 255);
                frame[4] = (Byte)((length >> 40) & 255);
                frame[5] = (Byte)((length >> 32) & 255);
                frame[6] = (Byte)((length >> 24) & 255);
                frame[7] = (Byte)((length >> 16) & 255);
                frame[8] = (Byte)((length >> 8) & 255);
                frame[9] = (Byte)(length & 255);

                indexStartRawData = 10;
            }

            response = new Byte[indexStartRawData + length];

            Int32 i, reponseIdx = 0;

            //Add the frame bytes to the reponse
            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }

            //Add the data bytes to the response
            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = bytesRaw[i];
                reponseIdx++;
            }

            return response;
        }

        private byte[] GetDecodedData(byte[] buffer)
        {
            string incomingData = string.Empty;
            byte secondByte = buffer[1];
            int dataLength = secondByte & 127;
            int indexFirstMask = 2;
            if (dataLength == 126)
                indexFirstMask = 4;
            else if (dataLength == 127)
                indexFirstMask = 10;
            IEnumerable<byte> keys = buffer.Skip(indexFirstMask).Take(4);
            int indexFirstDataByte = indexFirstMask + 4;

            byte[] decoded = new byte[buffer.Length - indexFirstDataByte];
            for (int i = indexFirstDataByte, j = 0; i < buffer.Length; i++, j++)
            {
                decoded[j] = (byte)(buffer[i] ^ keys.ElementAt(j % 4));
            }

            return decoded;
        }

        private void WriteCallback(IAsyncResult ar)
        {
            TcpClient stream = (TcpClient)ar.AsyncState;
            try
            {
                stream.Client.EndSend(ar);
            }
            catch
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": Client disconnected");
                Disconnect();
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            TcpClient stream = (TcpClient)ar.AsyncState;
            if(DateTime.Now.Subtract(lastCheck).Seconds > 1)
            {
                readCount = 0;
                lastCheck = DateTime.Now;
            }
            readCount++;
            if (readCount > 10)
            {
                WriteData("Too many packets.");
                Disconnect();
                return;
            }
            int byteCount = -1;
            try
            {
                byteCount = stream.Client.EndReceive(ar);
                var newbuff = new byte[byteCount];
                if (byteCount == 0)
                {
                    stream.Dispose();
                    return;
                }
                Buffer.BlockCopy(_readbuffer, 0, newbuff, 0, byteCount);
                if (_isWebsocket)
                {
                    var message = GetDecodedData(newbuff);
                    if ((newbuff[0] & 0x80) == 0)
                    {
                        _ppayload += Encoding.ASCII.GetString(message);
                    }
                    else
                    {
                        switch (newbuff[0] & 0x0F)
                        {
                            case 0x01: // text
                                _ppayload += Encoding.ASCII.GetString(message);
                                _payloads.Add(_ppayload);
                                _ppayload = "";
                                break;
                            case 0x00: // continue
                                _ppayload += Encoding.ASCII.GetString(message);
                                break;
                            case 0x02: // binary
                                break;
                            case 0x08: // close
                                stream.Dispose();
                                return;
                            case 0x09: // ping
                                Buffer.BlockCopy(newbuff, 1, _writebuffer, 1, byteCount - 1);
                                _writebuffer[0] = 0x8A;
                                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": Ping");
                                stream.Client.BeginSend(_writebuffer, 0, byteCount, 0, new AsyncCallback(WriteCallback), stream);
                                break;
                            case 0xA: // pong
                                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": Pong");
                                break;
                            default:
                                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": Unknown opcode: " + (newbuff[0] & 0x0F));
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
                        if (message.StartsWith("GET "))
                        {
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
                            stream.Client.BeginSend(response, 0, response.Length, 0, new AsyncCallback(WriteCallback), stream);
                            _first = false;
                            stream.Client.BeginReceive(_readbuffer, 0, buff_size, 0, new AsyncCallback(ReadCallback), stream);
                            return;
                        }
                    }
                    _payloads.Add(message);
                }

                _first = false;
                stream.Client.BeginReceive(_readbuffer, 0, buff_size, SocketFlags.None, new AsyncCallback(ReadCallback), stream);
            }
            catch
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": Client disconnected");
                Disconnect();
            }
        }
    }
}
