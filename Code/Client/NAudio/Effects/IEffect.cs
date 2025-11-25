using Basic_Voice_Chat.Code.Server;
using Basic_Voice_Chat.Code.Utility;
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

        protected static short Clamp16(double value)
        {
            double clamped = Math.Clamp(value, short.MinValue, short.MaxValue);
            return (short)Math.Round(clamped);
        }

        protected static short ReadSample16(byte[] buffer, int index)
        {
            return (short)(buffer[index] | (buffer[index + 1] << 8));
        }

        protected static void WriteSample16(byte[] buffer, int index, short value)
        {
            buffer[index] = (byte)(value & 0xFF);
            buffer[index + 1] = (byte)((value >> 8) & 0xFF);
        }

        public abstract void Apply(ref VoiceChatAudioData audioData);
    }
}
