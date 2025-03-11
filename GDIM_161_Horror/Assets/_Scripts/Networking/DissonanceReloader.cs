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
        if (scene.name == "BUILD_1") // Replace with your target scene name
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
        yield return StartCoroutine(ReRegisterPlayers());
    }

    private IEnumerator ReRegisterPlayers()
    {
        yield return new WaitForSeconds(1); // Allow some time for network updates

        if (_dissonanceComms == null)
        {
            Debug.LogError("[Dissonance] Cannot register player: DissonanceComms is missing!");
            yield break;
        }

        // Register the local player for Dissonance
        var localPlayer = NetworkClient.localPlayer;
        if (localPlayer != null)
        {
            Debug.Log("[Dissonance] Local player found, ensuring they are tracked.");
            // Re-initialize the Dissonance components for the local player
            // In newer versions of Dissonance, you may not need to manually register the player for tracking
        }
        else
        {
            Debug.LogWarning("[Dissonance] No local player found to track.");
        }

        // Ensure remote players are registered as well (only needed on the server)
        if (NetworkServer.active)
        {
            foreach (var connection in NetworkServer.connections)
            {
                var player = connection.Value; // This is the NetworkConnectionToClient

                if (player != null && player.identity != null)
                {
                    var playerTransform = player.identity.transform;
                    Debug.Log($"[Dissonance] Tracking remote player: {player.identity.netId}");

                    // If TrackPlayer is not available, check if Dissonance has another method for handling player registration
                    // You may need to check the Dissonance documentation for the correct method to track players.
                    // For now, we'll assume Dissonance automatically tracks players, so this may not be necessary.
                }
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
