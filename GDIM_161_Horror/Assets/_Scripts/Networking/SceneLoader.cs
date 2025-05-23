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
    public bool m_debugger;
    void Start()
    {
        Debugger("STARTED");
        SceneManager.sceneLoaded += SceneLoaded;
        if (isServer) RpcScenesLoader();
        else CmdScenesLoader();
    }

    [ClientRpc]
    private void RpcScenesLoader()
    {
        Debugger("CLIENT");
        ScenesLoader();
    }

    [Command]
    private void CmdScenesLoader()
    {
        Debugger("SERVER");
        RpcScenesLoader();
    }

    private void SceneLoaded(Scene name, LoadSceneMode mode)
    {
        Debugger("SCENE LOADED");
        m_loaded = true;
    }

    private void ScenesLoader()
    {
        Debugger("COROUTINE");
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
