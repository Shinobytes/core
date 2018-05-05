/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;
using System.Net;
using System.Net.Sockets;

namespace Shinobytes.Core.Net.Tcp
{
    public class TcpServer : INetworkServer
    {
        private readonly INetworkServerSettings settings;
        private readonly INetworkConnectionHandler connectionHandler;
        private readonly ILogger logger;
        private readonly TcpListener listener;
        private bool isRunning = false;

        public TcpServer(ILogger logger, INetworkServerSettings settings, INetworkConnectionHandler connectionHandler)
        {
            this.settings = settings;
            this.connectionHandler = connectionHandler;
            this.logger = logger;
            this.listener = new TcpListener(IPAddress.Parse(settings.Ip), settings.Port);
        }

        public async void StartAsync()
        {
            ThrowIfRunning();
            isRunning = true;
            listener.Start();
            logger.WriteDebug("TcpServer Started and listening for incoming connections");            
            while (isRunning)
            {
                var connection = await listener.AcceptTcpClientAsync();
                if (isRunning)
                    connectionHandler.HandleConnect(this, settings, connection.Client);
            }
        }

        public void Start()
        {
            ThrowIfRunning();
            isRunning = true;
            listener.Start();
            logger.WriteDebug("TcpServer Started and listening for incoming connections");            
            while (isRunning)
            {
                var connection = listener.AcceptTcpClient();
                if (isRunning)
                    connectionHandler.HandleConnect(this, settings, connection.Client);
            }
        }

        public void Stop()
        {
            ThrowIfNotRunning();
            isRunning = false;
            listener.Stop();
            logger.WriteDebug("TcpServer Stopped");
        }

        private void ThrowIfRunning()
        {
            if (isRunning) throw new Exception("Server is already running");
        }

        private void ThrowIfNotRunning()
        {
            if (!isRunning) throw new Exception("Server is not running");
        }
    }
}