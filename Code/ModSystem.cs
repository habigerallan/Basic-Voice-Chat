using Basic_Voice_Chat.Code.Client;
using Basic_Voice_Chat.Code.Config;
using Basic_Voice_Chat.Code.Server;
using Basic_Voice_Chat.Code.Utility;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Basic_Voice_Chat.Code
{
    public class Basic_Voice_ChatModSystem : ModSystem
    {
        private ClientManager? clientManager;

        private void TryToLoadConfig(ICoreAPI api)
        {
            try
            {
                BasicVoiceChatConfig _config = api.LoadModConfig<BasicVoiceChatConfig>("basicvoicechatconfig.json");
                _config ??= new();

                api.StoreModConfig(_config, "basicvoicechatconfig.json");
            }
            catch (Exception e)
            {
                Mod.Logger.Error("[basicvoicechat] Could not load config! Loading default settings instead.");
                Mod.Logger.Error(e);

                BasicVoiceChatConfig _config = new();
                api.StoreModConfig(_config, "basicvoicechatconfig.json");
            }
        }

        public override void Start(ICoreAPI api)
        {
            TryToLoadConfig(api);

            api.Network.RegisterChannel("basicvoicechat:network-channel")
                .RegisterMessageType<VoiceChatAudioData>();
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            BasicVoiceChatConfig _config = api.LoadModConfig<BasicVoiceChatConfig>("basicvoicechatconfig.json");

            if (_config.VoiceChatEnabled == false)
            {
                return;
            }

            _ = new ServerManager(api);
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            BasicVoiceChatConfig config = api.LoadModConfig<BasicVoiceChatConfig>("basicvoicechatconfig.json");

            if (config.VoiceChatEnabled == false)
            {
                return;
            }

            clientManager = new ClientManager(api, config);
        }

        public override void Dispose()
        {
            base.Dispose();
            clientManager?.Dispose();
        }
    }
}
