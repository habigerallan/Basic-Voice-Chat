using Basic_Voice_Chat.Code.Utility;
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
                .SetMessageHandler<VoiceChatAudioData>(OnPlayerTalk);
        }

        private void OnPlayerTalk(IServerPlayer talkingPlayer, VoiceChatAudioData audioData)
        {
            _sapi.Network.GetChannel("basicvoicechat:network-channel")
                .BroadcastPacket(audioData);
                //.BroadcastPacket(audioData, talkingPlayer);
        }
    }
}
