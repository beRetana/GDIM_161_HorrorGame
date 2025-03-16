using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Pickables")]
public class PickableItemSO : ScriptableObject
{
    [SerializeField] private Transform _networkPrefab;
    [SerializeField] private Transform _prefab;

    public Transform NetworkPrefab { get { return _networkPrefab; } }
    public Transform Prefab { get { return _prefab; } }
}
