using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCalculator : MonoBehaviour
{
    public Transform targetTransform; // 目標のワールド座標系におけるposition

    void Start()
    {
        CalculateParentPosition();
    }

    void CalculateParentPosition()
    {
        // 親オブジェクトのrotationを考慮した子オブジェクトのワールド座標系におけるpositionを計算
        Vector3 rotatedOffset = this.transform.rotation * this.transform.GetChild(0).localPosition;

        // 親オブジェクトのpositionを計算して目標のワールド座標系におけるpositionを達成
        Vector3 parentPosition = targetTransform.position - rotatedOffset;

        // 親オブジェクトのpositionを設定
        this.transform.position = parentPosition;
    }
}
