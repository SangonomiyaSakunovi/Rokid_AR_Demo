using System.Runtime.CompilerServices;
using UnityEngine;

namespace SangoUtils.UnityCodeGenerator.Extensions
{
    public static class LayerExtensions
    {
        private const int LayersCount = 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask All()
        {
            var mask = 0;
            for (var i = 0; i < LayersCount; i++)
                mask |= 1 << i;

            return mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask Except(params int[] layerNumbers)
        {
            var mask = (int)All();
            foreach (var layer in layerNumbers)
                mask ^= 1 << layer;

            return mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask Except(LayerMask layerMask)
        {
            var mask1 = (int)All();
            var mask2 = (int)layerMask;
            return mask1 ^ mask2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask Where(params int[] layerNumbers)
        {
            var mask = 0;
            foreach (var layer in layerNumbers)
                mask |= 1 << layer;

            return mask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask Union(this LayerMask mask1, LayerMask mask2)
        {
            var intMask1 = (int)mask1;
            var intMask2 = (int)mask2;
            return intMask1 | intMask2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LayerMask Union(this LayerMask mask1, params int[] layerNumbers)
        {
            return Union(mask1, Where(layerNumbers));
        }
    }
}