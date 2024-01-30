using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileVisibilityController : MonoBehaviour
{
    [SerializeField]
    private GameObject tile;
    [SerializeField]
    private Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickToggle()
    {
        foreach (MeshRenderer child in tile.GetComponentsInChildren<MeshRenderer>())
        {
            child.enabled = toggle.isOn;
        }
    }
}
