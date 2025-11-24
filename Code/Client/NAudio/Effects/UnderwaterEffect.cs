using Basic_Voice_Chat.Code.Utility;
using Vintagestory.API.Client;

namespace Basic_Voice_Chat.Code.Client.NAudio.Effects
{
    internal class UnderwaterEffect(ICoreClientAPI capi) : IEffect(capi)
    {
        public override void Apply(ref VoiceChatAudioData audioData)
        {
            throw new System.NotImplementedException();
        }
    }
}
