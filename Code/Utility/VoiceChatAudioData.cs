using ProtoBuf;
using Vintagestory.API.MathTools;

namespace Basic_Voice_Chat.Code.Utility
{
    [ProtoContract]
    public sealed class VoiceChatAudioData
    {
        [ProtoMember(1)]
        public byte[] Buffer { get; set; }

        [ProtoMember(2)]
        public Vec3d Origin { get; set; }

        public int NumberOfChannels {  get; set; }

        public VoiceChatAudioData()
        {
            Buffer = [];
            Origin = new();
            NumberOfChannels = 0;
        }

        public VoiceChatAudioData(byte[] buffer, Vec3d origin)
        {
            Buffer = buffer;
            Origin = origin;
            NumberOfChannels = 0;
        }
    }
}
