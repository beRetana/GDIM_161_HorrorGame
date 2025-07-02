using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MeshCombinerEditor : MonoBehaviour
{
    [MenuItem("Tools/Combine Selected Meshes")]
    static void CombineSelectedMeshes()
    {
        if (Selection.transforms.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

        // Create a new GameObject to hold the combined mesh
        GameObject combinedParent = new GameObject("Combined Mesh");

        List<CombineInstance> combineList = new List<CombineInstance>();
        Material sharedMaterial = null;

        foreach (var transform in Selection.transforms)
        {
            MeshFilter mf = transform.GetComponent<MeshFilter>();
            MeshRenderer mr = transform.GetComponent<MeshRenderer>();

            if (mf == null || mr == null || mf.sharedMesh == null) continue;

            if (sharedMaterial == null)
                sharedMaterial = mr.sharedMaterial;

            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = mf.transform.localToWorldMatrix;
            combineList.Add(ci);
        }

        if (combineList.Count == 0)
        {
            Debug.LogWarning("No valid meshes found in selection.");
            return;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineList.ToArray(), true, true);

        MeshFilter combinedMeshFilter = combinedParent.AddComponent<MeshFilter>();
        MeshRenderer combinedMeshRenderer = combinedParent.AddComponent<MeshRenderer>();

        combinedMeshFilter.sharedMesh = combinedMesh;
        combinedMeshRenderer.sharedMaterial = sharedMaterial;

        // Optionally add a MeshCollider for physics
        MeshCollider meshCollider = combinedParent.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = combinedMesh;

        // Save the mesh asset (optional)
        AssetDatabase.CreateAsset(combinedMesh, "Assets/CombinedMesh.asset");
        AssetDatabase.SaveAssets();

        Debug.Log("Mesh combining complete. Combined " + combineList.Count + " meshes.");
    }
}
