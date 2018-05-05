/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System.Collections.Generic;
using System.Linq;

namespace Shinobytes.Core.Net
{
    public class NetworkSessionManager : INetworkSessionManager
    {
        private readonly ILogger logger;
        private readonly INetworkSessionManagerSettings settings;
        private readonly List<INetworkSession> sessions;
        private readonly List<INetworkSession> acceptedSessions;

        public NetworkSessionManager(ILogger logger, INetworkSessionManagerSettings settings)
        {
            this.sessions = new List<INetworkSession>();
            this.acceptedSessions = new List<INetworkSession>();
            this.logger = logger;
            this.settings = settings;
        }

        //public INetworkSession Generate(INetworkConnectionHandler connectionHandler, INetworkConnection connection)
        //{
        //    var newSession = new NetworkSession(Guid.NewGuid().ToString().Replace("-", ""),
        //                                    this,
        //                                    connectionHandler,
        //                                    connection);
        //    sessions.Add(newSession);
        //    return newSession;
        //}

        public IReadOnlyList<INetworkSession> AllSessions()
        {
            return sessions;
        }

        public IReadOnlyList<INetworkSession> AcceptedSessions()
        {
            return acceptedSessions;
        }

        public INetworkSession Get(string key)
        {
            return sessions.FirstOrDefault(s => s != null && s.Key == key);
        }

        public INetworkSession GetByToken(object token)
        {
            return sessions.FirstOrDefault(s => object.Equals(s.Token, token));
        }

        public void Add(INetworkSession networkSession)
        {
            this.sessions.Add(networkSession);
        }

        public void Remove(INetworkSession networkSession)
        {
            if (networkSession.IsAccepted) logger.WriteDebug($"Session: '{networkSession.Key}' removed");
            this.sessions.Remove(networkSession);
        }

        public void Accept(INetworkSession networkSession)
        {
            acceptedSessions.Add(networkSession);
        }

        public void Reject(INetworkSession networkSession)
        {
            if (acceptedSessions.Contains(networkSession))
            {
                acceptedSessions.Remove(networkSession);
            }
        }

        public INetworkSessionManagerSettings Settings => settings;

        public bool NewAllowed()
        {
            if (settings.MaximumConcurrentSessions <= 0) return true;
            return sessions.Count + 1 < settings.MaximumConcurrentSessions;
        }
    }
}