using Basic_Voice_Chat.Code.Server;
using Basic_Voice_Chat.Code.Utility;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Basic_Voice_Chat.Code.Client.NAudio.Effects
{
    internal class AttenuationEffect(ICoreClientAPI capi) : IEffect(capi)
    {
        public override void Apply(ref VoiceChatAudioData audioData)
        {
            Vec3d playerLocation = _capi.World.Player.Entity.Pos.XYZ;
            playerLocation = new(512000, 4, 512000);

            double distance = (audioData.Origin - playerLocation).Length();

            if (distance > 50.0)
            {
                audioData.Buffer = [];
                return;
            }

            double volumePercent = Math.Pow((50.0 - distance) / 50.0, 2);

            for (int i = 0; i < audioData.Buffer.Length; i += 2)
            {
                short monoSample = ReadSample16(audioData.Buffer, i);

                short scaledShort = Clamp16(monoSample * volumePercent);

                WriteSample16(audioData.Buffer, i, scaledShort);
            }
        }
    }
}
