using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public class CameraMoveSmooth : MonoBehaviour
{
    /// <summary>
    /// カメラの移動先
    /// (ContextMenuItemによりInspectorからExecuteMoveを実行できる)
    /// </summary>
    /*[SerializeField, ContextMenuItem("Move", "ExecuteMove")]
    private Transform p_TargetTransform;*/

    public GameObject leftyAxisObject;
    public Vector3 p_TargetPosition;
    public Quaternion p_TargetRotation;

    /// <summary>
    /// カメラの移動開始地点
    /// </summary>
    private Vector3 p_StartPosition;
    private Quaternion p_StartRotation;

    /// <summary>
    /// 継続処理の参照
    /// </summary>
    private IDisposable p_Trigger;

    [SerializeField]
    private float p_LerpTime = 1.0f;

    void Start()
    {
        ExecuteMove();
    }

    public void ExecuteMove()
    {
        // 前回トリガーを終了する
        p_Trigger?.Dispose();

        // カメラの移動開始地点を保存する
        p_StartPosition = leftyAxisObject.transform.position;
        p_StartRotation = leftyAxisObject.transform.rotation;

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
                leftyAxisObject.transform.position = Vector3.Lerp(p_StartPosition, p_TargetPosition, lerpFactor);
                leftyAxisObject.transform.rotation = Quaternion.Lerp(p_StartRotation, p_TargetRotation, lerpFactor);
            },
            () =>
            {
                // 最終的に指定のトランスフォームに到達させる
                leftyAxisObject.transform.position = p_TargetPosition;
                leftyAxisObject.transform.rotation = p_TargetRotation;
            }
            )
            .AddTo(this);
    }
}