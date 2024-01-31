using UnityEngine;

/// <summary>
/// ターゲットに振り向くスクリプト
/// </summary>
internal class LookAtTarget : MonoBehaviour
{
    // 自身のTransform
    [SerializeField] private Transform _target;

    private void Update()
    {
        // ターゲットの方向に自身を回転させる
        this.transform.LookAt(_target);
    }
}