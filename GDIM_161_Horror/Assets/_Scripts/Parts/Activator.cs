using Mirror;
using OtherUtils;
using UnityEngine;

public abstract class Activator : NetworkBehaviour, IDebugger
{
    protected bool m_Debugger;

    public abstract void UpdateObjectsState(Vector3[] playerLocations);

    public void Debugger(object log)
    {
        if (m_Debugger) Debug.Log($"[{GetType().ToString()}]: {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_Debugger = active;
    }
}
