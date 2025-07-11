using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace OtherUtils
{
    public class ObjectPool<T> where T : Component
    {
        readonly Transform m_ParentTransform;
        readonly T m_Template;
        readonly T[] m_Pool;
        private int m_Index;

        public ObjectPool(T template, int size)
        {
            m_Template = template;
            m_Pool = new T[size];
        }

        public T GetObject()
        {
            if (m_Pool.Length <= m_Index) return null;

            T pooledObject = m_Pool[m_Index++];
            return pooledObject;
        }

        public void AddObject(T newObject)
        {
            if (m_Pool.Length <= m_Index)
            {
                Debug.LogWarning($"[{this.GetType().ToString()}]:" +
                    $"Tried to add a new Object beyond pool capacity of {m_Pool.Length}");
                m_Index = m_Pool.Length - 1; // Added as safety guard.
                return;
            }
            m_Pool[m_Index++] = newObject;
        }

        public void ReturnObject(T returnedObject)
        {
            if (m_Index <= 0)
            {
                Debug.LogWarning($"[{this.GetType().ToString()}]:" +
                    $"Tried to Return an Object beyond pool capacity of {m_Pool.Length}");
                m_Index = 0; // Reset to zero as safety guard.
                return;
            }
            --m_Index;
        }
    }
}
