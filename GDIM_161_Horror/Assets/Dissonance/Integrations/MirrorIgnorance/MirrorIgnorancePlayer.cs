using Mirror;
using UnityEngine;
using System.Collections;

namespace Dissonance.Integrations.MirrorIgnorance
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class MirrorIgnorancePlayer
        : NetworkBehaviour, IDissonancePlayer
    {
        private static readonly Log Log = Logs.Create(LogCategory.Network, "Mirror Player Component");

        private DissonanceComms _comms;
        private bool _isSceneTransitioning = false;

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

        public void OnDestroy()
        {
            if (_comms != null)
                _comms.LocalPlayerNameChanged -= SetPlayerName;
        }

        public void OnEnable()
        {
            StartCoroutine(FindDissonanceComms());
        }

        private IEnumerator FindDissonanceComms()
        {
            while (_comms == null)
            {
                _comms = FindObjectOfType<DissonanceComms>();

                if (_comms != null)
                {
                    Debug.Log("[Dissonance] DissonanceComms found, resuming player tracking.");
                    if (!string.IsNullOrEmpty(PlayerId))
                        StartTracking();
                }

                yield return null; // Wait until the next frame to check again
            }
        }

        public void OnDisable()
        {
            if (_isSceneTransitioning) return; // Avoid duplicate stop calls
            _isSceneTransitioning = true;

            if (IsTracking)
            {
                Debug.Log("[Dissonance] Stopping tracking due to scene transition.");
                StopTracking();
            }
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            if (_comms == null)
                _comms = FindObjectOfType<DissonanceComms>();

            if (_comms == null)
            {
                Debug.LogError("[Dissonance] Cannot find DissonanceComms component in scene!");
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
            _isSceneTransitioning = false; // Reset flag

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
