/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shinobytes.Core.Net
{
    public abstract class PacketHandlerBase : INetworkPacketHandler
    {
        public delegate Task RequestHandlerAsync(INetworkConnection connection, Packet packet);

        private readonly Dictionary<int, RequestHandlerAsync> handlers = new Dictionary<int, RequestHandlerAsync>();

        protected readonly ILogger Logger;

        protected static PacketStatisticsCruncher PacketStatisticsCruncher = new PacketStatisticsCruncher();

        protected PacketHandlerBase(ILogger logger)
        {
            this.Logger = logger;
        }

        public void Register(short packetId, RequestHandlerAsync handler)
        {
            handlers.Add(packetId, handler);
        }

        public async void HandleClientPacket(INetworkConnection connection, Packet packet)
        {
            PacketStatisticsCruncher.RequestReceived();

            try
            {
                if (packet == null)
                {
                    //logger.WriteError("Client has been disconnected but HandleClientPacket was still called.");
                    connection.Close(false);
                    return; // user has been terminated
                }
                var id = packet.ReadId();

                if (handlers.ContainsKey(id))
                {
                    await handlers[id](connection, packet);
                }
                else
                {
                    await HandleUnknownPacketAsync(connection, packet);
                }
            }
            catch (Exception exc)
            {
                Logger.WriteError(exc.ToString());
            }
            finally
            {
                Logger.SetTopic("Packets per seconds: " + PacketStatisticsCruncher.GetRequestsPerSeconds());
            }
        }

        protected abstract Task HandleUnknownPacketAsync(INetworkConnection connection, Packet packet);
    }
}