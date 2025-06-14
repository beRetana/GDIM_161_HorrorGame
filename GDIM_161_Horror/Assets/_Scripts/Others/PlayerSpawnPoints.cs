using UnityEngine;

namespace OtherUtils
{
    public class PlayerSpawnPoints : MonoBehaviour
    {
        static PlayerSpawnPoints instance;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
                
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
