using UnityEngine;
using UnityEngine.Events;

namespace SangoUtils.HTCViveTrackerHelpers
{
    public class TrackerCoordinateDetector : MonoBehaviour
    {
        private readonly Vector3 ORIGIN_POS = Vector3.zero;
        private readonly Quaternion ORIGIN_ROT = Quaternion.identity;
        private readonly float ANGLE_2PI = 360f;

        [SerializeField] private bool _isTrackerCoordinateDetectToggle = false;

        [SerializeField] private float _originRange = 1f;
        [SerializeField] private float _originPrecisionPos = 0.01f;
        [SerializeField] private float _originPrecisionRotEular = 10f;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _audioClipIdle;
        [SerializeField] private AudioClip _audioClipPosDetecting;
        [SerializeField] private AudioClip _audioClipRotDetecting;
        [SerializeField] private AudioClip _audioClipDetected;

        [SerializeField] private float _audioVolumMin = 0.1f;
        [SerializeField] private float _audioVolumMax = 1f;
        [SerializeField] private float _audioVolumDefault = 0.5f;
        [SerializeField] private float _audioPitchMin = 1f;
        [SerializeField] private float _audioPitchMax = 3f;
        [SerializeField] private float _audioPitchDefault = 1f;

        [SerializeField] private bool _isCustomDetect = false;
        [SerializeField] private UnityEvent<float, float> _onDetectedEvt = new();

        private float _normsPos = float.MaxValue;
        private float _normsRot = float.MaxValue;

        private DetectMachine _detectMachine;

        private void Awake()
        {
            if (_isTrackerCoordinateDetectToggle && !_isCustomDetect)
            {
                _detectMachine = new DetectMachine(this);
                _detectMachine.Enter(typeof(DetectIdleState));
            }
        }

        private void FixedUpdate()
        {
            if (_isTrackerCoordinateDetectToggle)
            {
                UpdateDetectRes();

                if (!_isCustomDetect)
                    _detectMachine?.Update();
            }
        }

        private void UpdateDetectRes()
        {
            _normsPos = Vector3.Distance(transform.localPosition, ORIGIN_POS);
            _normsRot = Quaternion.Angle(transform.localRotation, ORIGIN_ROT);
            _onDetectedEvt?.Invoke(_normsPos, _normsRot);
        }

        private void PlayAudio(AudioClip clip)
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();

            _audioSource.clip = clip;
            _audioSource.volume = _audioVolumDefault;
            _audioSource.pitch = _audioPitchDefault;
            _audioSource.loop = true;

            _audioSource.Play();
        }

        #region DetectMachine
        private class DetectMachine : StateMachine
        {
            public DetectMachine(TrackerCoordinateDetector detector)
            {
                Add(new DetectIdleState(this, detector));
                Add(new DetectPosState(this, detector));
                Add(new DetectRotState(this, detector));
                Add(new DetectDoneState(this, detector));
            }
        }

        private class DetectIdleState : IState
        {
            private readonly TrackerCoordinateDetector Detector;

            public DetectIdleState(StateMachine machine, TrackerCoordinateDetector detector)
            {
                Machine = machine;
                Detector = detector;
            }

            public override void Enter()
            {
                Detector.PlayAudio(Detector._audioClipIdle);
                Detector._audioSource.volume = Detector._audioVolumMin;
            }

            public override void Update()
            {
                if (Detector._normsPos < Detector._originRange)
                    Machine.Enter(typeof(DetectPosState));
            }

            public override void Exit() { }
        }

        private class DetectPosState : IState
        {
            private readonly TrackerCoordinateDetector Detector;

            public DetectPosState(StateMachine machine, TrackerCoordinateDetector detector)
            {
                Machine = machine;
                Detector = detector;
            }

            public override void Enter()
            {
                Detector.PlayAudio(Detector._audioClipPosDetecting);
            }

            public override void Update()
            {
                float detectPosCharge = 1 - Detector._normsPos / Detector._originRange;
                Detector._audioSource.volume = Detector._audioVolumMin + detectPosCharge * (Detector._audioVolumMax - Detector._audioVolumMin);

                if (Detector._normsPos < Detector._originPrecisionPos)
                    Machine.Enter(typeof(DetectRotState));

                if (Detector._normsPos > Detector._originRange)
                    Machine.Enter(typeof(DetectIdleState));
            }

            public override void Exit() { }
        }

        private class DetectRotState : IState
        {
            private readonly TrackerCoordinateDetector Detector;

            public DetectRotState(StateMachine machine, TrackerCoordinateDetector detector)
            {
                Machine = machine;
                Detector = detector;
            }

            public override void Enter()
            {
                Detector.PlayAudio(Detector._audioClipRotDetecting);
            }

            public override void Update()
            {
                float detectRotCharge = 1 - Detector._normsRot / Detector.ANGLE_2PI;
                Detector._audioSource.pitch = Detector._audioPitchMin + detectRotCharge * (Detector._audioPitchMax - Detector._audioPitchMin);

                if (Detector._normsRot < Detector._originPrecisionRotEular)
                    Machine.Enter(typeof(DetectDoneState));

                if (Detector._normsPos > Detector._originPrecisionPos)
                    Machine.Enter(typeof(DetectPosState));
            }

            public override void Exit() { }
        }

        private class DetectDoneState : IState
        {
            private readonly TrackerCoordinateDetector Detector;

            public DetectDoneState(StateMachine machine, TrackerCoordinateDetector detector)
            {
                Machine = machine;
                Detector = detector;
            }

            public override void Enter()
            {
                Detector.PlayAudio(Detector._audioClipDetected);
            }

            public override void Update()
            {
                if (Detector._normsRot > Detector._originPrecisionRotEular)
                    Machine.Enter(typeof(DetectRotState));

                if (Detector._normsPos > Detector._originPrecisionPos)
                    Machine.Enter(typeof(DetectPosState));
            }

            public override void Exit() { }
        }
        #endregion
    }
}
