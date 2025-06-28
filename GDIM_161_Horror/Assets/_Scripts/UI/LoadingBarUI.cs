using UnityEngine;

public class LoadingBarUI : MonoBehaviour
{
    [SerializeField] private PlayerInteractableUI m_InteractableUI;

    private void ResetHoldingUI()
    {
        m_InteractableUI?.ResetHoldingUI();
    }
}
