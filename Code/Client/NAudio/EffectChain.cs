using Basic_Voice_Chat.Code.Client.NAudio.Effects;
using Basic_Voice_Chat.Code.Server;
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

            effects.Add(new MuffleEffect(_capi));
        }

        public AudioData ApplyEffects(AudioData audioData)
        {
            AudioData finalAudioData = audioData;

            foreach (IEffect effect in effects)
            {
                finalAudioData = effect.Apply(finalAudioData);
            }

            return finalAudioData;
        }
    }
}