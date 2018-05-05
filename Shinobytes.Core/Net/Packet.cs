/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;
using System.Linq;
using System.Text;

namespace Shinobytes.Core.Net
{
    [Serializable]
    public class Packet
    {
        public const int MAX_PACKET_SIZE = 65536;

        public byte[] Payload;
        public int Size;

        private int packetReadOffset;

        public Packet()
        {
            Payload = new byte[MAX_PACKET_SIZE];
            Size = 0;
            packetReadOffset = 0;
        }

        public Packet Clone()
        {
            return new Packet
            {
                packetReadOffset = packetReadOffset,
                Payload = Payload,
                Size = Size
            };
        }

        public ushort PeekId()
        {
            return BitConverter.ToUInt16(Payload, 0);
        }

        public ushort ReadId()
        {
            packetReadOffset += sizeof(ushort);
            return BitConverter.ToUInt16(Payload, 0);
        }

        public long ReadI8()
        {
            try { return Payload[packetReadOffset] & 0xFF; }
            finally { packetReadOffset += sizeof(sbyte); }
        }

        public short ReadI16()
        {
            try { return BitConverter.ToInt16(Payload, packetReadOffset); }
            finally { packetReadOffset += sizeof(short); }
        }

        public T Read<T>()
        {
            if (typeof(T) == typeof(sbyte)) return (T)(object)ReadI8();
            if (typeof(T) == typeof(short)) return (T)(object)ReadI16();
            if (typeof(T) == typeof(int)) return (T)(object)ReadI32();
            if (typeof(T) == typeof(long)) return (T)(object)ReadI64();

            if (typeof(T) == typeof(byte)) return (T)(object)ReadU8();
            if (typeof(T) == typeof(ushort)) return (T)(object)ReadU16();
            if (typeof(T) == typeof(uint)) return (T)(object)ReadU32();
            if (typeof(T) == typeof(ulong)) return (T)(object)ReadU64();

            if (typeof(T) == typeof(string)) return (T)(object)ReadString();

            if (typeof(T) == typeof(float)) return (T)(object)ReadF32();
            if (typeof(T) == typeof(double)) return (T)(object)ReadF64();

            if (typeof(T) == typeof(bool)) return (T)(object)ReadBool();
            //if (typeof(T) == typeof(Vector2)) return (T)(object)ReadVector2();
            //if (typeof(T) == typeof(Vector3)) return (T)(object)ReadVector3();
            //if (typeof(T) == typeof(Quaternion)) return (T)(object)ReadQuaternion();
            return default(T);
        }

        public T ReadStruct<T>() where T : ISerializablePacketData<T>, new()
        {
            var data = new T();
            data.Deserialize(this);
            return data;
        }

        public int ReadI32()
        {
            try { return BitConverter.ToInt32(Payload, packetReadOffset); }
            finally { packetReadOffset += sizeof(int); }
        }

        public long ReadI64()
        {
            try { return BitConverter.ToInt64(Payload, packetReadOffset); }
            finally { packetReadOffset += sizeof(long); }
        }

        public long ReadU8()
        {
            try { return Payload[packetReadOffset]; }
            finally { packetReadOffset += sizeof(byte); }
        }

        public ushort ReadU16()
        {
            try { return BitConverter.ToUInt16(Payload, packetReadOffset); }
            finally { packetReadOffset += sizeof(ushort); }
        }

        public uint ReadU32()
        {
            try { return BitConverter.ToUInt32(Payload, packetReadOffset); }
            finally { packetReadOffset += sizeof(uint); }
        }

        public ulong ReadU64()
        {
            try { return BitConverter.ToUInt64(Payload, packetReadOffset); }
            finally { packetReadOffset += sizeof(ulong); }
        }

        public float ReadF32()
        {
            try { return BitConverter.ToSingle(Payload, packetReadOffset); }
            finally { packetReadOffset += sizeof(float); }
        }

        public double ReadF64()
        {
            try { return BitConverter.ToDouble(Payload, packetReadOffset); }
            finally { packetReadOffset += sizeof(double); }
        }

        public bool ReadBool()
        {
            return ReadU8() == 1;
        }

        public byte[] Read(int length)
        {
            try { return Payload.Skip(packetReadOffset).Take(length).ToArray(); }
            finally { packetReadOffset += sizeof(byte) * length; }
        }

        public string ReadString()
        {
            var readLength = ReadI32();
            var bytes = Read(readLength);
            return Encoding.UTF8.GetString(bytes);
        }

        //public Vector2 ReadVector2()
        //{
        //    return new Vector2(ReadF32(), ReadF32());
        //}

        //public Vector3 ReadVector3()
        //{
        //    return new Vector3(ReadF32(), ReadF32(), ReadF32());
        //}

        //public Quaternion ReadQuaternion()
        //{
        //    return new Quaternion(ReadF32(), ReadF32(), ReadF32(), ReadF32());
        //}

        public void ResetPosition()
        {
            Position = 0;
        }

        public int Position
        {
            get { return packetReadOffset; }
            set { packetReadOffset = value; }
        }

        public static Packet FromBytes(byte[] payload, int offset, int size)
        {
            return new Packet
            {
                packetReadOffset = offset,
                Payload = payload,
                Size = size
            };
        }

        //public MapNode ReadMapNode()
        //{
        //    return new MapNode
        //    {
        //        Name = ReadString(),
        //        Position = ReadVector2()
        //    };
        //}
    }
}