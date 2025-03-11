using UnityEngine;
using UnityEngine.SceneManagement;
using Dissonance;
using Mirror;
using System.Collections;

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
                Debug.LogError("[Dissonance] DissonanceComms not found in the scene.");
            }
        }
    }

    private IEnumerator RestartDissonance()
    {
        _dissonanceComms.enabled = false;
        
        // Wait for Mirror to fully reinitialize players
        yield return new WaitUntil(() => NetworkClient.ready);

        yield return new WaitForSeconds(1.5f); // Extra delay to ensure networking syncs

        _dissonanceComms.enabled = true;
        Debug.Log("[Dissonance] DissonanceComms restarted after Mirror was ready.");

        // Ensure local player is properly synchronized with Dissonance
        yield return StartCoroutine(ReRegisterLocalPlayer());
    }

    private IEnumerator ReRegisterLocalPlayer()
    {
        yield return new WaitForSeconds(1); // Give time for network updates

        if (_dissonanceComms == null)
        {
            Debug.LogError("[Dissonance] Cannot register player: DissonanceComms is missing!");
            yield break;
        }

        // The following might differ depending on your Dissonance setup:
        var localPlayer = NetworkClient.localPlayer; 
        if (localPlayer != null)
        {
            Debug.Log("[Dissonance] Local player found, ensuring they are tracked.");
            // Ensure local player's Dissonance components are properly set up (this might differ in your project)
            // You can set up voice chat components for the local player or ensure their Dissonance setup is re-registered
        }
        else
        {
            Debug.LogWarning("[Dissonance] No local player found to track.");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
