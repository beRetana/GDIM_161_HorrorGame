using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Pickables")]
public class PickableItemSO : ScriptableObject
{
    [Header("Left Hand Socket Location and Rotation")]
    [SerializeField, Tooltip("The local position of the item on the hand of the player")]
    private Vector3 _leftHandPosition = Vector3.zero;
    [SerializeField, Tooltip("The rotation of the item on the hand of the player")]
    private Vector3 _leftHandRotation = new Vector3(0f, 270f, 0f);

    [Header("Right Hand Socket Location and Rotation")]
    [SerializeField, Tooltip("The local position of the item on the hand of the player")]
    private Vector3 _rightHandPosition = Vector3.zero;
    [SerializeField, Tooltip("The rotation of the item on the hand of the player")]
    private Vector3 _rightHandRotation = new Vector3(0f, 270f, 0f);

    public Vector3 LeftHandPosition { get { return _leftHandPosition; } }
    public Vector3 LeftHandRotation { get { return _leftHandRotation; } }
    public Vector3 RightHandPosition { get { return _rightHandPosition; } }
    public Vector3 RightHandRotation { get { return _rightHandRotation; } }
}
