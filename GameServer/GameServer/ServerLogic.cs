using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace GameServer
{
    class ServerLogic
    {   
        public static int Port { get; private set; }
        public static int MaxNumConnections { get; private set; }
        public static Dictionary<int, Client> connectedClients = new Dictionary<int, Client>();
        public static TcpListener tcpListener;
        public static UdpClient udpListener;

        public static void initializeClientDictionary()
        {
            for (int i = 1; i <= MaxNumConnections; i++)
            {
                connectedClients.Add(i, new Client(i));
            }
        }

        public void SendUDPPacket(Packet packet)
        {
            //To be continued
            return;
        }

        public static void Start(int maxConn, int port)
        {
            Console.WriteLine("Booting up");
            MaxNumConnections = maxConn;
            Port = port;
            initializeClientDictionary();
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(UponConnectionCallback), null);
            Console.WriteLine("Connection accepted");


            //UDP related
            udpListener = new UdpClient(port);
            udpListener.BeginReceive(UponReceiveUDPCallback, null);
        }


    
        public static void UponReceiveUDPCallback(IAsyncResult result)
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] dataReceived = udpListener.EndReceive(result, ref clientEndPoint);
            udpListener.BeginReceive(UponReceiveUDPCallback, null);

            //Perform checks on packet and handle data

        }

        public static void UponConnectionCallback(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(UponConnectionCallback), null);
            Console.WriteLine("Connection Accepted");

            for(int i = 1; i <= MaxNumConnections; i++)
            {
                if(connectedClients[i].tcp.ConnectionCallbackInstance == null)
                {
                    connectedClients[i].tcp.Connect(client);
                    return;
                }
            }

        }
    }
}
