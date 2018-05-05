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
    public class PacketBuilder
    {
        private Packet packet;

        public PacketBuilder()
        {
            packet = new Packet();
        }

        public PacketBuilder Write(byte[] bytes)
        {
            var localRef = this;
            var addSize = sizeof(byte) * bytes.Length;
            AssertPacketSize(packet.Size, addSize);
            return bytes.Aggregate(localRef, (current, b) => current.Write(b));
        }

        public PacketBuilder Write(string str)
        {
            var data = Encoding.UTF8.GetBytes(str);
            var self = Write(data.Length);
            return self.Write(data);
        }

        public PacketBuilder Write(byte u8)
        {
            var addSize = sizeof(byte);
            AssertPacketSize(packet.Size, addSize);
            packet.Payload[packet.Size] = u8;
            packet.Size += addSize;
            return this;
        }

        public PacketBuilder Write(sbyte i8)
        {
            return Write((byte)i8);
        }

        public PacketBuilder Write(short i16)
        {
            var addSize = sizeof(short);
            AssertPacketSize(packet.Size, addSize);
            packet.Payload[packet.Size + 1] = (byte)(i16 >> 8);
            packet.Payload[packet.Size + 0] = (byte)i16;
            packet.Size += addSize;
            return this;
        }

        public PacketBuilder Write(int i32)
        {
            var addSize = sizeof(int);
            AssertPacketSize(packet.Size, addSize);
            packet.Payload[packet.Size + 3] = (byte)(i32 >> 24);
            packet.Payload[packet.Size + 2] = (byte)(i32 >> 16);
            packet.Payload[packet.Size + 1] = (byte)(i32 >> 8);
            packet.Payload[packet.Size + 0] = (byte)i32;
            packet.Size += addSize;
            return this;
        }

        public PacketBuilder Write(long i64)
        {
            var addSize = sizeof(long);
            AssertPacketSize(packet.Size, addSize);
            packet.Payload[packet.Size + 7] = (byte)(i64 >> 56);
            packet.Payload[packet.Size + 6] = (byte)(i64 >> 48);
            packet.Payload[packet.Size + 5] = (byte)(i64 >> 40);
            packet.Payload[packet.Size + 4] = (byte)(i64 >> 32);
            packet.Payload[packet.Size + 3] = (byte)(i64 >> 24);
            packet.Payload[packet.Size + 2] = (byte)(i64 >> 16);
            packet.Payload[packet.Size + 1] = (byte)(i64 >> 8);
            packet.Payload[packet.Size + 0] = (byte)i64;
            packet.Size += addSize;
            return this;
        }

        public PacketBuilder Write(float f32)
        {
            var addSize = sizeof(float);
            AssertPacketSize(packet.Size, addSize);
            var data = BitConverter.GetBytes(f32);
            packet.Payload[packet.Size + 0] = data[0];
            packet.Payload[packet.Size + 1] = data[1];
            packet.Payload[packet.Size + 2] = data[2];
            packet.Payload[packet.Size + 3] = data[3];
            packet.Size += addSize;
            return this;
        }

        public PacketBuilder Write(double f64)
        {
            var addSize = sizeof(double);
            AssertPacketSize(packet.Size, addSize);
            var data = BitConverter.GetBytes(f64);
            packet.Payload[packet.Size + 0] = data[0];
            packet.Payload[packet.Size + 1] = data[1];
            packet.Payload[packet.Size + 2] = data[2];
            packet.Payload[packet.Size + 3] = data[3];
            packet.Payload[packet.Size + 4] = data[4];
            packet.Payload[packet.Size + 5] = data[5];
            packet.Payload[packet.Size + 6] = data[6];
            packet.Payload[packet.Size + 7] = data[7];
            packet.Size += addSize;
            return this;
        }

        //public PacketBuilder Write(Vector2 vector2)
        //{
        //    var addSize = sizeof(float) * 2;
        //    var localRef = this;
        //    AssertPacketSize(packet.Size, addSize);
        //    localRef = localRef.Write(vector2.x);
        //    localRef = localRef.Write(vector2.y);
        //    return localRef;
        //}

        //public PacketBuilder Write(Vector3 vector3)
        //{
        //    var addSize = sizeof(float) * 3;
        //    var localRef = this;
        //    AssertPacketSize(packet.Size, addSize);
        //    localRef = localRef.Write(vector3.x);
        //    localRef = localRef.Write(vector3.y);
        //    localRef = localRef.Write(vector3.z);
        //    return localRef;
        //}


        //public PacketBuilder Write(Quaternion quat)
        //{
        //    var addSize = sizeof(float) * 4;
        //    var localRef = this;
        //    AssertPacketSize(packet.Size, addSize);
        //    localRef = localRef.Write(quat.x);
        //    localRef = localRef.Write(quat.y);
        //    localRef = localRef.Write(quat.z);
        //    localRef = localRef.Write(quat.w);
        //    return localRef;
        //}

        public PacketBuilder Write<T>(T data) where T : ISerializablePacketData<T>
        {
            var localRef = this;
            data.Serialize(localRef);
            return localRef;
        }


        public PacketBuilder WriteValueType<T>(T value)
        {
            if (typeof(T) == typeof(sbyte)) return Write((sbyte)(object)value);
            if (typeof(T) == typeof(short)) return Write((short)(object)value);
            if (typeof(T) == typeof(int)) return Write((int)(object)value);
            if (typeof(T) == typeof(long)) return Write((long)(object)value);

            if (typeof(T) == typeof(byte)) return Write((byte)(object)value);
            if (typeof(T) == typeof(ushort)) return Write((ushort)(object)value);
            if (typeof(T) == typeof(uint)) return Write((uint)(object)value);
            if (typeof(T) == typeof(ulong)) return Write((ulong)(object)value);

            if (typeof(T) == typeof(string)) return Write((string)(object)value);

            if (typeof(T) == typeof(float)) return Write((float)(object)value);
            if (typeof(T) == typeof(double)) return Write((double)(object)value);

            if (typeof(T) == typeof(bool)) return Write((bool)(object)value);
            //if (typeof(T) == typeof(Vector2)) return Write((Vector2)(object)value);
            //if (typeof(T) == typeof(Vector3)) return Write((Vector3)(object)value);
            //if (typeof(T) == typeof(Quaternion)) return Write((Quaternion)(object)value);
            return this;
        }

        public PacketBuilder Write(ulong u64)
        {
            return Write((long)u64);
        }

        public PacketBuilder Write(uint u32)
        {
            return Write((int)u32);
        }

        public PacketBuilder Write(ushort u16)
        {
            return Write((short)u16);
        }

        public PacketBuilder Write(bool boolean)
        {
            return Write((byte)(boolean ? 1 : 0));
        }

        public Packet AsPacket()
        {
            return packet;
        }

        private void AssertPacketSize(int currentPacketSize, int bytesToAdd)
        {
            // check if this packet needs to be split into several packets
            if (currentPacketSize + bytesToAdd >= Packet.MAX_PACKET_SIZE)
            {
                // it does! and since we havn't implemented any support for it yet, panic!     
                throw new IndexOutOfRangeException("Automatic packet splitting has not been implemented. Target packet size of '" + (currentPacketSize + bytesToAdd) + "' " +
                                                   "is too large and needs to be splitted into multiple packets. Maximum packet size is '" + Packet.MAX_PACKET_SIZE + "'");
            }
        }

    }
}
