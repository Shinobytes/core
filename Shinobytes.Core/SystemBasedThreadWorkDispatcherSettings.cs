/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;

namespace Shinobytes.Core
{
    public class SystemBasedThreadWorkDispatcherSettings : IThreadWorkDispatcherSettings
    {
        public SystemBasedThreadWorkDispatcherSettings()
        {
            MaximumConcurrentThreads = Environment.ProcessorCount * 4;
        }

        public int MaximumConcurrentThreads { get; }
        public int MsWaitTime { get; } = 50;
    }
}