/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System.Net.Sockets;

namespace Shinobytes.Core.Net
{
    public class NetworkConnectionStream : INetworkConnectionStream
    {
        private readonly Socket socket;
        private readonly bool keepAlive;
        private readonly NetworkStream stream;

        public NetworkConnectionStream(Socket socket, bool keepAlive)
        {
            this.socket = socket;
            this.keepAlive = keepAlive;
            this.stream = new NetworkStream(socket);
        }

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            Disposed = true;
            if (!keepAlive)
            {
                try
                {
                    this.socket.Close();
                }
                catch { }
            }
        }

        public void Write(Packet packet)
        {
            this.stream.Write(packet.Payload, 0, packet.Size);
            this.stream.Flush();
        }

        public Packet ReadNextPacket()
        {
            try
            {
                var readBuffer = new byte[Packet.MAX_PACKET_SIZE];
                var read = this.stream.Read(readBuffer, 0, readBuffer.Length);
                return read <= 0 ? null : Packet.FromBytes(readBuffer, 0, read);
            }
            catch
            {
                return null;
            }
        }

        public PacketReply<T> Read<T>() where T : ISerializablePacketData<T>, new()
        {
            var packet = ReadNextPacket();
            if (packet == null) return PacketReply.Error<T>("disconnected by server");

            var data = new T();
            data.Deserialize(packet);
            return PacketReply.FromValue<T>(data, packet);
        }
    }
}