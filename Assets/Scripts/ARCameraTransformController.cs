using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImmersalRESTLocalizer.Types;
using Cysharp.Threading.Tasks;
using UnityEngine.XR.ARFoundation;

public class ARCameraTransformController : MonoBehaviour
{
    [SerializeField] private ARSessionOrigin sessionOrigin;
    public float span = 10f;
    private float currentTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("AR Session Origin coordinate: " + sessionOrigin.transform.position.ToString());
        Debug.Log("AR Session Origin rotation: " + sessionOrigin.transform.eulerAngles.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime > span){
            sessionOrigin.transform.position = new Vector3(5, 0, 0);
            sessionOrigin.transform.eulerAngles = new Vector3(0, -90, 0);
            Debug.Log("Changed AR Session Origin coordinate: " + sessionOrigin.transform.position.ToString());
            Debug.Log("Changed AR Session Origin rotation: " + sessionOrigin.transform.eulerAngles.ToString());
            currentTime = 0;
        }
    }
}
