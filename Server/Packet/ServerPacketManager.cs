
using ServerCore;
class PacketManager
{
    #region SingleTon
    static PacketManager m_instance = new PacketManager();
    public static PacketManager Instance { get { return m_instance; } }
    #endregion

    PacketManager()
    {
        Register();
    }
    
    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>,IPacket>> m_makeFuc= new
        Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> m_handler = new
        Dictionary<ushort, Action<PacketSession, IPacket>>();
    
    public void Register()
    {
                m_makeFuc.Add((ushort)PacketID.C_LeaveGame, MakePacket<C_LeaveGame>);
        m_handler.Add((ushort)PacketID.C_LeaveGame, PacketHandler.C_LeaveGameHandler);

        m_makeFuc.Add((ushort)PacketID.C_Move, MakePacket<C_Move>);
        m_handler.Add((ushort)PacketID.C_Move, PacketHandler.C_MoveHandler);


    }
    
    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, 
        Action<PacketSession,IPacket> onRecvCallback = null)
    {
    
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
    
    
         Func<PacketSession, ArraySegment<byte>,IPacket> func = null;
        if (m_makeFuc.TryGetValue(id, out func))
        {
            IPacket packet =  func.Invoke(session, buffer);
            if (onRecvCallback != null)
                onRecvCallback.Invoke(session, packet);
            else
                HandlePacket(session, packet);
        }
    }
    
    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer )where T: IPacket, new ()
    {
        T p = new T();
        p.Read(buffer);
        return p;
      
    }

    public void HandlePacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if (m_handler.TryGetValue(packet.Protocol, out action))
            action.Invoke(session, packet);
    }





}
