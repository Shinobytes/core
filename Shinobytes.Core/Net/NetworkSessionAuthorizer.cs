/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;

namespace Shinobytes.Core.Net
{
    public class NetworkSessionAuthorizer : INetworkSessionAuthorizer
    {
        private readonly INetworkSessionManager sessionManager;

        public NetworkSessionAuthorizer(INetworkSessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        public void ThrowIfNotAuthorized(string sessionKey, INetworkConnection connection)
        {
            var session = sessionManager.Get(sessionKey);
            if (session == null) throw new UnauthorizedAccessException($"Target session '{sessionKey}' not found.");
            var connSession = connection.GetSession();
            if (connSession == null || connSession.IsRejected) throw new UnauthorizedAccessException($"Current session has been rejected.");
            if (!session.IsAccepted) throw new UnauthorizedAccessException($"Target session '{sessionKey}' was never accepted.");
            sessionManager.Remove(connSession); // remove old session
            connection.SetSession(session);     // give ownership of session
        }
    }
}