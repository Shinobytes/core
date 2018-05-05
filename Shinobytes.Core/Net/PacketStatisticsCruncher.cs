/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;
using System.Threading;

namespace Shinobytes.Core.Net
{
    public class PacketStatisticsCruncher
    {
        private readonly object feedLock = new object();
        private long totalRequestCount;
        private int requestCountLastSecond;
        private DateTime lastRequest;
        private DateTime firstRequest = DateTime.MinValue;
        private DateTime lastSecondUpdate = DateTime.MinValue;

        private float lastRequestPerSecond;

        public void RequestReceived()
        {
            lock (feedLock)
            {
                Interlocked.Increment(ref totalRequestCount);
                requestCountLastSecond++;

                var time = DateTime.Now;
                if (firstRequest == DateTime.MinValue)
                    firstRequest = time;
                else
                {
                    var lastRequestElapsed = time - lastSecondUpdate;
                    if (lastRequestElapsed.TotalSeconds >= 1.000)
                    {                        
                        lastRequestPerSecond = requestCountLastSecond / (float)lastRequestElapsed.TotalSeconds;                        
                        requestCountLastSecond = 0;
                        lastSecondUpdate = time;
                    }
                }
                lastRequest = time;
            }
        }

        public double GetRequestsPerSeconds()
        {
            lock (feedLock)
            {
                return lastRequestPerSecond;
            }
        }
    }
}