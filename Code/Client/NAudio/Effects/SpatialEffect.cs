using Basic_Voice_Chat.Code.Server;
using Basic_Voice_Chat.Code.Utility;
using Vintagestory.API.Client;
namespace Basic_Voice_Chat.Code.Client.NAudio.Effects
{
    internal class SpatialEffect(ICoreClientAPI capi) : IEffect(capi)
    {
        public override void Apply(ref VoiceChatAudioData audioData)
        {
            int baseLength = audioData.Buffer.Length;

            // 2.0 | L, R
            if (audioData.NumberOfChannels == 2)
            {
                byte[] newBytes = new byte[baseLength * 2];

                audioData.Buffer = newBytes;
            }

            // 2.1 | L, R, LFE
            else if (audioData.NumberOfChannels == 3)
            {
                byte[] newBytes = new byte[baseLength * 3];

                audioData.Buffer = newBytes;
            }

            // 5.0 | FL, FR, C, SL, SR
            else if (audioData.NumberOfChannels == 5)
            {
                byte[] newBytes = new byte[baseLength * 5];

                audioData.Buffer = newBytes;
            }

            // 5.1 | FL, FR, C, SL, SR, LFE
            else if (audioData.NumberOfChannels == 6)
            {
                byte[] newBytes = new byte[baseLength * 6];

                audioData.Buffer = newBytes;

            }

            // 7.0 | FL, FR, C, SL, SR, RL, RR
            else if (audioData.NumberOfChannels == 7)
            {
                byte[] newBytes = new byte[baseLength * 7];

                audioData.Buffer = newBytes;

            }

            // 7.1 | FL, FR, C, SL, SR, RL, RR, LFE
            else if (audioData.NumberOfChannels == 8)
            {
                byte[] newBytes = new byte[baseLength * 8];

                audioData.Buffer = newBytes;
            }

            // Unsupported, does nothing
            return;
        }
    }
}
