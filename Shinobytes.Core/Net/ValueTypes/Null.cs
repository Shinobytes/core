/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

namespace Shinobytes.Core.Net.ValueTypes
{
    public class Null : ISerializablePacketData<Null>
    {
        public void Deserialize(Packet packet) { }
        public void Serialize(PacketBuilder writer) { }
    }
}