/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;
using System.Net.Sockets;

namespace Shinobytes.Core.Net.Tcp
{
    public class TcpConnectionProvider : INetworkConnectionProvider
    {
        private readonly ILogger logger;
        private readonly IThreadWorkDispatcher workDispatcher;
        private readonly INetworkConnectionSettings settings;
        private readonly INetworkPacketHandler packetHandler;

        public TcpConnectionProvider(ILogger logger, IThreadWorkDispatcher workDispatcher, INetworkConnectionSettings settings, INetworkPacketHandler packetHandler)
        {
            this.logger = logger;
            this.workDispatcher = workDispatcher;
            this.settings = settings;
            this.packetHandler = packetHandler;
        }

        public INetworkConnection Get(Socket client)
        {
            return new TcpConnection(logger, Guid.NewGuid(), client, workDispatcher, settings, packetHandler);
        }
    }
}