using UnityEngine;

public class AutomaticDoors : MoveDoors
{
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(MoveTo(m_LeftDoorTransform, m_LeftDoorOpenPosition));
        StartCoroutine(MoveTo(m_RightDoorTransform, m_RightDoorOpenPosition));
    }

    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(MoveTo(m_LeftDoorTransform, m_LefDoorClosedPosition));
        StartCoroutine(MoveTo(m_RightDoorTransform, m_RightDoorClosedPosition));
    }
}
