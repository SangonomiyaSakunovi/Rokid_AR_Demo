using System.Collections;
using UnityEngine;

namespace SangoUtils.AnchorEngines_Unity.Core.Utils
{
    internal static class UnityTimer
    {
        public static TimerAsyncOperation WaitForSecondsRealtime(float duration)
        {
            TimerAsyncOperation operation = new();
            WaitYield(operation).Start();
            return operation;

            IEnumerator WaitYield(TimerAsyncOperation operation)
            {
                yield return new WaitForSecondsRealtime(duration);

                if (!operation.isDone)
                {
                    operation.isDone = true;
                    operation.Call();
                }
            }
        }

        public static TimerAsyncOperation RepeatWaitForSecondsRealtime(float duration)
        {
            TimerAsyncOperation operation = new();
            RepeatWaitYield(operation).Start();
            return operation;

            IEnumerator RepeatWaitYield(TimerAsyncOperation operation)
            {
                while (true) 
                {
                    yield return new WaitForSecondsRealtime(duration);

                    if (!operation.isDone)
                    {
                        operation.Call();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
