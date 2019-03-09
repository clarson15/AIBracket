using System;
using System.Collections.Generic;
using System.Text;

namespace AIBracket.API.Sockets
{
    public interface ISocket
    {
        string ReadData();
        void WriteData(string data);
        void Disconnect();
        bool IsReady { get; }
        bool IsConnected { get; }
    }
}
