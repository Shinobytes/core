/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System.Collections.Generic;

namespace Shinobytes.Core.Net
{
    public interface INetworkSessionManager
    {
        //INetworkSession Generate(INetworkConnectionHandler connectionHandler, INetworkConnection connection);
        IReadOnlyList<INetworkSession> AllSessions();
        INetworkSession Get(string key);
        INetworkSession GetByToken(object token);

        bool NewAllowed();
        void Add(INetworkSession networkSession);
        void Remove(INetworkSession networkSession);
        void Accept(INetworkSession networkSession);
        void Reject(INetworkSession networkSession);

        INetworkSessionManagerSettings Settings { get; }
    }
}