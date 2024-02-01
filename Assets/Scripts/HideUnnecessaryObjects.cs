using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideUnnecessaryObjects : MonoBehaviour
{
    /*[SerializeField]
    private MeshRenderer parentMeshRenderer;*/
    [SerializeField]
    private GameObject pointCloud;
    [SerializeField]
    private GameObject offset;
    [SerializeField]
    private Toggle toggle;

    private void Start()
    {
        /*parentMeshRenderer.enabled = false;

            // 親オブジェクトから子オブジェクトを取得
        Transform childTransform = parentMeshRenderer.transform.GetChild(0);*/

        // 子オブジェクトから孫オブジェクト（MeshRendererを持つオブジェクト）を取得
        /*MeshRenderer[] grandchildMeshRenderers = childTransform.GetComponentsInChildren<MeshRenderer>();

        // 全ての孫オブジェクトに対して繰り返し処理
        foreach (MeshRenderer grandchildMeshRenderer in grandchildMeshRenderers)
        {
            grandchildMeshRenderer.enabled = false;
        }*/

        /*for (int i = 0; i < childTransform.childCount; i++)
        {
            Transform grandchildTransform = childTransform.GetChild(i);
            MeshRenderer grandchildMeshRenderer = grandchildTransform.GetComponent<MeshRenderer>();
            grandchildMeshRenderer.enabled = false;
        }*/
        /*foreach (MeshRenderer child in pointCloud.GetComponentsInChildren<MeshRenderer>())
        {
            child.enabled = false;
        }*/

        MeshRenderer parentMeshRenderer = pointCloud.GetComponent<MeshRenderer>();
        parentMeshRenderer.enabled = false;
    }

    public void OnClickToggle()
    {
        foreach (MeshRenderer child in offset.GetComponentsInChildren<MeshRenderer>())
        {
            child.enabled = toggle.isOn;
        }
    }
}
