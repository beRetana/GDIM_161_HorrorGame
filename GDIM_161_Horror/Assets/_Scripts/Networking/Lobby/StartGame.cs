using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private string _gameSceneName;

    void Start()
    {
        startButton.onClick.AddListener(StartGameServer);
    }

    void StartGameServer()
    {
        LobbyController.Instance.StartGame(_gameSceneName);
    }
}
