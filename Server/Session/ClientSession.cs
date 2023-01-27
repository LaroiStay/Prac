using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{




	class ClientSession : PacketSession
    {
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public GameRoom Room { get; set; }
        public int Sessionid { get; set; }
        public override void OnConnected(EndPoint endpoint)
        {
            //Console.WriteLine($"OnConnected{endpoint}");
            Program.Room.Push(() =>Program.Room.Enter(this));
        }

        public override void OnDisconnected(EndPoint endpoint)
        {
            SessionManager.Instance.Remove(this);
            if(Room != null)
            {
                GameRoom Ro = Room;
                Ro.Push(() => Ro.Leave(this));
                Room = null;
            } 
            Console.WriteLine($"OnDisconnected {endpoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {

            PacketManager.Instance.OnRecvPacket(this, buffer);

        }
        public enum PacketID
        {
            PlayerInfoReq = 1,
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes :{numOfBytes}");

        }
    }
}
