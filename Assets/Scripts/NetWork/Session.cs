using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;
            int packetCount = 0;
            while (true)
            {
                if (buffer.Count < HeaderSize)
                    break;
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset,dataSize));
                packetCount++;
                 processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize,buffer.Count - dataSize);
            }
            if(packetCount >0)
                Console.WriteLine($"패킷 모아보내기 : {packetCount}");
            return processLen;
        }


        public abstract void OnRecvPacket(ArraySegment<byte> buffer); 
    }


    public abstract class Session
    {
        RecvBuffer m_recvBuffer = new RecvBuffer(65535);
        Socket m_socket;
        int disconnect = 0;
        object m_lock = new object();
        Queue<ArraySegment<byte>> m_sendQueue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> m_pendinglist = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs m_sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();


        public abstract void OnConnected(EndPoint endpoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endpoint);


        void Clear()
        {
            lock (m_lock)
            {
                m_sendQueue.Clear();
                m_pendinglist.Clear();
            }
        }

        public void Start(Socket socket)
        {
            m_socket = socket;
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            m_sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();

        }
        public void Send(ArraySegment<byte> sendbuff)
        {
            lock (m_lock)
            {
                m_sendQueue.Enqueue(sendbuff);
                if (m_pendinglist.Count == 0)
                    RegisterSend();
            }
        }

        public void Send(List<ArraySegment<byte>> sendbuffList)
        {

            if (sendbuffList.Count == 0)
                return;
            lock (m_lock)
            {
                foreach(ArraySegment<byte> sendbuff in sendbuffList)
                    m_sendQueue.Enqueue(sendbuff);
                if (m_pendinglist.Count == 0)
                    RegisterSend();
            }
        }

        void RegisterSend()
        {

            if (disconnect == 1)
                return;

            while (m_sendQueue.Count >0)
            {
                ArraySegment<byte> buff = m_sendQueue.Dequeue();
                m_pendinglist.Add(buff);
            }
            m_sendArgs.BufferList = m_pendinglist;
            try
            {
                bool pending = m_socket.SendAsync(m_sendArgs);
                if (pending == false)
                    OnSendCompleted(null, m_sendArgs);
            }
            catch(Exception e)
            {
                Console.WriteLine("RegisterSend failed");
            }
          
        }

        void OnSendCompleted(object sender , SocketAsyncEventArgs args)
        {
            lock (m_lock)
            {
                try
                {

                    m_sendArgs.BufferList = null;
                    m_pendinglist.Clear();
                    OnSend(args.BytesTransferred);
                    if (m_sendQueue.Count > 0)
                        RegisterSend();
                  
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
          
        }


        void RegisterRecv()
        {
            if (disconnect == 1)
                return;
            m_recvBuffer.Clean();
            ArraySegment<byte> segment = m_recvBuffer.WriteSegment;
            recvArgs.SetBuffer(segment);
            recvArgs.AcceptSocket = null;

            try
            {
                bool pending = m_socket.ReceiveAsync(recvArgs);
                if (pending == false)
                    OnRecvCompleted(null, recvArgs);
            }
            catch(Exception e)
            {
                Console.WriteLine("Register Recv Failed");
            }
        

        }
        
       

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref disconnect, 1) == 1)
                return;
            OnDisconnected(m_socket.RemoteEndPoint);
            m_socket.Shutdown(SocketShutdown.Both);
            m_socket.Close();
            Clear();
        }

        #region 네트워크통신
        void OnRecvCompleted(object obj, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred>0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    if(m_recvBuffer.OnWrite(args.BytesTransferred)==false )
                    {
                        Disconnect();
                        return;
                    }
                    int processInt =  OnRecv(m_recvBuffer.ReadSegment); 
                    if(processInt < 0|| m_recvBuffer.Datasize <processInt)
                    {
                        Disconnect();
                        return;
                    }
                    if(m_recvBuffer.OnRecv(processInt)== false)
                    {
                        Disconnect();
                        return;
                    }
                  
                    RegisterRecv();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
           
            }
            else
            {
                Disconnect();
            }
        }
        #endregion

    }
}
