using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class SessionManager
    {
        static SessionManager m_session = new SessionManager();
        public static SessionManager Instance { get { return m_session; } }

        int m_sessionid = 0;
        Dictionary<int, ClientSession> m_sessions = new Dictionary<int, ClientSession>();
        object m_lock = new object();
        public ClientSession Generate()
        {
            lock (m_lock)
            {
                int sessionid = ++m_sessionid;
                ClientSession session = new ClientSession();
                session.Sessionid = sessionid;
                m_sessions.Add(sessionid, session);
                return session;
            }
        }

        public ClientSession Find(int id)
        {
            ClientSession session = null;
            if (m_sessions.TryGetValue(id, out session))
                return session;
            return null;
            
        }

        public void Remove(ClientSession session)
        {
            lock (m_lock)
            {
                m_sessions.Remove(session.Sessionid);
            }
        }





    }
}
