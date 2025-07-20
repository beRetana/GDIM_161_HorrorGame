//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;
//using UnityMeshSimplifier; // Make sure the library is imported

//public class SimplifyMeshEditor : EditorWindow
//{
//    private GameObject targetObject;
//    private float quality = 0.5f;

//    [MenuItem("Tools/Mesh Simplifier")]
//    public static void ShowWindow()
//    {
//        GetWindow<SimplifyMeshEditor>("Mesh Simplifier");
//    }

//    void OnGUI()
//    {
//        GUILayout.Label("Simplify Mesh", EditorStyles.boldLabel);
//        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);
//        quality = EditorGUILayout.Slider("Quality", quality, 0.0f, 1.0f);

//        if (GUILayout.Button("Simplify"))
//        {
//            SimplifySelectedMesh();
//        }
//    }

//    void SimplifySelectedMesh()
//    {
//        if (targetObject == null)
//        {
//            Debug.LogWarning("No GameObject selected.");
//            return;
//        }

//        var meshFilter = targetObject.GetComponent<MeshFilter>();
//        if (meshFilter == null || meshFilter.sharedMesh == null)
//        {
//            Debug.LogWarning("Selected object doesn't have a MeshFilter with a mesh.");
//            return;
//        }

//        var originalMesh = meshFilter.sharedMesh;
//        var simplifier = new MeshSimplifier();
//        simplifier.Initialize(originalMesh);
//        simplifier.SimplifyMesh(quality);
//        var simplifiedMesh = simplifier.ToMesh();

//        // Save new mesh as an asset
//        string path = "Assets/Simplified_" + targetObject.name + ".asset";
//        AssetDatabase.CreateAsset(simplifiedMesh, AssetDatabase.GenerateUniqueAssetPath(path));
//        AssetDatabase.SaveAssets();

//        // Apply simplified mesh
//        meshFilter.sharedMesh = simplifiedMesh;

//        Debug.Log($"Mesh simplified and saved to {path}");
//    }
//}
