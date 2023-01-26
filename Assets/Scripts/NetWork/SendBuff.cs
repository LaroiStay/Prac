using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{


    public class SendBuffHelper
    {
        public static ThreadLocal<SendBuff> CurrentBuff =
            new ThreadLocal<SendBuff>(() => { return null; });

        public static int ChunckSize { get; set; } = 65535 * 100;


        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (CurrentBuff.Value == null)
                CurrentBuff.Value = new SendBuff(ChunckSize);

            if (CurrentBuff.Value.FreeSize < reserveSize)
                CurrentBuff.Value = new SendBuff(ChunckSize);
            return CurrentBuff.Value.Open(reserveSize);
        }


        public static ArraySegment<byte> Close(int usesize)
        {
            return CurrentBuff.Value.Close(usesize);


        }

    }

    public class SendBuff
    {
        byte[] m_buffer;
        int m_used_size = 0;

        public int FreeSize { get { return m_buffer.Length - m_used_size; } }
    

        public SendBuff (int chunckSize)
        {
            m_buffer = new byte[chunckSize];
        }
        public ArraySegment<byte> Open(int reserveSize) {
            return new ArraySegment<byte>(m_buffer, m_used_size, reserveSize);
        }
        public ArraySegment<byte> Close(int useSIze) {
            ArraySegment<byte> segment = new ArraySegment<byte>(m_buffer,m_used_size,useSIze);
            m_used_size += useSIze;
            return segment;
        }

    }
}
