using MessengerSystem;
using UnityEngine;

public class TrainningVisuals : MonoBehaviour
{
    private MeshRenderer floorMesh;

    void Start()
    {
        floorMesh = GetComponent<MeshRenderer>();
    }

    public void SetFloorColor(Color color)
    {
        floorMesh.material.color = color;
    }
}
