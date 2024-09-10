using SangoUtils.AnchorEngines_Unity.Core.SubspaceOPs;
using UnityEngine;

namespace SangoUtils.AnchorEngines_Unity.Core.BaiduAIs.LogoRecognitions
{
    internal class BaiduAI_LogoRecognitionsSubspaceOP
    {
        private readonly Transform _speceRootTrans;

        internal BaiduAI_LogoRecognitionsSubspaceOP()
        {
            _speceRootTrans = BaiduAI_LogoRecognitionsEventBus.Config.SangoSubspaceRootObject.transform;
            BaiduAI_LogoRecognitionsEventBus.OnSubspaceSpinedEvent.AddListener(OnSubspaceSpined);
        }

        private void OnSubspaceSpined(object sender, BaiduAI_LogoRecognitionsMessages.SubspaceSpinEvtArgs eventArgs)
        {
            switch (eventArgs.SubspaceSpinMode)
            {
                case BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.RefocusMode
                    when eventArgs.SubspaceSpinRefocus is BaiduAI_LogoRecognitionsMessages.SubspaceSpinRefocusCode.DoneRefocus:
                    {
                        goto case BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.AdjustmentMode;
                    }
                case BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.AdjustmentMode:
                    {
                        CalcSubspaceRootTrans();
                    }
                    break;
            }

            void CalcSubspaceRootTrans()
            {
                BaiduAI_LogoRecognitionsEventBus.LogEvent.Invoke("Now is AdjustmentMode Begin: On Response.");
                Transform parentTrans = BaiduAI_LogoRecognitionsEventBus.Config.SubspaceSpinsRootObject.transform;
                Vector3 intersection = SangoSubspaceGeneratorUtils.CalcCenterPosByParent(parentTrans);

                if (_speceRootTrans.TryGetComponent<Rigidbody>(out var rigidbody))
                {
                    Vector3 intersectionToWorldPosition = parentTrans.TransformPoint(intersection);
                    rigidbody.MovePosition(intersectionToWorldPosition);
                }
                else
                {
                    _speceRootTrans.localPosition = intersection;
                }

                if (_speceRootTrans.gameObject.activeSelf is false)
                {
                    _speceRootTrans.gameObject.SetActive(true);
                }
            }
        }
    }
}