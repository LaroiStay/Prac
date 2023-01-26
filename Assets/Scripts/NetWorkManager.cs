using System.Collections;
using System.Collections.Generic;
using System.Net;
using ServerCore;
using UnityEngine;
using DummyClient;
using System;

public class NetWorkManager : MonoBehaviour
{
    ServerSession m_sessions = new ServerSession();

    public void Send(ArraySegment<byte> sendBuff) { m_sessions.Send(sendBuff); }

    void Start()
    {


        string host = Dns.GetHostName();
        IPHostEntry IPhost = Dns.GetHostEntry(host);
        IPAddress IPAdr = IPhost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(IPAdr, 7777);

        Connecter connecter = new Connecter();
        connecter.Connect(endPoint, () => { return m_sessions; },
            1);
    }

    void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach(IPacket packet in list)
            PacketManager.Instance.HandlePacket(m_sessions,packet);
    }


    




}
