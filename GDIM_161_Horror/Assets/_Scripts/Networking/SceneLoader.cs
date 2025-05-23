using Mirror;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : NetworkBehaviour
{
    [SerializeField] private List<string> m_scenesToLoad;
    private bool m_loaded;
    void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        if (isServer) RpcScenesLoader();
        else CmdScenesLoader();
    }

    [ClientRpc]
    private void RpcScenesLoader()
    {
        ScenesLoader();
    }

    [Command]
    private void CmdScenesLoader()
    {
        RpcScenesLoader();
    }

    private void SceneLoaded(Scene name, LoadSceneMode mode)
    {
        m_loaded = true;
    }

    private void ScenesLoader()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        foreach(string scene_name in m_scenesToLoad)
        {
            m_loaded = false;
            SceneManager.LoadSceneAsync(scene_name, LoadSceneMode.Additive);
            yield return new WaitUntil(() => m_loaded);
        }
    }
}
