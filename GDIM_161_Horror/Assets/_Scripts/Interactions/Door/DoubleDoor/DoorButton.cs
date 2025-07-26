using Mirror;
using OtherUtils;
using System.Collections;
using UnityEngine;

namespace Interactions
{
    public class DoorButton : NetworkBehaviour, IDebugger
    {
        [SerializeField] private DoubleDoor m_DoorsManager;
        [SerializeField] private Transform m_ButtonModel;
        [SerializeField] private MeshRenderer m_MeshRenderer;
        [SerializeField] private Material m_OffMaterial;
        [SerializeField] private Material m_OnMaterial;
        [SerializeField] private float m_NewButtonScale;
        [SerializeField] private float m_WaitTime;
        [SerializeField] private float m_AnimTime;
        
        private InteractableItem m_InteractableItem;
        private Vector3 m_InitialScale;
        private Vector3 m_PressedScale;
        private bool _debugger;

        [SyncVar] private bool m_IsButtonPressed;

        private void Start()
        {
            m_InteractableItem = GetComponent<PolyInteractable>();
            m_InteractableItem.SetInteractAction(OnInteracted);
            m_InitialScale = m_ButtonModel.localScale;
            m_PressedScale = new Vector3(1f, m_NewButtonScale, 1f); 
            m_MeshRenderer.material = m_OffMaterial;
        }

        public void OnInteracted(int playerID)
        {
            if (!PlayerManager.Instance.GetPlayer(playerID).isLocalPlayer) return;
            if (m_IsButtonPressed) return;
            if (m_DoorsManager.HasPlayerPressed((byte)playerID)) return;
            ButtonPressed((byte)playerID);
        }

        public void ButtonPressed(byte playerID)
        {
            UpdateButtonState(true);
            SetInteractive(false);
            m_DoorsManager.AddPlayer(playerID);
            StartCoroutine(ButtonScaleAnimation(playerID, true, m_InitialScale,
                m_PressedScale, m_OnMaterial));
            Debugger($"{transform.parent.parent.parent.name}: Has been pressed by Player {playerID}");
        }

        private void ResetButton(byte playerID)
        {
            UpdateButtonState(false);
            SetInteractive(true);
            m_DoorsManager.RemovePlayer(playerID);
            StartCoroutine(ButtonScaleAnimation(playerID, false, m_PressedScale,
                m_InitialScale, m_OffMaterial));
            Debugger($"{transform.parent.parent.parent.name}: Has been resetted");
        }

        private IEnumerator ButtonScaleAnimation(byte playerID, bool reset, Vector3 initial, Vector3 final, Material newMaterial)
        {
            float ratio = 0;
            for (float time = 0; ratio <= 1; time += Time.deltaTime)
            {
                ratio = time / m_AnimTime;
                m_ButtonModel.localScale = Vector3.Lerp(initial, final, ratio);
                yield return null;
            }
            m_MeshRenderer.material = newMaterial;

            yield return new WaitForSeconds(m_WaitTime);

            if (reset && m_DoorsManager.State == DoubleDoor.DoorState.Locked)
            {
                ResetButton(playerID);
            }
        }

        private void UpdateButtonState(bool isPlayerOnHandle)
        {
            if (isServer) m_IsButtonPressed = isPlayerOnHandle;
            else CmdUpdateHandleState(isPlayerOnHandle);
        }

        [Command]
        private void CmdUpdateHandleState(bool isPlayerOnHandle)
        {
            m_IsButtonPressed = isPlayerOnHandle;
        }

        public void SetInteractive(bool isInteractive)
        {
            if (isServer) RpcSetInteractive(isInteractive);
            else CmdSetInteractive(isInteractive);
        }

        [Command]
        private void CmdSetInteractive(bool isInteractive)
        {
            RpcSetInteractive(isInteractive);
        }

        [ClientRpc]
        private void RpcSetInteractive(bool isInteractive)
        {
            m_InteractableItem.SetInteractive(isInteractive);
        }

        public void Debugger(object log)
        {
            if (_debugger) Debug.Log(log);
        }

        public void SetDebugActive(bool active)
        {
            _debugger = active;
        }
    }
}
