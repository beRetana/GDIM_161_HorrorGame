using Dissonance;
using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DissonanceReloader : MonoBehaviour
{
    private static DissonanceReloader Singleton;
    private DissonanceComms _dissonanceComms;
    private Dissonance.Integrations.MirrorIgnorance.MirrorIgnoranceCommsNetwork _dissonanceCommsNetwork;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnSceneChanged;
        
    }

    private void OnSceneChanged(Scene current, Scene next)
    {
        if (NewNetworkManager.NewSingleton.GetLobbySceneName() != next.name) return;
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnSceneChanged;
        
        Destroy(this.gameObject);

        StartCoroutine(RestartDissonance());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!NewNetworkManager.NewSingleton.IsGameplayScene(scene.name)) return;

        // Find all DissonanceComms 
        var allComms = FindObjectsByType<DissonanceComms>(FindObjectsSortMode.None);

        // Safe to continue 
        _dissonanceComms = allComms.Length > 0 ? allComms[0] : null;
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

    private IEnumerator RestartDissonance()
    {
        _dissonanceComms.enabled = false;

        if (_dissonanceCommsNetwork != null)
        {
            _dissonanceCommsNetwork.Stop();
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitUntil(() => NetworkClient.ready);
        yield return new WaitForSeconds(1.5f);

        _dissonanceComms.enabled = true;
        Debug.Log("[Dissonance] DissonanceComms restarted after Mirror was ready.");

        yield return StartCoroutine(ReRegisterLocalPlayer());
    }

    private IEnumerator ReRegisterLocalPlayer()
    {
        yield return new WaitForSeconds(1);

        if (_dissonanceComms == null)
        {
            Debug.LogError("[Dissonance] Cannot register player: DissonanceComms is missing!");
            yield break;
        }

        var localPlayer = FindLocalPlayer();
        if (localPlayer != null)
        {
            Debug.Log("[Dissonance] Local player found, ensuring they are tracked.");
            RegisterLocalPlayer(localPlayer);
        }
        else
        {
            Debug.LogWarning("[Dissonance] No local player found to track.");
        }
    }

    private GameObject FindLocalPlayer()
    {
        return GameObject.FindWithTag("Player");
    }

    private void RegisterLocalPlayer(GameObject localPlayer)
    {
        // Example placeholder
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
