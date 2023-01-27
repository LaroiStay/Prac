using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{

    
    class Program
    {

        static Listener m_Listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void FlushRoom() { Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static void Main(string[] args)
        {

            string host = Dns.GetHostName();
            IPHostEntry IPhost = Dns.GetHostEntry(host);
            IPAddress IPAdr = IPhost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(IPAdr, 7777);
            m_Listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("listening...");

            FlushRoom();
            while (true)
            {
                JobTimer.Instance.Flush();

            }
        }





    }
}