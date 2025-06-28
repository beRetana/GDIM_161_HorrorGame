using System.Collections;
using UnityEngine;

public class MoveDoors : MonoBehaviour
{
    [SerializeField] protected Transform m_LeftDoorTransform;
    [SerializeField] protected Transform m_RightDoorTransform;
    [SerializeField] protected float speed = 1;

    protected Vector3 m_LeftDoorOpenPosition;
    protected Vector3 m_LefDoorClosedPosition;
    protected Vector3 m_RightDoorOpenPosition;
    protected Vector3 m_RightDoorClosedPosition;

    protected IEnumerator MoveTo(Transform door, Vector3 destPos)
    {
        float t = 0;
        float rate = speed;

        while (t < 1f && door.position != destPos)
        {
            yield return null;
            t += Time.deltaTime * rate;
            door.position = Vector3.Lerp(door.position, destPos, t);
        }
    }

    protected void Start()
    {
        float doorSize = GetComponent<Collider>().bounds.size.z / 2f;

        m_LefDoorClosedPosition = m_LeftDoorTransform.position;
        m_LeftDoorOpenPosition = m_LefDoorClosedPosition + (transform.forward * doorSize);
        m_RightDoorClosedPosition = m_RightDoorTransform.transform.position;
        m_RightDoorOpenPosition = m_RightDoorClosedPosition - (transform.forward * doorSize);
    }
}
