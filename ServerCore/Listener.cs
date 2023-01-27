using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class Listener
    {

        Socket m_listenerSocket;
        Func<Session> m_sessionFactoty;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactoty, int register = 10,int backlog = 100)
        {
            m_sessionFactoty = sessionFactoty;
            m_listenerSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_listenerSocket.Bind(endPoint);
            m_listenerSocket.Listen(backlog);

            for(int  i=0;i <10; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args);
            }
        }


        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;
            bool pending = m_listenerSocket.AcceptAsync(args);
            if (pending == false)
                OnAcceptCompleted(null, args);
        }


        void OnAcceptCompleted(object obj, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success) {
                Session m_session = m_sessionFactoty.Invoke();
                m_session.Start(args.AcceptSocket);
                m_session.OnConnected(args.AcceptSocket.RemoteEndPoint);
                
            }
            else
                Console.WriteLine(args.ToString());
            RegisterAccept(args);
        }
        public Socket Accept()
        {
            return m_listenerSocket.Accept();
        }
        

    }
}
