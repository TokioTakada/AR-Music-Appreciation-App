using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MaterialChangerWindow : EditorWindow
{
    private Material targetMaterial;
    private Material newMaterial;
    private List<GameObject> objectsWithSameMaterial = new List<GameObject>();
    private Vector2 scrollPosition;

    [MenuItem("Nebula Tools/Material Changer")] 
    static void Init()
    {
        MaterialChangerWindow window = (MaterialChangerWindow)EditorWindow.GetWindow(typeof(MaterialChangerWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Material Changer", EditorStyles.boldLabel);

        targetMaterial = EditorGUILayout.ObjectField("Target Material", targetMaterial, typeof(Material), false) as Material;
        newMaterial = EditorGUILayout.ObjectField("New Material", newMaterial, typeof(Material), false) as Material;

        if (GUILayout.Button("Find Objects with Same Material"))
        {
            FindObjectsWithSameMaterial();
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (objectsWithSameMaterial.Count > 0)
        {
            GUILayout.Label("Objects with Same Material:");
            foreach (var obj in objectsWithSameMaterial)
            {
                EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
            }

            if (GUILayout.Button("Select All Objects"))
            {
                SelectAllObjects();
            }

            if (GUILayout.Button("Change Material"))
            {
                ChangeMaterial();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void FindObjectsWithSameMaterial()
    {
        objectsWithSameMaterial.Clear();
        if (targetMaterial == null)
        {
            Debug.LogWarning("Please select a target material first.");
            return;
        }

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (var obj in allObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material[] materials = renderer.sharedMaterials;
                foreach (Material material in materials)
                {
                    if (material == targetMaterial)
                    {
                        objectsWithSameMaterial.Add(obj);
                        break; 
                    }
                }
            }
        }
    }

    private void SelectAllObjects()
    {
        Selection.objects = objectsWithSameMaterial.ToArray();
    }

    private void ChangeMaterial()
    {
        if (newMaterial == null)
        {
            Debug.LogWarning("Please select a new material.");
            return;
        }

        foreach (var obj in objectsWithSameMaterial)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material[] materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == targetMaterial)
                    {
                        materials[i] = newMaterial;
                    }
                }
                renderer.sharedMaterials = materials;
            }
        }
    }
}
