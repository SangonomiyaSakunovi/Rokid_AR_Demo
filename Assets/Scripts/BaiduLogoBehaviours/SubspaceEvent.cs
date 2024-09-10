using SangoProjects.RokidARDemo.EventBus;
using SangoUtils.UnityDevelopToolKits.Timers;
using UnityEngine;

namespace SangoProjects.RokidARDemo.Events
{
    internal partial class SubspaceEvent : MonoBehaviour
    {
        [field: SerializeField] private AudioSource AudioSource_FG { get; set; }
        [field: SerializeField] private AudioSource AudioSource_BG { get; set; }

        [field: SerializeField] private AudioClip AudioClip { get; set; }
        [field: SerializeField] private float IntervalBeforeRefocusAllDone { get; set; }
        [field: SerializeField] private float IntervalBeforeGenerateRandomObj { get; set; }
        [field: SerializeField] private GameObject SubspaceSpinsRoot { get; set; }

        public void OnLogoFound()
        {
            AudioSource_FG.Stop();
            AudioSource_FG.clip = AudioClip;
            AudioSource_FG.Play();
        }

        public void OnAutoRefocusToManual()
        {
            Debug.Log("时间到了，需要手动进行调整了");
        }

        public void OnAutoRefocusDone()
        {
            Debug.Log("自动调整结束");
            TimerAsyncOperation operation = UnityTimer.WaitForSecondsRealtime(IntervalBeforeRefocusAllDone);
            operation.completed += delegate
            {
                for (int i = 0; i < SubspaceSpinsRoot.transform.childCount; i++)
                {
                    var childObj = SubspaceSpinsRoot.transform.GetChild(i).gameObject;
                    childObj.SetActive(false);
                }

                TimerAsyncOperation operation1 = UnityTimer.WaitForSecondsRealtime(IntervalBeforeGenerateRandomObj);
                operation1.completed += delegate
                {
                    var randomChildObj = SubspaceSpinsRoot.transform.GetChild(0).gameObject;
                    randomChildObj.SetActive(true);
                    SubspaceEventBus.Instance.OnRefocusAllDoneEvent?.Invoke(this, new EventMessages.SubspaceEventArgs.OnRefocusAllDoneEventArgs());
                };
            };
        }
    }
}