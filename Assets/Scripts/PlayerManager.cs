using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager 
{
    MyPlayer m_myplayer;
    Dictionary<int, Player> m_players = new Dictionary<int, Player>();

    public static PlayerManager Instance { get; } = new PlayerManager();

    public void Add(S_PlayerList packet)
    {
        Object obj = Resources.Load("Player");

        foreach (S_PlayerList.Player p in packet.players)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;
            if (p.isSelf)
            {
                MyPlayer myPlayer = go.AddComponent<MyPlayer>();
                myPlayer.playerId = p.playerId;
                myPlayer.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                m_myplayer = myPlayer;
            }
            else
            {
                Player player = go.AddComponent<Player>();
                player.playerId = p.playerId;
                player.transform.position = new Vector3(p.posX, p.posY, p.posZ);
                m_players.Add(p.playerId, player);
            }
        }
    }


    public void EnterGmae(S_BroadcastEnterGame enterPacket)
    {
        if (enterPacket.playerId == m_myplayer.playerId)
            return;
        Object obj = Resources.Load("Player");
        GameObject go = Object.Instantiate(obj) as GameObject;
        Player player = go.AddComponent<Player>();
        player.transform.position = new Vector3(enterPacket.posX, 0, enterPacket.posZ);
        m_players.Add(enterPacket.playerId, player);
    }

    public void LevaeGame(S_BroadcastLeaveGame leavePacket)
    {
        if(m_myplayer.playerId == leavePacket.playerId )
        {
            GameObject.Destroy(m_myplayer.gameObject);
            m_myplayer = null;
        }
        else
        {
            Player player = null;
            if(m_players.TryGetValue(leavePacket.playerId, out player))
            {
                GameObject.Destroy(player.gameObject);
                m_players.Remove(leavePacket.playerId);
            }

        }
    }

    public void Move(S_BroadcastMove movePacket)
    {
        if (m_myplayer.playerId == movePacket.playerId)
            m_myplayer.transform.position = new Vector3(movePacket.posX, 0, movePacket.posZ);
        else
        {
            Player player = null;
            if(m_players.TryGetValue(movePacket.playerId,out player))
                player.transform.position = new Vector3(movePacket.posX, 0, movePacket.posZ);
        }
    }







}
