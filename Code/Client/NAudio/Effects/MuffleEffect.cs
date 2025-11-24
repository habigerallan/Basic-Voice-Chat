using Basic_Voice_Chat.Code.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Basic_Voice_Chat.Code.Client.NAudio.Effects
{
    internal class MuffleEffect(ICoreClientAPI capi) : IEffect(capi)
    {
        private Dictionary<BlockPos, int>? _checkedPositions;

        public override void Apply(ref VoiceChatAudioData audioData)
        {
            _checkedPositions = [];

            Vec3d playerLocation = _capi.World.Player.Entity.Pos.XYZ;
            playerLocation = new(512000, 4, 512000);

            double distance = (audioData.Origin - playerLocation).Length();

            BlockSelection? blockSelection = null;
            EntitySelection? entitySelection = null;
            _capi.World.RayTraceForSelection(playerLocation, audioData.Origin, ref blockSelection, ref entitySelection, IgnoreCheckedBlocks, IgnoreAllEntities);

            while (blockSelection != null)
            {
                blockSelection = null;
                entitySelection = null;
                _capi.World.RayTraceForSelection(playerLocation, audioData.Origin, ref blockSelection, ref entitySelection, IgnoreCheckedBlocks, IgnoreAllEntities);
            }

            if (_checkedPositions.Count == 0)
            {
                return;
            } 

            int nonAirBlocks = _checkedPositions.Values.Count(value => value != 0);
            double mufflingPercent = Math.Pow(nonAirBlocks / _checkedPositions.Count, 2);

            short prevSample = 0;
            for (int i = 0; i < audioData.Buffer.Length; i += 2)
            {
                short monoSample = ReadSample16(audioData.Buffer, i);

                double filtered = (1.0 - mufflingPercent) * monoSample + mufflingPercent * prevSample;

                short filteredShort = Clamp16(filtered);
                prevSample = filteredShort;

                WriteSample16(audioData.Buffer, i, filteredShort);
            }
        }

        private bool IgnoreAllEntities(Entity entity)
        {
            return false;
        }

        private bool IgnoreCheckedBlocks(BlockPos pos, Block block)
        {
            if (_checkedPositions?.ContainsKey(pos) == true)
            {
                return false;
            }

            for (int x=-1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    for (int z = -1; z < 2; z++)
                    {
                        BlockPos neighborPos = pos.AddCopy(x, y, z);
                        Block neighborBlock = _capi.World.BlockAccessor.GetBlock(neighborPos);
                        _checkedPositions?.Add(neighborPos, neighborBlock.Id);
                    }
                }
            }

            return true;
        }
    }
}
