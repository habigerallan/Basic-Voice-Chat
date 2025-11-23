using Basic_Voice_Chat.Code.Server;
using Basic_Voice_Chat.Code.Utility;
using System;
using Vintagestory.API.Client;

namespace Basic_Voice_Chat.Code.Client.NAudio.Effects
{
    internal class ReverbEffect(ICoreClientAPI capi) : IEffect(capi)
    {
        public override void Apply(ref VoiceChatAudioData audioData)
        {
            throw new NotImplementedException();
        }
    }
}
