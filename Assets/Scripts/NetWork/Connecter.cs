using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Connecter
    {
        Func<Session> m_sessionFac;

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFac, int count =  1)
        {

            for(int i=0; i< count; i++) {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                m_sessionFac = sessionFac;
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);
                args.RemoteEndPoint = endPoint;
                args.UserToken = socket;
                RegisterConnect(args);
            }
           

        }


        void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;
            if (socket == null)
                return;
            bool pending = socket.ConnectAsync(args);
            if (pending == false)
                OnConnectCompleted(null, args);
        }




        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {

            if(args.SocketError == SocketError.Success)
            {
                Session session = m_sessionFac.Invoke();
                session.Start(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine("Socket Error");
            }
        }




    }
}
