using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace DummyClient
{


	class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint endpoint)
        {
           // Console.WriteLine("good");
        }

        public override void OnDisconnected(EndPoint endpoint)
        {
            Console.WriteLine($"OnDisconnected____ {endpoint}");
        }



        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer, (s, p) => { PacketQueue.Instance.Push(p); });
        }


        public override void OnSend(int numOfBytes)
        {

            //Console.WriteLine($"Transferred bytes :{numOfBytes}");

        }
    }
}
