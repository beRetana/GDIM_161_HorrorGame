using Mirror;
using UnityEngine;

public class CheckPointer : MonoBehaviour
{
    [SerializeField] private Transform m_CheckPointPosition;
    [SerializeField] private LadderManager m_LadderManager;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerDataTracker>().SavedPosition = m_CheckPointPosition.position;
        m_LadderManager?.PlayerCheckedIn(other.transform.root.position);
    }
}
