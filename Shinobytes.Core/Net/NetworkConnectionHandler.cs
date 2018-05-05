/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System.Net.Sockets;

namespace Shinobytes.Core.Net
{
    public class NetworkConnectionHandler : INetworkConnectionHandler
    {
        private readonly ILogger logger;
        private readonly INetworkConnectionManager connectionManager;
        private readonly INetworkConnectionProvider connectionProvider;
        private readonly INetworkSessionManager sessionManager;
        private readonly INetworkSessionProvider sessionProvider;

        public NetworkConnectionHandler(ILogger logger, INetworkConnectionManager connectionManager, INetworkConnectionProvider connectionProvider, INetworkSessionManager sessionManager, INetworkSessionProvider sessionProvider)
        {
            this.logger = logger;
            this.connectionManager = connectionManager;
            this.connectionProvider = connectionProvider;
            this.sessionManager = sessionManager;
            this.sessionProvider = sessionProvider;
        }

        public void HandleConnect(INetworkServer networkServer, INetworkServerSettings settings, Socket connection)
        {
            var conn = connectionProvider.Get(connection);
            if (conn != null)
            {
                //logger.WriteDebug($"Client from '{conn.RemoteEndPoint}' connected!");
                if (sessionManager.NewAllowed())
                {
                    conn.SetSession(sessionProvider.New(this, conn)); // creates a temporarily session                    
                    connectionManager.Add(conn);
                    conn.BeginHandlePackets();
                }
                else
                {
                    logger.WriteDebug("Cannot generate any more sessions. Closing connection.");
                    conn.Close(true);
                }
            }
        }

        public void HandleDisconnect(INetworkConnection connection)
        {
            //logger.WriteDebug($"Client from '{connection.RemoteEndPoint}' disconnected!");
            connectionManager.Remove(connection);
        }
    }
}