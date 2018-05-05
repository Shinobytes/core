/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

namespace Shinobytes.Core.Net
{
    public class NetworkSessionManagerSettings : INetworkSessionManagerSettings
    {
        public NetworkSessionManagerSettings(int maximumConcurrentSessions, bool overwriteSessionsUsingSameToken)
        {
            MaximumConcurrentSessions = maximumConcurrentSessions;
            OverwriteSessionsUsingSameToken = overwriteSessionsUsingSameToken;
        }

        public int MaximumConcurrentSessions { get; }
        public bool OverwriteSessionsUsingSameToken { get; }
    }
}