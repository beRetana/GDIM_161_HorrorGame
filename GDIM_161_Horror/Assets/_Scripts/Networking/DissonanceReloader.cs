using UnityEngine;
using UnityEngine.SceneManagement;
using Dissonance;

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
        yield return null; // Wait a frame
        _dissonanceComms.enabled = true;
        Debug.Log("DissonanceComms restarted.");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}