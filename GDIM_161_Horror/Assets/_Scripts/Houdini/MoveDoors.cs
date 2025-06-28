using Mirror;
using OtherUtils;
using System.Collections;
using UnityEngine;

public class MoveDoors : NetworkBehaviour, IDebugger
{
    [SerializeField] protected Transform m_LeftDoorTransform;
    [SerializeField] protected Transform m_RightDoorTransform;
    [SerializeField] protected float m_Speed = 1;

    protected Vector3 m_LeftDoorOpenPosition;
    protected Vector3 m_LefDoorClosedPosition;
    protected Vector3 m_RightDoorOpenPosition;
    protected Vector3 m_RightDoorClosedPosition;

    protected bool m_Debug;

    protected IEnumerator MoveTo(Transform door, Vector3 destPos)
    {
        float t = 0;
        float rate = m_Speed;

        while (t < 1f && door.position != destPos)
        {
            yield return null;
            t += Time.deltaTime * rate;
            door.position = Vector3.Lerp(door.position, destPos, t);
        }
    }

    protected virtual void Start()
    {
        float doorSize = m_RightDoorTransform.GetComponent<Collider>().bounds.size.z;

        m_LefDoorClosedPosition = m_LeftDoorTransform.position;
        m_LeftDoorOpenPosition = m_LefDoorClosedPosition + (transform.forward * doorSize);
        m_RightDoorClosedPosition = m_RightDoorTransform.transform.position;
        m_RightDoorOpenPosition = m_RightDoorClosedPosition - (transform.forward * doorSize);
    }

    public void OpenDoors()
    {
        if (isServer) RpcOpenDoors();
        else CmdOpenDoors();
    }

    [Command]
    private void CmdOpenDoors()
    {
        RpcOpenDoors();
    }

    [ClientRpc]
    protected virtual void RpcOpenDoors()
    {
        StartCoroutine(MoveTo(m_LeftDoorTransform, m_LeftDoorOpenPosition));
        StartCoroutine(MoveTo(m_RightDoorTransform, m_RightDoorOpenPosition));
    }

    public void SetSpeed(float value)
    {
        if (m_Speed < 0) return;
        m_Speed = value;
    }

    public void Debugger(object log)
    {
        if (m_Debug) Debug.Log($"[{GetType().ToString()}]: {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_Debug = active;
    }
}
