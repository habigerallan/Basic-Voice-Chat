using Basic_Voice_Chat.Code.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace Basic_Voice_Chat.Code.Client.NAudio.Effects
{
    internal class GainEffect(ICoreClientAPI _capi) : IEffect(_capi)
    {
        private static readonly int MULTIPLIER = 2;
        public override void Apply(ref VoiceChatAudioData audioData)
        {
            for (int i = 0; i < audioData.Buffer.Length; i += 2)
            {
                short monoSample = ReadSample16(audioData.Buffer, i);

                short scaledShort = Clamp16(monoSample * MULTIPLIER);

                WriteSample16(audioData.Buffer, i, scaledShort);
            }
        }
    }
}
