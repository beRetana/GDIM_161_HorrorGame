using UnityEngine;
using UnityEngine.SceneManagement;

public class DissonanceAudio : MonoBehaviour
{
    [SerializeField] private const string MAIN_SCENE = "BUILD_MainMenu";
    static DissonanceAudio instance;
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += DestroyOnMainMenu;
    }

    private void DestroyOnMainMenu(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == MAIN_SCENE && gameObject !=null )
        {
            SceneManager.sceneLoaded -= DestroyOnMainMenu;
            Destroy(this.gameObject);
        }
    }
}
