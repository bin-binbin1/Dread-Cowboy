using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System;

namespace SocketMultplayerGameServer
{
    class Server
    {
        private static Socket socket;
        private static byte[] buffer=new byte[1024];
        static void Main(string[] atgs)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 5555));//°ó¶¨
            socket.Listen(0);
            StartAccept();
            Console.Read();
        }

       static void StartAccept()
        {
            socket.BeginAccept(AcceptCallback, null);
        }

        static void StartReceive(Socket client)
        {
            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, client);
        }
        static void ReceiveCallback(IAsyncResult iar)
        {
            Socket client = (Socket)iar.AsyncState;
            int len = client.EndReceive(iar);
            if (len == 0)
                return;
            string str=Encoding.UTF8.GetString(buffer,0,len);
            Console.WriteLine(str);
            StartReceive(client);
        }

        static void AcceptCallback(IAsyncResult iar) 
        {
           Socket client = socket.EndAccept(iar);
            StartReceive(client);
            StartAccept();
        }

    }
}


