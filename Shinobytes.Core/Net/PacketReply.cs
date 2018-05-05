/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

namespace Shinobytes.Core.Net
{
    public class PacketReply
    {
        public static PacketReply<T> FromValue<T>(T val, Packet response)
        {
            return new PacketReply<T>()
            {
                IsSuccess = true,
                Message = null,
                Data = val,
                ResponsePacket = response
            };
        }

        public static PacketReply<T> FromValue<T>(object val)
        {
            return new PacketReply<T>()
            {
                IsSuccess = true,
                Message = null
            };
        }

        public static PacketReply<T> Error<T>(string message)
        {
            return new PacketReply<T>
            {
                IsSuccess = false,
                Message = message
            };
        }
    }

    public class PacketReply<T> : PacketReply
    {
        public bool IsSuccess;
        public string Message;
        public T Data;
        public Packet ResponsePacket;
    }
}