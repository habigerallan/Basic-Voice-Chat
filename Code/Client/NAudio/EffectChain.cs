using Basic_Voice_Chat.Code.Client.NAudio.Effects;
using Basic_Voice_Chat.Code.Server;
using Basic_Voice_Chat.Code.Utility;
using HarmonyLib;
using System.Collections.Generic;
using Vintagestory.API.Client;

namespace Basic_Voice_Chat.Code.Client.NAudio
{
    internal class EffectChain
    {
        private readonly ICoreClientAPI _capi;
        private readonly List<IEffect> effects;

        public EffectChain(ICoreClientAPI capi)
        {
            _capi = capi;
            effects = [];

            effects.Add(new AttenuationEffect(_capi));
            effects.Add(new MuffleEffect(_capi));
            //effects.Add(new ReverbEffect(_capi));
            //effects.Add(new DistortionEffect(_capi));
            effects.Add(new SpatialEffect(_capi));
        }

        public void ApplyEffects(ref VoiceChatAudioData audioData)
        {
            foreach (IEffect effect in effects)
            {
                if (audioData.Buffer.Length == 0)
                {
                    return;
                }

                effect.Apply(ref audioData);
            }
        }
    }
}