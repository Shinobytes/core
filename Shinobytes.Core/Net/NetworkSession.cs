/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;

namespace Shinobytes.Core.Net
{
    public class NetworkSession : INetworkSession
    {
        private readonly INetworkSessionManager sessionManager;
        private readonly INetworkConnectionHandler connectionHandler;
        private readonly INetworkConnection connection;

        public NetworkSession(string key, INetworkSessionManager sessionManager, INetworkConnectionHandler connectionHandler, INetworkConnection connection)
        {
            this.sessionManager = sessionManager;
            this.connectionHandler = connectionHandler;
            this.connection = connection;
            this.Key = key;
            this.Created = DateTime.UtcNow;
            this.Expires = DateTime.UtcNow.AddMinutes(20);
        }

        public string Key { get; }

        public DateTime Created { get; }

        public DateTime Expires { get; }

        public bool IsRejected { get; private set; }

        public bool IsAccepted { get; private set; }

        public object Token { get; private set; }

        public void Accept(object token)
        {
            if (token != null && sessionManager.Settings.OverwriteSessionsUsingSameToken)
            {
                var existingSession = sessionManager.GetByToken(token);
                existingSession?.Terminate();
            }

            Token = token;
            IsRejected = false;
            IsAccepted = true;
            sessionManager.Accept(this);
        }

        public void Reject()
        {
            Token = null;
            IsRejected = true;
            IsAccepted = false;
            sessionManager.Reject(this);
        }

        public void Terminate()
        {
            if (connectionHandler == null) throw new NullReferenceException();
            sessionManager.Remove(this);
            connectionHandler.HandleDisconnect(connection);
        }

    }
}