/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

namespace Shinobytes.Core.Net
{
    public class NetworkServerSettings : INetworkServerSettings
    {
        public NetworkServerSettings(string ip, int port, bool keepAlive)
        {
            Ip = ip;
            Port = port;
            KeepAlive = keepAlive;
        }

        public string Ip { get; }
        public int Port { get; }
        public bool KeepAlive { get; }
    }
}