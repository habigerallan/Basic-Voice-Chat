using Basic_Voice_Chat.Code.Server;
using Basic_Voice_Chat.Code.Utility;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.Common;

namespace Basic_Voice_Chat.Code.Client.NAudio
{
    internal class PlaybackDevice : IDisposable
    {
        private readonly ICoreClientAPI _capi;
        private readonly WaveOutEvent _waveOut;
        private readonly BufferedWaveProvider _bufferedProvider;
        private readonly EffectChain _effectChain;
        private readonly WaveFormat _playbackFormat;

        private int _numberOfChannels;

        public PlaybackDevice(ICoreClientAPI capi)
        {
            _capi = capi;

            MMDeviceEnumerator enumerator = new();
            MMDevice defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            _numberOfChannels = defaultDevice.AudioClient.MixFormat.Channels;
            _playbackFormat = new(44100, 16, _numberOfChannels);

            _effectChain = new EffectChain(_capi);

            _bufferedProvider = new BufferedWaveProvider(_playbackFormat);

            _waveOut = new WaveOutEvent();
            _waveOut.Init(_bufferedProvider);
        }

        public void PlaybackAudio(ref VoiceChatAudioData audioData)
        {
            if (audioData.Buffer.Length == 0)
            {
                return;
            }

            audioData.NumberOfChannels = _numberOfChannels;
            _effectChain.ApplyEffects(ref audioData);

            if (audioData.Buffer.Length == 0)
            {
                return;
            }

            _bufferedProvider.AddSamples(audioData.Buffer, 0, audioData.Buffer.Length);

            if (_waveOut.PlaybackState != PlaybackState.Playing)
            {
                _waveOut.Play();
            }
        }

        public void Dispose()
        {
            _waveOut.Stop();
            _waveOut.Dispose();
        }
    }
}
