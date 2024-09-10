using System.Runtime.CompilerServices;
using UnityEngine;

namespace SangoUtils.AnchorEngines_Unity.Core.SubspaceOPs
{
    internal static class SangoSubspaceGeneratorUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector3 CalcCenterPosByParent(Transform parentTrans)
        {
            float sumX = 0;
            float sumY = 0;
            float sumZ = 0;
            for (int i = 0; i < parentTrans.childCount; i++)
            {
                sumX += parentTrans.GetChild(i).localPosition.x;
                sumY += parentTrans.GetChild(i).localPosition.y;
                sumZ += parentTrans.GetChild(i).localPosition.z;
            }
            return new Vector3(sumX / parentTrans.childCount, sumY / parentTrans.childCount, sumZ / parentTrans.childCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector3 CalcCenterPosByChild(Transform trans0, Transform trans1)
        {
            float sumX = 0;
            float sumY = 0;
            float sumZ = 0;

            sumX += trans0.localPosition.x;
            sumY += trans0.localPosition.y;
            sumZ += trans0.localPosition.z;

            sumX += trans1.localPosition.x;
            sumY += trans1.localPosition.y;
            sumZ += trans1.localPosition.z;

            return new Vector3(sumX / 2, sumY / 2, sumZ / 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector3 CalcVertexByCenter(Transform centerTrans, Transform trans0)
        {
            float centerX = centerTrans.localPosition.x;
            float centerY = centerTrans.localPosition.y;
            float centerZ = centerTrans.localPosition.z;

            float vertexX = trans0.localPosition.x;
            float vertexY = trans0.localPosition.y;
            float vertexZ = trans0.localPosition.z;

            return new Vector3(2 * centerX - vertexX, 2 * centerY - vertexY, 2 * centerZ - vertexZ);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector3 CalcVertexByCenter(Vector3 centerPos, Transform trans0)
        {
            float centerX = centerPos.x;
            float centerY = centerPos.y;
            float centerZ = centerPos.z;

            float vertexX = trans0.localPosition.x;
            float vertexY = trans0.localPosition.y;
            float vertexZ = trans0.localPosition.z;

            return new Vector3(2 * centerX - vertexX, 2 * centerY - vertexY, 2 * centerZ - vertexZ);
        }
    }
}
