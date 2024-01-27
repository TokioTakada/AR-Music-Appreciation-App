using UnityEngine;

public class LightPlacement : MonoBehaviour
{
    public Matrix4x4[] targetMatrices; // 13個のMatrix4x4を格納する配列

    void Start()
    {
        SetTransforms();
    }

    void SetTransforms()
    {
        // 子オブジェクトの数だけ繰り返し
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            // i番目の子オブジェクトに対応するMatrix4x4を取得
            Matrix4x4 targetMatrix = targetMatrices[i];

            // 取得したMatrix4x4からQuaternionを取得
            Quaternion rotationFromMatrix = GetQuaternionFromMatrix(targetMatrix);

            // 取得したQuaternionを子オブジェクトのrotationに代入
            child.rotation = rotationFromMatrix;

            Vector3 pos = child.position;
            pos.x = targetMatrix[0, 3];
            pos.y = targetMatrix[1, 3];
            pos.z = targetMatrix[2, 3];

            child.position = pos;
        }
    }

    Quaternion GetQuaternionFromMatrix(Matrix4x4 matrix)
    {
        // Matrix4x4からQuaternionに変換する処理を実装する
        // ここでは例としてidentity行列を使用していますが、実際の要件に合わせて変更してください。
        return matrix.rotation;
    }
}
