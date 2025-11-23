using Basic_Voice_Chat.Code.Client.NAudio;
using Basic_Voice_Chat.Code.Config;
using Basic_Voice_Chat.Code.Utility;
using Vintagestory.API.Client;

namespace Basic_Voice_Chat.Code.Client
{
    internal class ClientManager
    {
        private readonly ICoreClientAPI _capi;
        private readonly BasicVoiceChatConfig _config;
        private readonly CaptureDevice _captureDevice;
        private readonly PlaybackDevice _playbackDevice;

        private GlKeys _currentPushToTalkKey;

        public ClientManager(ICoreClientAPI api, BasicVoiceChatConfig config)
        {
            _capi = api;
            _config = config;

            _captureDevice = new(_capi);
            _playbackDevice = new(_capi);

            _capi.Network.GetChannel("basicvoicechat:network-channel")
                .SetMessageHandler<VoiceChatAudioData>(OnPlayerTalked);

            _currentPushToTalkKey = _config.PushToTalkKey;

            _capi.Input.RegisterHotKey("basicvoicechat:hotkey-push-to-talk", "Push-to-talk", _currentPushToTalkKey);
            _capi.Input.SetHotKeyHandler("basicvoicechat:hotkey-push-to-talk", OnHotKeyDown);

            _capi.Event.KeyDown += OnKeyDown;
            _capi.Event.KeyUp += OnKeyUp;
        }
        private void OnKeyDown(KeyEvent keyEvent)
        {
            if (_config.VoiceChatEnabled == false)
            {
                if (_captureDevice.IsRecording())
                {
                    _captureDevice.StopRecording();
                }

                return;
            }

            if (keyEvent.KeyCode == ((int)_currentPushToTalkKey))
            {
                if (!_captureDevice.IsRecording())
                {
                    _captureDevice.StartRecording();
                }
            }
        }

        private void OnKeyUp(KeyEvent keyEvent)
        {

            if (_config.VoiceChatEnabled == false)
            {
                if (_captureDevice.IsRecording())
                {
                    _captureDevice.StopRecording();
                }

                return;
            }

            if (keyEvent.KeyCode == ((int)_currentPushToTalkKey))
            {
                if (_captureDevice.IsRecording())
                {
                    _captureDevice.StopRecording();
                }
            }
        }

        private bool OnHotKeyDown(KeyCombination keyCombination)
        {
            if (keyCombination.KeyCode != (int)_currentPushToTalkKey)
            {
                _currentPushToTalkKey = (GlKeys)keyCombination.KeyCode;
                _config.PushToTalkKey = _currentPushToTalkKey;
            }

            return true;
        }

        private void OnPlayerTalked(VoiceChatAudioData audioData)
        {
            if (_config.VoiceChatEnabled == false)
            {
                return;
            }

            _playbackDevice.PlaybackAudio(ref audioData);
        }

        public void Dispose()
        {
            _captureDevice.Dispose();
            _playbackDevice.Dispose();
        }
    }
}
