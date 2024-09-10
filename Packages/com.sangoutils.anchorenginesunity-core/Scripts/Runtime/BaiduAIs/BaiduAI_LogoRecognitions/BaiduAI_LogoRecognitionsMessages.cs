using System;
using UnityEngine;

namespace SangoUtils.AnchorEngines_Unity.Core.BaiduAIs.LogoRecognitions
{
    internal partial class BaiduAI_LogoRecognitionsMessages
    {
        internal class UploadEvtArgs : EventArgs
        {
            internal UploadEvtArgs(string responseName)
            {
                ResponseName = responseName;
            }

            internal string ResponseName { get; }
        }

        internal class SubspaceSpinAdjustmentEvtArgs : EventArgs
        {
            internal SubspaceSpinAdjustmentEvtArgs(GameObject spinObject)
            {
                SpinObject = spinObject;
            }

            internal GameObject SpinObject { get; }
        }

        internal enum SubspaceSpinModeCode
        {
            WaitingMode,
            RefocusMode,
            ManualMode,
            AdjustmentMode
        }
        internal enum SubspaceSpinRefocusCode
        {            
            NoRefocus,
            StartRefocus,
            UpdateRefocus,
            DoneRefocus,
            CancelRefocus,
        }

        internal class SubspaceSpinEvtArgs : EventArgs
        {           
            public SubspaceSpinEvtArgs(SubspaceSpinModeCode modeCode, SubspaceSpinRefocusCode refocusCode)
            {
                SubspaceSpinMode = modeCode;
                SubspaceSpinRefocus = refocusCode;
            }

            internal SubspaceSpinModeCode SubspaceSpinMode { get; }
            internal SubspaceSpinRefocusCode SubspaceSpinRefocus { get; }
        }
    }
}
