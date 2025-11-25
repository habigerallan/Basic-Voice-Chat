using Basic_Voice_Chat.Code.Server;
using Basic_Voice_Chat.Code.Utility;
using NAudio.Wave;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Basic_Voice_Chat.Code.Client.NAudio
{
    internal class CaptureDevice : IDisposable
    {
        private readonly ICoreClientAPI _capi;
        private readonly WaveInEvent _waveIn;
        private readonly WaveFormat _captureFormat;
        private bool _isRecording;

        public CaptureDevice(ICoreClientAPI capi)
        {
            _capi = capi;
            _captureFormat = new(44100, 16, 1);
            _isRecording = false;

            _waveIn = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = _captureFormat,
                BufferMilliseconds = 100
            };

            _waveIn.DataAvailable += OnDataAvailable;
        }

        // Freeze at beginning here
        public void StartRecording()
        {
            _isRecording = true;
            _waveIn.StartRecording();
        }

        public void StopRecording()
        {
            _isRecording = false;
            _waveIn.StopRecording();
        }

        public bool IsRecording()
        {
            return _isRecording;
        }

        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            byte[] buffer = new byte[e.BytesRecorded];
            Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesRecorded);

            Vec3d soundOrigin = _capi.World.Player.Entity.Pos.XYZ;
            soundOrigin.Y += 2;

            VoiceChatAudioData audioData = new(buffer, soundOrigin);

            _capi.Network
                .GetChannel("basicvoicechat:network-channel")
                .SendPacket(audioData);
        }

        public void Dispose()
        {
            _waveIn.Dispose();
        }
    }
}
