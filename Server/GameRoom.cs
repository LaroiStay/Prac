using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class GameRoom: IJobQueue
    {
        List<ClientSession> m_sessions = new List<ClientSession>();
        JobQueue m_jobqueue = new JobQueue();
        List<ArraySegment<byte>> m_pendinglist = new List<ArraySegment<byte>>();


        public void Push(Action job)
        {
            m_jobqueue.Push(job);
        }


        public void Enter(ClientSession session)
        {
            m_sessions.Add(session);
            session.Room = this;

            S_PlayerList players = new S_PlayerList();  
            foreach(ClientSession s in m_sessions)
            {
                players.players.Add(new S_PlayerList.Player()
                {   isSelf =(s==session),
                    playerId = s.Sessionid,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ
                });
            }
            session.Send(players.Write());

            S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
            enter.playerId = session.Sessionid;
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;
            BroadCast(enter.Write());
        }

        public void Flush()
        { 
            foreach (ClientSession ses in m_sessions)
                ses.Send(m_pendinglist);
           // Console.WriteLine($"Flushed {m_pendinglist.Count} item");
            m_pendinglist.Clear();
        }



        public void BroadCast(ArraySegment<byte> segment)
        {
         
            m_pendinglist.Add(segment);
          
        }


        public void Leave(ClientSession session)
        {
              m_sessions.Remove(session);
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.Sessionid;
            BroadCast(leave.Write());
        }

        public void Move(ClientSession session, C_Move packet)
        {
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;

            S_BroadcastMove move = new S_BroadcastMove();
            move.playerId = session.Sessionid;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;

            BroadCast(move.Write());

        }




    }
}
