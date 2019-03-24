using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Linq;

namespace AIBracket.API.Sockets
{
    public class WebSocket : ISocket
    {

        private TcpClient _socket;

        public TcpClient Socket
        {
            get
            {
                return _socket;
            }
        }

        private string _name;

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

        public bool IsReady => _socket.Connected && _socket.Available > 0;

        public bool IsConnected => _socket.Connected;

        public WebSocket(TcpClient client)
        {
            _socket = client;
        }

        public WebSocket(TcpClient client, string name)
        {
            _socket = client;
            Name = name;
        }

        public void Disconnect()
        {
            _socket.Close();
        }

        public string ReadData()
        {
            if (IsReady)
            {
                var readAmount = _socket.Available;
                if(readAmount > 1024)
                {
                    Console.WriteLine("Data larger than 1KB: " + readAmount + " bytes");
                    Disconnect();
                    return "";
                }
                var data = new byte[readAmount];
                try
                {
                    _socket.GetStream().Read(data, 0, readAmount);
                    return Encoding.ASCII.GetString(GetDecodedData(data)).Trim();
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error receiving data");
                    Console.WriteLine(e.Message);
                    Disconnect();
                    return "";
                }
            }
            return "";
        }

        public void WriteData(string data)
        {
            if (IsConnected)
            {
                try
                {
                    var bytes = EncodeMessageToSend(data);
                    _socket.GetStream().Write(bytes);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error sending data");
                    Console.WriteLine(e.Message);
                    Disconnect();
                }
            }
        }

        private static byte[] GetDecodedData(Byte[] buffer)
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
    }
}
