using UnityEngine;

public class LoadingBarUI : MonoBehaviour
{
    [SerializeField] private PlayerInteractionsHUD m_InteractableUI;

    private void ResetHoldingUI()
    {
        m_InteractableUI?.ResetHoldingUI();
    }
}
