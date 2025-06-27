using UnityEngine;

namespace OtherUtils
{
    public class MasterMenuUI : MonoBehaviour
    {
        public static MasterMenuUI instance;
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
