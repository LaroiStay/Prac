using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
   
    NetWorkManager m_network;
    void Start()
    {
        StartCoroutine(CoSendPacket());
        m_network = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);

            C_Move movePacket = new C_Move();
            movePacket.posX = UnityEngine.Random.Range(-50, 50);
            movePacket.posY = 0;
            movePacket.posZ = UnityEngine.Random.Range(-50, 50);
            m_network.Send(movePacket.Write());
        }
    }

}
