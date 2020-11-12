using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Data;
using System.Runtime.InteropServices.ComTypes;
using System.ComponentModel;

namespace GameServer
{
    class Client
    {
        public static int BufferSize = 1024;
        public int id;
        public TCP tcpInstance;
        public UDP udpInstance;

        public Client(int clientId)
        {
            id = clientId;
            tcpInstance = new TCP(id);
            udpInstance = new UDP(id); 
        }
       
        
        public class UDP
        {
            public int id;
            public IPEndPoint endPoint;

            public UDP(int instanceID)
            {
                id = instanceID;
            }

            public void Connect(IPEndPoint end)
            {
                endPoint = end;
            }

            public void SendPacket(Packet packet)
            {
                ServerLogic.SendUDPPacket(endPoint, packet);
            }

            public void HandleData(Packet packet)
            {
                int length = packet.ReadInt();
                byte[] receivedData = packet.ReadBytes(length);
            }
        }

        public class TCP
        {
            public TcpClient ConnectionCallbackInstance;
            private int id;
            private NetworkStream stream;
            private byte[] receiveBuffer;

            public TCP(int ClientId)
            {
                id = ClientId;
            }
            
            
            public void SendPacket(Packet packet)
            {
                if(ConnectionCallbackInstance != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    Console.WriteLine("Data sent");
                }
            }

            public void ReceiveCallback(IAsyncResult result)
            {
                //Will crash server on error. Maybe try catch block?
                //End stream to receive data
                int numBytesReceived = stream.EndRead(result);
                if(numBytesReceived <= 0)
                {
                    //No data received
                    return;
                } else
                {
                    byte[] dataReceived = new byte[numBytesReceived];
                    Array.Copy(receiveBuffer, dataReceived, numBytesReceived);

                    //Handle data

                    stream.BeginRead(receiveBuffer, 0, BufferSize, ReceiveCallback, null);
                }
            }

            public void Connect(TcpClient CallbackInstance)
            {
                //Tcp Client instance from BeginAcceptTcpCLient call
                ConnectionCallbackInstance = CallbackInstance;
                ConnectionCallbackInstance.ReceiveBufferSize = BufferSize;
                ConnectionCallbackInstance.SendBufferSize = BufferSize;
                stream = ConnectionCallbackInstance.GetStream();
                receiveBuffer = new byte[BufferSize];
                //Start stream to begin fetching data
                stream.BeginRead(receiveBuffer, 0, BufferSize, ReceiveCallback, null);
                ServerSend.TestPacketSend(id, "Heelo u r connekt now");

                //TODO send confirmation
            }
        }
    }
}
