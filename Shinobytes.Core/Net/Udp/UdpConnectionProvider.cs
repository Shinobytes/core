/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;
using System.Net.Sockets;

namespace Shinobytes.Core.Net.Udp
{
    public class UdpConnectionProvider : INetworkConnectionProvider
    {
        private readonly ILogger logger;
        private readonly INetworkConnectionSettings settings;
        private readonly INetworkPacketHandler packetHandler;

        public UdpConnectionProvider(ILogger logger, INetworkConnectionSettings settings, INetworkPacketHandler packetHandler)
        {
            this.logger = logger;
            this.settings = settings;
            this.packetHandler = packetHandler;
        }

        public INetworkConnection Get(Socket socket)
        {
            return new UdpConnection(logger, Guid.NewGuid(), socket, settings, packetHandler);
        }
    }
}