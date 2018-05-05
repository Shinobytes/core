/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;

namespace Shinobytes.Core.Net
{
    public class NetworkSessionProvider : INetworkSessionProvider
    {
        private readonly INetworkSessionManager sessionManager;

        public NetworkSessionProvider(INetworkSessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        public INetworkSession New(INetworkConnectionHandler connectionHandler, INetworkConnection connection)
        {
            var newSession = new NetworkSession(Guid.NewGuid().ToString().Replace("-", ""),
                sessionManager,
                connectionHandler,
                connection);
            sessionManager.Add(newSession);
            return newSession;
        }
    }
}