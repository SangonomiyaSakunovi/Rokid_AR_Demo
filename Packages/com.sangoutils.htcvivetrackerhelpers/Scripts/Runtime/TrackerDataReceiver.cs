using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace SangoUtils.HTCViveTrackerHelpers
{
    /// <summary>
    /// HTC is right hand coordinate, Unity is left hand.
    /// Pose Matrix:[-x,y,z,rx,-ry,-rz,rw]
    /// </summary>
    public class TrackerDataReceiver : MonoBehaviour
    {        
        internal enum TrackerDataIPTypeCode
        {
            AnyIP = 0, TargetIP = 1
        }
        internal enum TrackerPositionDataBinding
        {
            TrackerX = 0, TrackerY = 1, TrackerZ = 2, TrackerX_ = -1, TrackerY_ = -2, TrackerZ_ = -3
        }
        internal enum TrackerRotationDataBinding
        {
            TrackerW = 3, TrackerX = 4, TrackerY = 5, TrackerZ = 6, TrackerW_ = -4, TrackerX_ = -5, TrackerY_ = -6, TrackerZ_ = -7
        }

        [SerializeField] private bool _isGetTrackerDataToggle = false;

        [SerializeField] private int _trackerDataLateFrames = 0;

        [Header("Listener")]
        [SerializeField] private TrackerDataIPTypeCode _trackerDataIpType = TrackerDataIPTypeCode.AnyIP;
        [SerializeField] private string _ipAddress = "";
        [SerializeField] private int _port = 8051;

        [SerializeField] private TrackerPositionDataBinding _unityPosX = TrackerPositionDataBinding.TrackerX;
        [SerializeField] private TrackerPositionDataBinding _unityPosY = TrackerPositionDataBinding.TrackerY;
        [SerializeField] private TrackerPositionDataBinding _unityPosZ = TrackerPositionDataBinding.TrackerZ;

        [SerializeField] private TrackerRotationDataBinding _unityRotX = TrackerRotationDataBinding.TrackerX;
        [SerializeField] private TrackerRotationDataBinding _unityRotY = TrackerRotationDataBinding.TrackerY;
        [SerializeField] private TrackerRotationDataBinding _unityRotZ = TrackerRotationDataBinding.TrackerZ;
        [SerializeField] private TrackerRotationDataBinding _unityRotW = TrackerRotationDataBinding.TrackerW;

        [SerializeField] private GameObject _trackerObject = null;

        private Thread _receiveThread;
        private UdpClient _client;
        private Double[] _float_array;
        private (Vector3, Quaternion) _trackerPose;

        private Queue<(Vector3, Quaternion)> _trackerDataQueue = new Queue<(Vector3, Quaternion)>();
        /// <summary>
        /// If this receiver has new data.
        /// </summary>
        public bool IsRunning { get; private set; } = false;
        /// <summary>
        /// Share the data of Tracker position and rotation, which coordinate has been convert.
        /// </summary>
        public Action<Vector3, Quaternion> OnTrackerDataUpdate;

        private void Awake()
        {
            if (_isGetTrackerDataToggle)
                _float_array = new Double[7];
        }

        private void Start()
        {
            if (_isGetTrackerDataToggle)
            {
                switch (_trackerDataIpType)
                {
                    case TrackerDataIPTypeCode.AnyIP:
                        _receiveThread = new Thread(new ThreadStart(ReceiveAnyData));
                        break;
                    case TrackerDataIPTypeCode.TargetIP:
                        _receiveThread = new Thread(new ThreadStart(ReceiveIpAddressData));
                        break;
                }

                _receiveThread.IsBackground = true;
                _receiveThread.Start();
                IsRunning = true;
            }
        }

        private void FixedUpdate()
        {
            if (!IsRunning)
                return;

            var position = new Vector3(GetData((int)_unityPosX), GetData((int)_unityPosY), GetData((int)_unityPosZ));
            var rotation = new Quaternion(GetData((int)_unityRotX), GetData((int)_unityRotY), GetData((int)_unityRotZ), GetData((int)_unityRotW));
            _trackerDataQueue.Enqueue((position, rotation));

            if (_trackerDataQueue.Count > _trackerDataLateFrames)
                _trackerPose = _trackerDataQueue.Dequeue();

            if (_trackerObject != null)
                _trackerObject.transform.SetLocalPositionAndRotation(_trackerPose.Item1, _trackerPose.Item2);

            OnTrackerDataUpdate?.Invoke(_trackerPose.Item1, _trackerPose.Item2);
        }

        private void OnApplicationQuit()
        {
            IsRunning = false;
            _receiveThread?.Abort();
            _client?.Close();
        }

        /// <summary>
        /// Receive data from any ip.
        /// </summary>
        private void ReceiveAnyData()
        {
            _client = new UdpClient(_port);
            Debug.Log("[Sango] Starting receive from IPAddress Any.");
            while (true)
            {
                try
                {
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = _client.Receive(ref endPoint);

                    for (int i = 0; i < _float_array.Length; i++)
                        _float_array[i] = BitConverter.ToDouble(data, i * 8);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Receive data from target ip.
        /// </summary>
        private void ReceiveIpAddressData()
        {
            _client = new UdpClient(_port);
            Debug.Log($"[Sango] Starting receive from IPAddress {_ipAddress}.");
            while (true)
            {
                try
                {
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), 0);
                    byte[] data = _client.Receive(ref endPoint);

                    for (int i = 0; i < _float_array.Length; i++)
                        _float_array[i] = BitConverter.ToDouble(data, i * 8);
                }
                catch (Exception err)
                {
                    Debug.LogWarning(err.ToString());
                }
            }
        }

        /// <summary>
        /// Read the index, we define that such as -1 can use ~ to read as 0, -2 read as 1
        /// But this also means the direction of coordinate, so to use - to change.
        /// </summary>
        private float GetData(in int index)
        {
            if (index < 0)
            {
                return -(float)_float_array[~index];
            }
            else
            {
                return (float)_float_array[index];
            }
        }

        /// <summary>
        /// You can reset late frame any time.
        /// </summary>
        internal void ResetLateFrames()
        {
            Debug.Log("[Sango] You have reset the LateFrames.");
            _trackerDataQueue.Clear();
        }
    }
}