using UnityEngine;

public class PlayerDataTracker : MonoBehaviour
{
    private Vector3 m_SavedPosition;

    public Vector3 SavedPosition { get { return m_SavedPosition; } set { m_SavedPosition = value; } }

    private void Start()
    {
        m_SavedPosition = Vector3.zero;
    }
}
