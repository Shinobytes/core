/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;

namespace Shinobytes.Core.Net
{
    public interface INetworkSession
    {
        string Key { get; }
        object Token { get; }
        DateTime Created { get; }
        DateTime Expires { get; }

        bool IsRejected { get; }
        bool IsAccepted { get; }
        void Accept(object token);
        void Reject();
        void Terminate();
    }
}