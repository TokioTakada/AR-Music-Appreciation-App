using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateRotation : MonoBehaviour
{
    public Transform targetTransform; // 目標の回転をアサイン

    void Start()
    {
        AdjustRotation();
    }

    void AdjustRotation()
    {
        // 目標の回転を考慮してAR Session Originのローカル座標系での目標回転を計算
        Quaternion requiredRotation = targetTransform.rotation * Quaternion.Inverse(this.transform.GetChild(0).localRotation);

        // AR Session Originのローカル座標系での回転を更新
        this.transform.rotation = requiredRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
