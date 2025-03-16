using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Pickables")]
public class PickableItemSO : ScriptableObject
{
    [SerializeField] private Transform _networkPrefab;
    [SerializeField] private Transform _prefab;
    [SerializeField, Tooltip("The local position of the item on the hand of the player")]
    private Vector3 _pickedPosition = Vector3.zero;
    [SerializeField, Tooltip("The rotation of the item on the hand of the player")]
    private Vector3 _pickedAngle = new Vector3(0f, 270f, 0f);


    public Transform NetworkPrefab { get { return _networkPrefab; } }
    public Transform Prefab { get { return _prefab; } }

    public Vector3 PickedPosition { get { return _pickedPosition; } }
    public Vector3 PickedAngle { get { return _pickedAngle; } }
}
