using Basic_Voice_Chat.Code.Server;
using NAudio.Wave;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Basic_Voice_Chat.Code.Client.NAudio
{
    internal class PlaybackDevice : IDisposable
    {
        private readonly ICoreClientAPI _capi;
        private readonly WaveOutEvent _waveOut;
        private readonly BufferedWaveProvider _bufferedProvider;
        //private readonly EffectChain _effectChain;

        private static readonly WaveFormat PlaybackFormat = new();

        public PlaybackDevice(ICoreClientAPI capi)
        {
            _capi = capi;

            //_effectChain = new EffectChain(_capi);
            _bufferedProvider = new BufferedWaveProvider(PlaybackFormat);

            _waveOut = new WaveOutEvent();
            _waveOut.Init(_bufferedProvider);
        }

        public void PlaybackAudio(AudioData audioData)
        {
            if (audioData.Buffer.Length == 0)
            {
                return;
            }

            Vec3d playerLocation = _capi.World.Player.Entity.Pos.XYZ;

            double distance = (audioData.Origin - playerLocation).Length();

            if (distance > 50.0)
            {
                return;
            }

            _waveOut.Volume = (float)Math.Pow((50.0 - distance) / 50.0, 2);

            //audioBytes = _effectChain.ApplyEffects(audioBytes);
            _bufferedProvider.AddSamples(audioData.Buffer, 0, audioData.Buffer.Length);

            if (_waveOut.PlaybackState != PlaybackState.Playing)
            {
                _waveOut.Play();
            }
        }

        public void Dispose()
        {
            _waveOut?.Stop();
            _waveOut?.Dispose();
        }
    }
}
