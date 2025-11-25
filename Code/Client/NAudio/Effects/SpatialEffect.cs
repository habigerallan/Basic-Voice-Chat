using Basic_Voice_Chat.Code.Server;
using Basic_Voice_Chat.Code.Utility;
using System;
using System.Threading.Channels;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
namespace Basic_Voice_Chat.Code.Client.NAudio.Effects
{
    internal class SpatialEffect(ICoreClientAPI capi) : IEffect(capi)
    {
        public override void Apply(ref VoiceChatAudioData audioData)
        {
            int baseLength = audioData.Buffer.Length;
            byte[] newBytes = new byte[baseLength * audioData.NumberOfChannels];

            Vec3d playerLocation = _capi.World.Player.Entity.Pos.XYZ;
            playerLocation.Y += 2;
            playerLocation = new(512000, 4, 512000);

            Vec3d soundDirection = (playerLocation - audioData.Origin).Normalize();
            float playerYaw = _capi.World.Player.Entity.BodyYaw;

            double playerXDirection = Math.Sin(playerYaw);
            double playerYDirection = Math.Sin(_capi.World.Player.CameraPitch);
            double playerZDirection = Math.Cos(playerYaw);

            Vec3d playerFacingDirection = new(playerXDirection, playerYDirection, playerZDirection);

            double horizontalDifference = playerFacingDirection.X * soundDirection.Z - playerFacingDirection.Z * soundDirection.X;
            horizontalDifference = GameMath.Clamp(horizontalDifference, -1, 1);

            double right = (horizontalDifference + 1.0) * 0.5;
            double left = 1.0 - right;

            double depthDifference = soundDirection.X * playerFacingDirection.X + soundDirection.Z * playerFacingDirection.Z;
            depthDifference = GameMath.Clamp(depthDifference, -1, 1);

            double forward = (depthDifference + 1.0) * 0.5;
            double backward = 1.0 - forward;

            _capi.Logger.Debug($"\nLeft: {left}\nRight {right}\nForward: {forward}\nBackward {backward}");

            // 2.0 | L, R
            if (audioData.NumberOfChannels == 2)
            {
                // i = index in mono
                // o = index in output
                for (int i = 0, o = 0; i < baseLength; i += 2, o += audioData.NumberOfChannels * 2)
                {
                    short monoSample = ReadSample16(audioData.Buffer, i);

                    short leftShort = Clamp16(monoSample * left);
                    short rightShort = Clamp16(monoSample * right);

                    // L
                    WriteSample16(newBytes, o, leftShort);
                    // R
                    WriteSample16(newBytes, o + 2, rightShort);
                }

                audioData.Buffer = newBytes;
            }

            // 2.1 | L, R, LFE
            else if (audioData.NumberOfChannels == 3)
            {
                // i = index in mono
                // o = index in output
                for (int i = 0, o = 0; i < baseLength; i += 2, o += audioData.NumberOfChannels * 2)
                {
                    short monoSample = ReadSample16(audioData.Buffer, i);

                    short leftShort = Clamp16(monoSample * left);
                    short rightShort = Clamp16(monoSample * right);

                    // L
                    WriteSample16(newBytes, o, leftShort);
                    // R
                    WriteSample16(newBytes, o + 2, rightShort);
                    // LFE
                    WriteSample16(newBytes, o + 4, 0);
                }

                audioData.Buffer = newBytes;
            }

            // 5.0 | FL, FR, C, SL, SR
            else if (audioData.NumberOfChannels == 5)
            {
                double offCenter = Math.Abs(left - right);
                double centered = 1 - offCenter;

                double fl = forward * left * offCenter;
                double fr = forward * right * offCenter;

                double c = forward * centered;

                double sl = backward * left;
                double sr = backward * right;

                // i = index in mono
                // o = index in output
                for (int i = 0, o = 0; i < baseLength; i += 2, o += 10)
                {
                    short monoSample = ReadSample16(audioData.Buffer, i);

                    short flShort = Clamp16(monoSample * fl);
                    short frShort = Clamp16(monoSample * fr);
                    short cShort = Clamp16(monoSample * c);
                    short slShort = Clamp16(monoSample * sl);
                    short srShort = Clamp16(monoSample * sr);

                    // FL
                    WriteSample16(newBytes, o, flShort);
                    // FR
                    WriteSample16(newBytes, o + 2, frShort);
                    // C
                    WriteSample16(newBytes, o + 4, cShort);
                    // SL
                    WriteSample16(newBytes, o + 6, slShort);
                    // SR
                    WriteSample16(newBytes, o + 8, srShort);
                }

                audioData.Buffer = newBytes;
            }

            // 5.1 | FL, FR, C, SL, SR, LFE
            else if (audioData.NumberOfChannels == 6)
            {
                double offCenter = Math.Abs(left - right);
                double centered = 1 - offCenter;

                double fl = forward * left * offCenter;
                double fr = forward * right * offCenter;

                double c = forward * centered;

                double sl = backward * left;
                double sr = backward * right;

                // i = index in mono
                // o = index in output
                for (int i = 0, o = 0; i < baseLength; i += 2, o += audioData.NumberOfChannels * 2)
                {
                    short monoSample = ReadSample16(audioData.Buffer, i);

                    short flShort = Clamp16(monoSample * fl);
                    short frShort = Clamp16(monoSample * fr);
                    short cShort = Clamp16(monoSample * c);
                    short slShort = Clamp16(monoSample * sl);
                    short srShort = Clamp16(monoSample * sr);

                    // FL
                    WriteSample16(newBytes, o, flShort);
                    // FR
                    WriteSample16(newBytes, o + 2, frShort);
                    // C
                    WriteSample16(newBytes, o + 4, cShort);
                    // SL
                    WriteSample16(newBytes, o + 6, slShort);
                    // SR
                    WriteSample16(newBytes, o + 8, srShort);
                    // LFE
                    WriteSample16(newBytes, o + 10, 0);
                }

                audioData.Buffer = newBytes;

            }

            // 7.0 | FL, FR, C, SL, SR, RL, RR
            else if (audioData.NumberOfChannels == 7)
            {
                double offCenterLR = Math.Abs(left - right);
                double centeredLR = 1 - offCenterLR;

                double offCenterFB = Math.Abs(forward - backward);
                double centeredFB = 1 - offCenterFB;

                double c = forward * centeredLR;

                double scaledLeft = (1.0 - c) * left;
                double scaledRight = (1.0 - c) * right;

                double fl = scaledLeft * forward * offCenterFB;
                double sl = scaledLeft * centeredFB;
                double rl = scaledLeft * backward * offCenterFB; 

                double fr = scaledRight * forward * offCenterFB;
                double sr = scaledRight * centeredFB;
                double rr = scaledRight * backward * offCenterFB;

                // i = index in mono
                // o = index in output
                for (int i = 0, o = 0; i < baseLength; i += 2, o += audioData.NumberOfChannels * 2)
                {
                    short monoSample = ReadSample16(audioData.Buffer, i);

                    short flShort = Clamp16(monoSample * fl);
                    short frShort = Clamp16(monoSample * fr);
                    short cShort = Clamp16(monoSample * c);
                    short slShort = Clamp16(monoSample * sl);
                    short srShort = Clamp16(monoSample * sr);
                    short rlShort = Clamp16(monoSample * rl);
                    short rrShort = Clamp16(monoSample * rr);

                    // FL
                    WriteSample16(newBytes, o, flShort);
                    // FR
                    WriteSample16(newBytes, o + 2, frShort);
                    // C
                    WriteSample16(newBytes, o + 4, cShort);
                    // SL
                    WriteSample16(newBytes, o + 6, slShort);
                    // SR
                    WriteSample16(newBytes, o + 8, srShort);
                    // RL
                    WriteSample16(newBytes, o + 10, rlShort);
                    // RR
                    WriteSample16(newBytes, o + 12, rrShort);
                }

                audioData.Buffer = newBytes;

            }

            // 7.1 | FL, FR, C, SL, SR, RL, RR, LFE
            else if (audioData.NumberOfChannels == 8)
            {
                double offCenterLR = Math.Abs(left - right);
                double centeredLR = 1 - offCenterLR;

                double offCenterFB = Math.Abs(forward - backward);
                double centeredFB = 1 - offCenterFB;

                double c = forward * centeredLR;

                double scaledLeft = (1.0 - c) * left;
                double scaledRight = (1.0 - c) * right;

                double fl = scaledLeft * forward * offCenterFB;
                double sl = scaledLeft * centeredFB;
                double rl = scaledLeft * backward * offCenterFB;

                double fr = scaledRight * forward * offCenterFB;
                double sr = scaledRight * centeredFB;
                double rr = scaledRight * backward * offCenterFB;

                // i = index in mono
                // o = index in output
                for (int i = 0, o = 0; i < baseLength; i += 2, o += audioData.NumberOfChannels * 2)
                {
                    short monoSample = ReadSample16(audioData.Buffer, i);

                    short flShort = Clamp16(monoSample * fl);
                    short frShort = Clamp16(monoSample * fr);
                    short cShort = Clamp16(monoSample * c);
                    short slShort = Clamp16(monoSample * sl);
                    short srShort = Clamp16(monoSample * sr);
                    short rlShort = Clamp16(monoSample * rl);
                    short rrShort = Clamp16(monoSample * rr);

                    // FL
                    WriteSample16(newBytes, o, flShort);
                    // FR
                    WriteSample16(newBytes, o + 2, frShort);
                    // C
                    WriteSample16(newBytes, o + 4, cShort);
                    // SL
                    WriteSample16(newBytes, o + 6, slShort);
                    // SR
                    WriteSample16(newBytes, o + 8, srShort);
                    // RL
                    WriteSample16(newBytes, o + 10, rlShort);
                    // RR
                    WriteSample16(newBytes, o + 12, rrShort);
                    // LFE
                    WriteSample16(newBytes, o + 14, 0);
                }

                audioData.Buffer = newBytes;
            }

            // Unsupported, writes original to all channels
            else
            {
                // i = index in mono
                // o = index in output
                for (int i = 0, o = 0; i < baseLength; i += 2, o += audioData.NumberOfChannels * 2)
                {
                    short monoSample = ReadSample16(audioData.Buffer, i);

                    for (int ch = 0; ch < audioData.NumberOfChannels; ch++)
                    {
                        WriteSample16(newBytes, o + ch * 2, monoSample);
                    }
                }

                audioData.Buffer = newBytes;
            }

            return;
        }
    }
}
