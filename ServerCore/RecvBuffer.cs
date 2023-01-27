using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> m_buffer;
        int m_readPos;
        int m_writePos;

        public RecvBuffer(int buffersize)
        {
            m_buffer = new ArraySegment<byte>(new byte[buffersize],0, buffersize);
        }

        public int Datasize { get { return m_writePos - m_readPos; } }
        public int Freesize { get { return m_buffer.Count - m_writePos; } }

        public ArraySegment<byte> ReadSegment { get { return 
                    new ArraySegment<byte>(m_buffer.Array,m_buffer.Offset+ m_readPos, Datasize); } }
        public ArraySegment<byte> WriteSegment { get { return
                    new ArraySegment<byte>(m_buffer.Array, m_buffer.Offset+m_writePos,Freesize); } }


        public void Clean()
        {
            if(Datasize == 0)
                m_readPos = m_writePos = 0;
            else
            {
                Array.Copy(m_buffer.Array, m_buffer.Offset + m_readPos, m_buffer.Array, m_buffer.Offset, Datasize);
                m_readPos = 0;
                m_writePos = Datasize;
            }

        }



        public bool OnRecv(int numOfBytes)
        {
            if (numOfBytes > Datasize)
                return false;
            m_readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > Freesize)
                return false;
            m_writePos += numOfBytes;
            return true;
        }






    }
}
