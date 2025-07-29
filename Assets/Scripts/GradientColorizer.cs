using UnityEngine;
using System.Collections.Generic; // Listを使うために必要

public class GradientColorizer : MonoBehaviour
{
    [Header("グラデーション設定")]
    public Gradient gradient; // インスペクターでグラデーションを編集します

    [Header("色を付けたいオブジェクト")]
    public List<Renderer> objectsToColor; // ここに対象オブジェクトのRendererを入れます

    // インスペクターのコンポーネントメニューからこのメソッドを実行できるようにします
    [ContextMenu("グラデーションを適用")]
    private void ApplyGradient()
    {
        if (objectsToColor == null || objectsToColor.Count == 0)
        {
            Debug.LogWarning("対象オブジェクトが設定されていません。");
            return;
        }

        // オブジェクトの数だけループ処理
        for (int i = 0; i < objectsToColor.Count; i++)
        {
            // リストの最初(0)から最後(1)までの位置を計算
            float t = (float)i / (objectsToColor.Count - 1);
            if (objectsToColor.Count == 1) t = 0; // オブジェクトが1つの場合は最初の色にする

            // グラデーションから色を取得
            Color color = gradient.Evaluate(t);

            // オブジェクトのMaterialに色を適用
            // 注意: この処理はオブジェクトごとに新しいマテリアルインスタンスを作成します
            objectsToColor[i].material.color = color;
        }

        Debug.Log(objectsToColor.Count + "個のオブジェクトにグラデーションを適用しました。");
    }
}