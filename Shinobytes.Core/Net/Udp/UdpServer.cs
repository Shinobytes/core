///*******************************************************************\
//* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
//* Any usage of the content of this file, in part or whole, without  *
//* a written agreement from Shinobytes, will be considered a         *
//* violation against international copyright law.                    *
//\*******************************************************************/

//using System;
//using System.Net;
//using System.Net.Sockets;

//namespace Shinobytes.Core.Net.Udp
//{
//    public class UdpServer : INetworkServer
//    {
//        private readonly INetworkServerSettings settings;
//        private readonly INetworkConnectionHandler connectionHandler;
//        private readonly ILogger logger;
//        private readonly Socket listener;
//        private bool isRunning = false;
//        private IPEndPoint ipEndPoint;

//        public UdpServer(ILogger logger, INetworkServerSettings settings, INetworkConnectionHandler connectionHandler)
//        {
//            this.settings = settings;
//            this.connectionHandler = connectionHandler;
//            this.logger = logger;

//            this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
//        }

//        public async void StartAsync()
//        {
//            ThrowIfRunning();
//            isRunning = true;
//            ipEndPoint = new IPEndPoint(IPAddress.Parse(settings.Ip), settings.Port);
//            this.listener.Bind(ipEndPoint);
//            logger.WriteDebug("UdpServer Started and listening for incoming connections");
//            while (isRunning)
//            {

//                var connection = await listener.ReceiveFrom();
//                if (isRunning)
//                    connectionHandler.HandleConnect(this, settings, connection.Client);
//            }
//        }

//        public void Start()
//        {
//            ThrowIfRunning();
//            isRunning = true;
//            ipEndPoint = new IPEndPoint(IPAddress.Parse(settings.Ip), settings.Port);
//            this.listener.Bind(ipEndPoint);
//            logger.WriteDebug("UdpServer Started and listening for incoming connections");
//            while (isRunning)
//            {
//                var connection = listener.AcceptTcpClient();
//                if (isRunning)
//                    connectionHandler.HandleConnect(this, settings, connection.Client);
//            }
//        }

//        public void Stop()
//        {
//            ThrowIfNotRunning();
//            isRunning = false;
//            listener.Close();
//            logger.WriteDebug("UdpServer Stopped");
//        }

//        private void ThrowIfRunning()
//        {
//            if (isRunning) throw new Exception("Server is already running");
//        }

//        private void ThrowIfNotRunning()
//        {
//            if (!isRunning) throw new Exception("Server is not running");
//        }
//    }
//}