using NAudio.Wave;
using ProtoBuf;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Basic_Voice_Chat.Code.Server
{
    internal class ServerManager
    {
        private readonly ICoreServerAPI _sapi;

        public ServerManager(ICoreServerAPI api)
        {
            _sapi = api;

            _sapi.Network.GetChannel("basicvoicechat:network-channel")
                .SetMessageHandler<AudioData>(OnPlayerTalk);
        }

        private void OnPlayerTalk(IServerPlayer talkingPlayer, AudioData audioData)
        {
            _sapi.Network.GetChannel("basicvoicechat:network-channel")
                .BroadcastPacket(audioData, talkingPlayer);
        }
    }

    [ProtoContract]
    public sealed class AudioData
    {
        [ProtoMember(1)]
        public byte[] Buffer { get; set; }

        [ProtoMember(2)]
        public Vec3d Origin { get; set; }

        public AudioData()
        {
            Buffer = [];
            Origin = new();
        }

        public AudioData(byte[] buffer, Vec3d origin)
        {
            Buffer = buffer;
            Origin = origin;
        }
    }
}
