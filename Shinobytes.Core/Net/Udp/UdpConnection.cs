/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;
using System.Net;
using System.Net.Sockets;

namespace Shinobytes.Core.Net.Udp
{
    public class UdpConnection : INetworkConnection
    {
        private readonly ILogger logger;
        private readonly Socket socket;
        private readonly INetworkConnectionSettings settings;
        private readonly INetworkPacketHandler packetHandler;
        private INetworkSession currentSession;
        private bool isDisposed;
        private NetworkConnectionStream currentStream;

        public UdpConnection(ILogger logger, Guid instanceIdentifier, Socket socket, INetworkConnectionSettings settings, INetworkPacketHandler packetHandler)
        {
            this.logger = logger;
            this.socket = socket;
            this.settings = settings;
            this.packetHandler = packetHandler;
            this.RemoteEndPoint = socket.RemoteEndPoint;
            InstanceIdentifier = instanceIdentifier;
        }

        public Guid InstanceIdentifier { get; }
        public EndPoint RemoteEndPoint { get; }
        public INetworkConnectionStream GetStream()
        {
            if (currentStream != null && !currentStream.Disposed) return currentStream;
            return currentStream = new NetworkConnectionStream(this.socket, settings.KeepAlive);
        }

        public INetworkSession GetSession()
        {
            return currentSession;
        }

        public void SetSession(INetworkSession session)
        {
            this.currentSession = session;
        }

        public void BeginHandlePackets()
        {
            this.HandlePackets();
        }


        public void RejectSession()
        {
            this.logger.WriteDebug($"Session reject for '{this.RemoteEndPoint}'");
            this.currentSession?.Reject();
        }

        public void AcceptSession(object token)
        {
            this.logger.WriteDebug($"Session accepted for '{this.RemoteEndPoint}' with key: '{this.currentSession.Key}'");
            this.currentSession?.Accept(token);
        }

        private void HandlePackets()
        {
            new System.Threading.Thread(() =>
            {
                using (var stream = GetStream())
                {
                    while (Connected)
                    {
                        var readNextPacket = stream.ReadNextPacket();
                        if (readNextPacket == null)
                        {
                            Close(false);
                            return;
                        }

                        packetHandler.HandleClientPacket(this, readNextPacket);
                    }
                }
            }).Start();
        }

        public bool Connected => !isDisposed && socket != null && socket.Connected;

        public void Close(bool forceTerminateSession)
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    socket.Close();
                }
            }
            catch (Exception exc)
            {
                logger.WriteError($"Exception thrown when closing the client connection. Reason: {exc.Message}");
            }
            finally
            {
                if (currentSession != null)
                {
                    if (!currentSession.IsAccepted || forceTerminateSession)
                        currentSession?.Terminate();
                }
            }
        }

        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;
            Close(false);
        }
    }
}