using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ImmersalRESTLocalizer.Types;
using Cysharp.Threading.Tasks;
using Unity.XR.CoreUtils;
using System;
using UniRx;
using UnityEngine.XR.ARFoundation;

namespace ImmersalRESTLocalizer
{
    public class LocalizedCameraController : MonoBehaviour
    {
        [SerializeField] private ARCameraManager cameraManager;
        [SerializeField] private XROrigin xrOrigin;
        [SerializeField] private ImmersalRESTConfiguration hilobbyConfiguration;
        [SerializeField] private ImmersalRESTConfiguration secondLobbyConfiguration;
        [SerializeField] private ImmersalRESTConfiguration myRoomConfiguration;

        private ImmersalRestClient _immersalRestClient;

        public float span = 5f;
        private float defaultSpan;
        private float currentTime = 0f;
        private Vector3 immersalPosition;
        private Quaternion immersalRotation;
        private Vector3 p_TargetPosition;
        private Quaternion p_TargetRotation;
        private Vector3 p_StartPosition;
        private Quaternion p_StartRotation;
        private IDisposable p_Trigger;

        [SerializeField] private float p_LerpTime = 1.0f;
        [SerializeField] private GameObject successPanel;
        [SerializeField] private GameObject failurePanel;
        [SerializeField] private GameObject startingPanel;
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private TextMeshProUGUI stampText;
        [SerializeField] private TextMeshProUGUI delayText;
        [SerializeField] private Toggle slowToggle;

        void Start()
        {
            _immersalRestClient = new ImmersalRestClient(hilobbyConfiguration);
            successPanel.SetActive(false);
            failurePanel.SetActive(false);
            Invoke(nameof(StartGetIntrinsics), 1.0f);
        }

        public void GetIntrinsics()
        {
            GetIntrinsicsAsync().Forget();
            currentTime = 0f;
        }

        public void StartGetIntrinsics()
        {
            GetIntrinsics();
            startingPanel.SetActive(false);
        }

        void Update()
        {
            currentTime += Time.deltaTime;
            if (currentTime > span)
            {
                GetIntrinsics();
            }
        }

        public void ChangeMap()
        {
            if (dropdown.value == 0)
                _immersalRestClient = new ImmersalRestClient(hilobbyConfiguration);
            else if (dropdown.value == 1)
                _immersalRestClient = new ImmersalRestClient(secondLobbyConfiguration);
            else if (dropdown.value == 2)
                _immersalRestClient = new ImmersalRestClient(myRoomConfiguration);
        }

        public void OnClickToggle()
        {
            if (slowToggle.isOn)
            {
                defaultSpan = span;
                span = 300f;
            }
            else
            {
                span = defaultSpan;
            }
        }

        private async UniTask GetIntrinsicsAsync()
        {
            if (!cameraManager.TryAcquireLatestCpuImage(out var image))
                return;

            var cameraTexture = await ARImageProcessingUtil.ConvertARCameraImageToTextureAsync(image, this.GetCancellationTokenOnDestroy());
            image.Dispose();

            if (!cameraManager.TryGetIntrinsics(out var intrinsics))
            {
                Debug.Log("cannot acquire intrinsics");
                return;
            }

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var resText = await _immersalRestClient.SendRequestAsync(intrinsics, cameraTexture);
            sw.Stop();

            DateTime timeStamp = DateTime.Now - sw.Elapsed;
            stampText.text = timeStamp.ToString();
            delayText.text = sw.Elapsed.ToString();
            Debug.Log("Stamp: " + timeStamp);
            Debug.Log("Delay: " + sw.Elapsed);

            var immersalResponse = JsonUtility.FromJson<ImmersalResponseParams>(resText);
            var immersalCameraMatrix = immersalResponse.ToMatrix4();

            immersalPosition = new Vector3(immersalResponse.px, immersalResponse.py, -immersalResponse.pz);
            if (immersalPosition != Vector3.zero)
            {
                Debug.Log("Success");
                failurePanel.SetActive(false);
                successPanel.SetActive(true);
                Vector3 immersalEulerAngles = immersalCameraMatrix.rotation.eulerAngles;
                Vector3 cameraEulerAngles = cameraManager.transform.localEulerAngles;
                immersalRotation.eulerAngles = new Vector3(180 - immersalEulerAngles.x, -immersalEulerAngles.y, 90 - immersalEulerAngles.z);
                p_TargetRotation = immersalRotation * Quaternion.Inverse(cameraManager.transform.localRotation);
                Vector3 rotatedOffset = p_TargetRotation * cameraManager.transform.localPosition;
                p_TargetPosition = immersalPosition - rotatedOffset;
                xrOrigin.transform.position = p_TargetPosition;
                xrOrigin.transform.rotation = p_TargetRotation;
            }
            else
            {
                Debug.Log("Failure");
                successPanel.SetActive(false);
                failurePanel.SetActive(true);
            }
        }

        public void ExecuteMove()
        {
            p_Trigger?.Dispose();
            p_StartPosition = xrOrigin.transform.position;
            p_StartRotation = xrOrigin.transform.rotation;

            p_Trigger = Observable
                .IntervalFrame(1, FrameCountType.FixedUpdate)
                .TimeInterval()
                .Select(timeInterval => timeInterval.Interval)
                .Scan((last, current) => last + current)
                .TakeWhile(intervalTimeSpan => (float)intervalTimeSpan.TotalSeconds < p_LerpTime)
                .SubscribeOnMainThread()
                .Subscribe(
                intervalTimeSpan =>
                {
                    float totalInterval = (float)intervalTimeSpan.TotalSeconds;
                    float t = Mathf.Min(totalInterval / p_LerpTime, 1.0f);
                    float lerpFactor = (t * t) * (3.0f - (2.0f * t));
                    xrOrigin.transform.position = Vector3.Lerp(p_StartPosition, p_TargetPosition, lerpFactor);
                    xrOrigin.transform.rotation = Quaternion.Lerp(p_StartRotation, p_TargetRotation, lerpFactor);
                },
                () =>
                {
                    xrOrigin.transform.position = p_TargetPosition;
                    xrOrigin.transform.rotation = p_TargetRotation;
                })
                .AddTo(this);
        }
    }
}
