using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dissonance.Integrations.MirrorIgnorance
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class MirrorIgnorancePlayer : NetworkBehaviour, IDissonancePlayer
    {
        private static readonly Log Log = Logs.Create(LogCategory.Network, "Mirror Player Component");

        private DissonanceComms _comms;
        private bool _sceneLoaded = false; // Ensure script activates only in the correct scene

        public bool IsTracking { get; private set; }

        [SyncVar]
        private string _playerId;
        public string PlayerId { get { return _playerId; } }

        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;

        public NetworkPlayerType Type
        {
            get
            {
                if (_comms == null || _playerId == null)
                    return NetworkPlayerType.Unknown;
                return _comms.LocalPlayerName.Equals(_playerId) ? NetworkPlayerType.Local : NetworkPlayerType.Remote;
            }
        }

        private void Awake()
        {
            enabled = false; // Start disabled
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_comms != null)
                _comms.LocalPlayerNameChanged -= SetPlayerName;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Game") // Replace "Game" with your actual game scene name
            {
                _sceneLoaded = true;
                _comms = FindObjectOfType<DissonanceComms>();

                if (_comms == null)
                {
                    Debug.LogError("DissonanceComms not found in Game scene!");
                    return;
                }

                enabled = true; // Activate the script only in the correct scene
            }
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            if (!_sceneLoaded) return; // Ensure the scene is loaded before proceeding

            if (_comms == null)
            {
                Debug.LogError("DissonanceComms is still null in OnStartLocalPlayer!");
                return;
            }

            Log.Debug("Tracking `OnStartLocalPlayer` Name={0}", _comms.LocalPlayerName);

            if (_comms.LocalPlayerName != null)
                SetPlayerName(_comms.LocalPlayerName);

            _comms.LocalPlayerNameChanged += SetPlayerName;
        }

        private void SetPlayerName(string playerName)
        {
            if (IsTracking)
                StopTracking();

            _playerId = playerName;
            StartTracking();

            if (isLocalPlayer)
                CmdSetPlayerName(playerName);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!string.IsNullOrEmpty(PlayerId))
                StartTracking();
        }

        [Command]
        private void CmdSetPlayerName(string playerName)
        {
            _playerId = playerName;
            RpcSetPlayerName(playerName);
        }

        [ClientRpc]
        private void RpcSetPlayerName(string playerName)
        {
            if (!isLocalPlayer)
                SetPlayerName(playerName);
        }

        private void StartTracking()
        {
            if (IsTracking)
                throw Log.CreatePossibleBugException("Attempting to start player tracking, but tracking is already started", "31971B1F-52FD-4FCF-89E9-67A17A917921");

            if (_comms != null)
            {
                _comms.TrackPlayerPosition(this);
                IsTracking = true;
            }
        }

        private void StopTracking()
        {
            if (!IsTracking)
                throw Log.CreatePossibleBugException("Attempting to stop player tracking, but tracking is not started", "C7CF0174-0667-4F07-88E3-800ED652142D");

            if (_comms != null)
            {
                _comms.StopTracking(this);
                IsTracking = false;
            }
        }
    }
}
