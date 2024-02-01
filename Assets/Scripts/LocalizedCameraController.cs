using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ImmersalRESTLocalizer.Types;
using Cysharp.Threading.Tasks;
using UnityEngine.XR.ARFoundation;
using System;
using UniRx;

namespace ImmersalRESTLocalizer
{
    public class LocalizedCameraController : MonoBehaviour
    {
        [SerializeField] private ARCameraManager cameraManager;
        [SerializeField] private ARSessionOrigin sessionOrigin;
        [SerializeField] private ImmersalRESTConfiguration configuration;

        private ImmersalRestClient _immersalRestClient;

        //[SerializeField] private Transform arSpace;

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

        [SerializeField]
        private float p_LerpTime = 1.0f;

        [SerializeField]
        private GameObject successPanel;
        [SerializeField]
        private GameObject failurePanel;
        [SerializeField]
        private GameObject startingPanel;
        [SerializeField]
        private Toggle slowToggle;

        // Start is called before the first frame update
        void Start()
        {
            _immersalRestClient = new ImmersalRestClient(configuration);
            //GetIntrinsicsAsync().Forget();
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

        void Update() {
            currentTime += Time.deltaTime;

            if(currentTime > span){
                GetIntrinsics();
            }
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
            /*arSpace.position = Vector3.zero;
            arSpace.rotation = Quaternion.identity;*/

            //var cameraMatrix = cameraManager.transform.localToWorldMatrix;

            if (!cameraManager.TryAcquireLatestCpuImage(out var image))
            {
                Debug.Log("cannot acquire cpu image");
                return;
            }

            var cameraTexture = await ARImageProcessingUtil.ConvertARCameraImageToTextureAsync(image, this.GetCancellationTokenOnDestroy());
            image.Dispose();

            if (!cameraManager.TryGetIntrinsics(out var intrinsics))
            {
                Debug.Log("cannot acquire intrinsics");
                return;
            }

            var resText = await _immersalRestClient.SendRequestAsync(intrinsics, cameraTexture);

            var immersalResponse = JsonUtility.FromJson<ImmersalResponseParams>(resText);

            var immersalCameraMatrix = immersalResponse.ToMatrix4();
            /*var mapMatrix = cameraMatrix * immersalCameraMatrix.inverse * arSpace.localToWorldMatrix;

            arSpace.position = mapMatrix.GetColumn(3);
            arSpace.rotation = mapMatrix.rotation;

            Debug.Log(intrinsics);*/
            /*Debug.Log("coordinate: (" + immersalResponse.px + ", " + immersalResponse.py + ", " + immersalResponse.pz + ")");
            Debug.Log("rotation matrix\n" + immersalCameraMatrix.ToString());
            Debug.Log("rotation: " + immersalCameraMatrix.rotation.eulerAngles.ToString());
            Debug.Log("AR Camera coordinate: " + cameraManager.transform.position.ToString());
            Debug.Log("AR Camera rotation: " + cameraManager.transform.rotation.eulerAngles.ToString());*/
            //Debug.Log(arSpace.position);
            immersalPosition = new Vector3(immersalResponse.px, immersalResponse.py, -immersalResponse.pz);
            if (immersalPosition != Vector3.zero)
            {
                Debug.Log("Success!");
                failurePanel.SetActive(false);
                successPanel.SetActive(true);
                //p_TargetPosition = immersalPosition - cameraManager.transform.localPosition;
                //Debug.Log("coordinate: " + p_TargetPosition.ToString());
                Vector3 immersalEulerAngles = immersalCameraMatrix.rotation.eulerAngles;
                Vector3 cameraEulerAngles = cameraManager.transform.localEulerAngles;
                immersalRotation.eulerAngles = new Vector3(180 - immersalEulerAngles.x, - immersalEulerAngles.y, 90 - immersalEulerAngles.z);
                p_TargetRotation = immersalRotation * Quaternion.Inverse(cameraManager.transform.localRotation);
                Vector3 rotatedOffset = p_TargetRotation * cameraManager.transform.localPosition;
                p_TargetPosition = immersalPosition - rotatedOffset;
                Debug.Log("coordinate: " + p_TargetPosition.ToString());
                Debug.Log("rotation: " + p_TargetRotation.eulerAngles.ToString());
                /*cameraManager.transform.localPosition = Vector3.zero;
                cameraManager.transform.localEulerAngles = Vector3.zero;*/
                //ExecuteMove();
                sessionOrigin.transform.position = p_TargetPosition;
                sessionOrigin.transform.rotation = p_TargetRotation;
                Debug.Log("AR Camera coordinate: " + cameraManager.transform.position.ToString());
                Debug.Log("AR Camera rotation: " + cameraManager.transform.rotation.eulerAngles.ToString());
            }
            else
            {
                Debug.Log("Failure...");
                successPanel.SetActive(false);
                failurePanel.SetActive(true);
            }
        }

        public void ExecuteMove()
        {
            // 前回トリガーを終了する
            p_Trigger?.Dispose();

            // カメラの移動開始地点を保存する
            p_StartPosition = sessionOrigin.transform.position;
            p_StartRotation = sessionOrigin.transform.rotation;

            // 移動処理を開始する
            p_Trigger = Observable
                .IntervalFrame(1, FrameCountType.FixedUpdate)    // 1フレーム毎に呼び出す
                .TimeInterval()                                  // フレーム間の経過時間を取得する
                .Select(timeInterval => timeInterval.Interval)   // TimeSpan型のデータを抽出する
                .Scan((last, current) => last + current)         // 前回までの経過時間を加算する
                .TakeWhile(intervalTimeSpan => (float)intervalTimeSpan.TotalSeconds < p_LerpTime) // Lerp時間を超えるまで実行する
                .SubscribeOnMainThread()                         // メインスレッドで実行する
                .Subscribe(
                intervalTimeSpan =>
                {
                    float totalInterval = (float)intervalTimeSpan.TotalSeconds;
                    // Ease-in, Ease-Out の計算式で徐々に加速して徐々に減速する補間を行う
                    float t = Mathf.Min(totalInterval / p_LerpTime, 1.0f);
                    float lerpFactor = (t * t) * (3.0f - (2.0f * t));
                    // Leap関数を使って徐々にターゲットに近づいていく
                    sessionOrigin.transform.position = Vector3.Lerp(p_StartPosition, p_TargetPosition, lerpFactor);
                    sessionOrigin.transform.rotation = Quaternion.Lerp(p_StartRotation, p_TargetRotation, lerpFactor);
                },
                () =>
                {
                    // 最終的に指定のトランスフォームに到達させる
                    sessionOrigin.transform.position = p_TargetPosition;
                    sessionOrigin.transform.rotation = p_TargetRotation;
                }
                )
                .AddTo(this);
        }
    }
}