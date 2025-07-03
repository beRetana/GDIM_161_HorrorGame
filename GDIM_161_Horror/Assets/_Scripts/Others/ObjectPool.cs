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

        public ObjectPool(T template, int size, Transform parent = null)
        {
            m_Template = template;
            m_Pool = new T[size];
            m_ParentTransform = parent;
            PopulatePool();
        }

        private void PopulatePool()
        {
            for (int i = 0; i < m_Pool.Length; i++)
            {
                T newObject = GameObject.Instantiate(m_Template, m_ParentTransform);
                newObject.gameObject.SetActive(false);
                m_Pool[i] =  newObject;
            }
        }

        public T PoolObject()
        {
            if (m_Pool.Length - 1 == m_Index) return null;

            T pooledObject = m_Pool[m_Index++];
            pooledObject.gameObject.SetActive(true);
            return pooledObject;
        }

        public void ReturnObject(T returnedObject)
        {
            returnedObject.gameObject.SetActive(false);
            --m_Index;
        }
    }
}
