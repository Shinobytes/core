/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System.Net.Sockets;

namespace Shinobytes.Core.Net
{
    public interface INetworkConnectionProvider
    {
        INetworkConnection Get(Socket socket);
    }
}