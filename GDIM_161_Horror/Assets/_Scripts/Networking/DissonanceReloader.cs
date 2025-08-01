using UnityEngine;
using UnityEngine.SceneManagement;
using Dissonance;
using Mirror;
using System.Collections;

public class DissonanceReloader : MonoBehaviour
{
    private DissonanceComms _dissonanceComms;
    private Dissonance.Integrations.MirrorIgnorance.MirrorIgnoranceCommsNetwork _dissonanceCommsNetwork;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (NewNetworkManager.NewSingleton.IsGameplayScene(scene.name))
        {
            _dissonanceComms = FindFirstObjectByType<DissonanceComms>();
            _dissonanceCommsNetwork = FindFirstObjectByType<Dissonance.Integrations.MirrorIgnorance.MirrorIgnoranceCommsNetwork>();

            if (_dissonanceComms != null && _dissonanceCommsNetwork != null)
            {
                StartCoroutine(RestartDissonance());
            }
            else
            {
                Debug.LogError("[Dissonance] DissonanceComms or MirrorIgnoranceCommsNetwork not found in the scene.");
            }
        }
    }

    private IEnumerator RestartDissonance()
    {
        // Disable DissonanceComms to restart the networking process
        _dissonanceComms.enabled = false;

        // If MirrorIgnoranceCommsNetwork is used, stop the networking (without calling Initialize directly)
        if (_dissonanceCommsNetwork != null)
        {
            // tk Debug.Log("[Dissonance] Restarting MirrorIgnoranceCommsNetwork...");
            _dissonanceCommsNetwork.Stop();  // Stop networking
            yield return new WaitForSeconds(0.5f); // Small delay to ensure it stops

            // You may want to manually restart the connection here or re-enable networking
            // This will depend on your network setup and how MirrorIgnorance is configured
            // Example: _dissonanceCommsNetwork.Start(); // Uncomment if such a method exists
            // tk Debug.Log("[Dissonance] MirrorIgnoranceCommsNetwork stopped.");
        }

        // Wait for Mirror to fully reinitialize players
        yield return new WaitUntil(() => NetworkClient.ready);

        // Wait for additional time to ensure synchronization
        yield return new WaitForSeconds(1.5f); // Extra delay to ensure networking syncs

        // Re-enable DissonanceComms
        _dissonanceComms.enabled = true;
        Debug.Log("[Dissonance] DissonanceComms restarted after Mirror was ready.");

        // Ensure the local player is re-registered (update this to your method of finding the player)
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

        // Find the local player. Replace with correct method if you have a different player tracking method
        var localPlayer = FindLocalPlayer();
        if (localPlayer != null)
        {
            Debug.Log("[Dissonance] Local player found, ensuring they are tracked.");
            // Replace with the correct way to register or track the local player
            RegisterLocalPlayer(localPlayer); // Adjust based on your player management system
        }
        else
        {
            Debug.LogWarning("[Dissonance] No local player found to track.");
        }
    }

    // Replace with your actual method for identifying the local player
    private GameObject FindLocalPlayer()
    {
        // Example method to find local player by tag
        return GameObject.FindWithTag("Player"); // Replace with your actual way to find the player
    }

    // Replace with your actual way of registering or tracking the player
    private void RegisterLocalPlayer(GameObject localPlayer)
    {
        // This may involve adding the player to Dissonance's tracking system or setting up voice for the player
        // Example: _dissonanceComms.TrackPlayer(localPlayer.transform); // Adjust based on actual functionality
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
