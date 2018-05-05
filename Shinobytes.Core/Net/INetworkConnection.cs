/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;
using System.Net;

namespace Shinobytes.Core.Net
{
    public interface INetworkConnection : IDisposable
    {
        Guid InstanceIdentifier { get; }
        EndPoint RemoteEndPoint { get; }
        INetworkConnectionStream GetStream();
        INetworkSession GetSession();
        void SetSession(INetworkSession session);
        void BeginHandlePackets();
        void RejectSession();
        void AcceptSession(object token);
        void Close(bool forceTerminateSession);
    }
}