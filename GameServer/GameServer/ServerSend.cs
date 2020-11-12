using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    class ServerSend
    {
        public static void TestPacketSend(int ClientID, string message)
        {
            using (Packet packet = new Packet())
            {
                //Send some sort of package
                return;
            }
        }

        private static void SendTCPData(int ClientID, Packet packet)
        {
            packet.WriteLength();
            ServerLogic.connectedClients[ClientID].tcpInstance.SendPacket(packet);
        }

    }
}
