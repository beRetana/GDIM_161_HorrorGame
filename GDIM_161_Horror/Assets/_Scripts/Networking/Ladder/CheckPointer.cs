using Mirror;
using UnityEngine;

public class CheckPointer : MonoBehaviour
{
    [SerializeField] private LadderManager m_LadderManager;
    [SerializeField] private byte m_FloorNumber;

    private void OnTriggerEnter(Collider other)
    {
        PlayerDataTracker playerData;
        if (!other.TryGetComponent<PlayerDataTracker>(out playerData)) return;

        playerData.SavedPosition = transform.position;
        playerData.OnReachedNewFloor(m_FloorNumber);
        m_LadderManager?.PlayerCheckedIn(other.transform.root.position);
    }
}
