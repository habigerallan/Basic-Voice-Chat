using Basic_Voice_Chat.Code.Server;
using NAudio.Dsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Basic_Voice_Chat.Code.Client.NAudio.Effects
{
    internal class MuffleEffect(ICoreClientAPI capi) : IEffect(capi)
    {
        static bool IgnoreAllEntities(Entity _)
        {
            return false;
        }

        public override AudioData Apply(AudioData audioData)
        {
            Vec3d playerLocation = _capi.World.Player.Entity.Pos.XYZ;

            playerLocation = new(512000, 4, 512000);

            List<Vec3d> locations = [];

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        locations.Add(playerLocation + new Vec3d(dx, dy, dz));
                    }
                }
            }

            BlockSelection? blockSelection;
            EntitySelection? entitySelection;

            int numHit = 0;
            foreach (Vec3d location in locations)
            {
                blockSelection = null;
                entitySelection = null;

                _capi.World.RayTraceForSelection(audioData.Origin, location, ref blockSelection, ref entitySelection, efilter: IgnoreAllEntities);
                if (blockSelection != null)
                {
                    numHit++;
                }
            }

            if (numHit == 0)
            {
                return audioData;
            }

            double muffleStrength = (double)numHit / locations.Count;

            _capi.Logger.Debug($" Number of blocks hit: {numHit}, muffleStrength={muffleStrength}");

            byte[] buffer = audioData.Buffer;
            if (buffer == null || buffer.Length < 4)
            {
                return audioData;
            }

            int totalSamples = buffer.Length / 2; // 16-bit = 2 bytes per sample value (per channel)
            int frames = totalSamples / 2;

            // Convert to float samples (-1..1)
            float[] samples = new float[totalSamples];

            int sampleIndex = 0;
            for (int i = 0; i < buffer.Length; i += 2)
            {
                short s = (short)(buffer[i] | (buffer[i + 1] << 8));
                samples[sampleIndex++] = s / 32768f;
            }

            // Pick a cutoff frequency based on how strong the muffle is
            // More blocks -> lower cutoff
            float minCutoff = 500f;   // very muffled
            float maxCutoff = 4000f;  // almost normal speech
            float cutoff = maxCutoff - (float)muffleStrength * (maxCutoff - minCutoff);

            var lpFilter = BiQuadFilter.LowPassFilter(44100, cutoff, 1.0f);

            // Apply filter frame-by-frame
            for (int frame = 0; frame < frames; frame++)
            {
                for (int ch = 0; ch < 2; ch++)
                {
                    int idx = frame * 2 + ch;
                    samples[idx] = lpFilter.Transform(samples[idx]);
                }
            }

            // Optional global attenuation so muffled voices are softer
            float volumeScale = 1f - 0.4f * (float)muffleStrength;

            // Back to 16-bit PCM
            sampleIndex = 0;
            for (int i = 0; i < buffer.Length; i += 2)
            {
                float v = samples[sampleIndex++] * volumeScale;
                if (v > 1f) v = 1f;
                if (v < -1f) v = -1f;

                short s = (short)(v * 32767f);
                buffer[i] = (byte)(s & 0xFF);
                buffer[i + 1] = (byte)((s >> 8) & 0xFF);
            }

            return audioData;
        }
    }
}
