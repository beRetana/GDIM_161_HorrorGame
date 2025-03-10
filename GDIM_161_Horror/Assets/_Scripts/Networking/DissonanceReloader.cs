using UnityEngine;
using UnityEngine.SceneManagement;
using Dissonance;
using Mirror;

public class DissonanceReloader : MonoBehaviour
{
    private DissonanceComms _dissonanceComms;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BUILD_1")
        {
            _dissonanceComms = FindObjectOfType<DissonanceComms>();

            if (_dissonanceComms != null)
            {
                StartCoroutine(RestartDissonance());
            }
            else
            {
                Debug.LogError("DissonanceComms not found in the scene.");
            }
        }
    }

    private System.Collections.IEnumerator RestartDissonance()
{
     _dissonanceComms.enabled = false;
    
    // Wait for Mirror to fully reinitialize players
    yield return new WaitUntil(() => NetworkClient.ready);

    yield return new WaitForSeconds(0.5f); // Extra delay to ensure networking syncs

    _dissonanceComms.enabled = true;
    Debug.Log("DissonanceComms restarted after Mirror was ready.");
}


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
