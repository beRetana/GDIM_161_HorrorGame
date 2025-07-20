using Mirror;
using System;
using UnityEngine;

public class FloorActivator : Activator
{
    [SerializeField] private FloorMeta[] m_FloorsMeta;

    private int m_CurrentUpper;
    private int m_CurrentLower;

    [Serializable]
    private enum Floor
    {
        Forest,
        Maze1,
        Maze2,
        Maze3
    }

    [Serializable]
    private struct FloorMeta
    {
        public Transform[] LevelObjects;
        public float LevelHeight;
        public Floor Floor;
    }

    [Server]
    public override void UpdateObjectsState(Vector3[] playerLocations)
    {
        float highest = float.MinValue;
        float lowest = float.MaxValue;

        foreach(Vector3 location in playerLocations)
        {
            if (location.y > highest) highest = location.y;
            if (location.y < lowest) lowest = location.y;
        }

        int upper = 0;
        int lower = 0;
        
        for (byte i = 0; i < m_FloorsMeta.Length; ++i)
        {
            if (m_FloorsMeta[i].LevelHeight <= highest) upper = i;
            if (m_FloorsMeta[i].LevelHeight <= lower) lower = i;
            else break;
        }

        lower = Mathf.Max(0, lower - 1);
        upper = Mathf.Min(m_FloorsMeta.Length - 1, upper + 1);

        if (lower == m_CurrentLower && upper == m_CurrentUpper) return;
        
        m_CurrentUpper = upper;
        m_CurrentLower = lower;

        for (byte i = 0; i < m_FloorsMeta.Length; ++i)
        {
            SetFloorActive((i >= lower && i <= upper), i);
        }
    }

    [ClientRpc]
    private void SetFloorActive(bool active, byte level)
    {
        foreach(Transform levelObject in m_FloorsMeta[level].LevelObjects)
        {
            levelObject.gameObject.SetActive(active);
        }
    }
}
