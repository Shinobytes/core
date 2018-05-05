using System;

namespace Shinobytes.Core.Net
{
    public interface INetworkConnectionStream : IDisposable
    {
        PacketReply<T> Read<T>() where T : ISerializablePacketData<T>, new();

        void Write(Packet packet);

        Packet ReadNextPacket();
    }
}