using Basic_Voice_Chat.Code.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace Basic_Voice_Chat.Code.Client.NAudio.Effects
{
    internal abstract class IEffect(ICoreClientAPI capi)
    {
        protected readonly ICoreClientAPI _capi = capi;

        public abstract AudioData Apply(AudioData audioData);
    }
}
