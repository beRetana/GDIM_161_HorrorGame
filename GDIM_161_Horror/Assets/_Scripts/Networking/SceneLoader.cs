using Mirror;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : NetworkBehaviour
{
    [SerializeField] private List<string> m_scenesToLoad;
    [SyncVar] private bool m_loaded;
    public bool m_debugger;
    void Start()
    {
        Debugger("STARTED");
        SceneManager.sceneLoaded += SceneLoaded;
        if (isServer) ScenesLoader();
    }

    [ClientRpc]
    private void RpcSceneLoaded()
    {
        Debugger("CLIENT: SCENE LOADED");
    }

    [Command]
    private void CmdSceneLoaded()
    {
        Debugger("SERVER: SCENE LOADED");
        RpcSceneLoaded();
    }

    private void SceneLoaded(Scene name, LoadSceneMode mode)
    {
        m_loaded = true;
        if (isServer) RpcSceneLoaded();
        else CmdSceneLoaded();
    }

    [Server]
    private void ScenesLoader()
    {
        Debugger("COROUTINE");
        if (!NetworkServer.active) return;
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        foreach(string scene_name in m_scenesToLoad)
        {
            m_loaded = false;
            Debugger("LOADING");
            SceneManager.LoadSceneAsync(scene_name, LoadSceneMode.Additive);
            yield return new WaitUntil(() => m_loaded);
        }
    }

    private void Debugger(object log)
    {
        if (m_debugger) Debug.Log(log);
    }
}
