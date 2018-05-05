/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

namespace Shinobytes.Core.Net.ValueTypes
{
    public class SerializableValueType<T, TValType> : ISerializablePacketData<T>
    {
        public TValType Value { get; protected set; }
        public virtual void Deserialize(Packet packet)
        {
            Value = packet.Read<TValType>();
        }

        public virtual void Serialize(PacketBuilder writer)
        {
            writer.WriteValueType(Value);
        }
    }
}