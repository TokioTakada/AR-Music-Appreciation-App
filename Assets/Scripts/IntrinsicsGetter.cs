using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImmersalRESTLocalizer.Types;
using Cysharp.Threading.Tasks;
using UnityEngine.XR.ARFoundation;

namespace ImmersalRESTLocalizer
{
    public class IntrinsicsGetter : MonoBehaviour
    {
        [SerializeField] private ARCameraManager cameraManager;

        [SerializeField] private ImmersalRESTConfiguration configuration;

        private ImmersalRestClient _immersalRestClient;

        //[SerializeField] private Transform arSpace;

        public float span = 10f;
        private float currentTime = 0f;

        // Start is called before the first frame update
        void Start()
        {
            _immersalRestClient = new ImmersalRestClient(configuration);
            GetIntrinsicsAsync().Forget();
        }

        public void GetIntrinsics()
        {
            GetIntrinsicsAsync().Forget();
        }

        void Update() {
            currentTime += Time.deltaTime;

            if(currentTime > span){
                GetIntrinsicsAsync().Forget();
                currentTime = 0f;
            }
        }

        private async UniTask GetIntrinsicsAsync()
        {
            /*arSpace.position = Vector3.zero;
            arSpace.rotation = Quaternion.identity;*/

            var cameraMatrix = cameraManager.transform.localToWorldMatrix;

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
            Debug.Log("coordinate: (" + immersalResponse.px + ", " + immersalResponse.py + ", " + immersalResponse.pz + ")");
            Debug.Log("rotation matrix\n" + immersalCameraMatrix.ToString());
            Debug.Log("rotation: " + immersalCameraMatrix.rotation.eulerAngles.ToString());
            Debug.Log("AR Camera coordinate: " + cameraManager.transform.position.ToString());
            Debug.Log("AR Camera rotation: " + cameraManager.transform.rotation.eulerAngles.ToString());
            //Debug.Log(arSpace.position);
        }
    }
}