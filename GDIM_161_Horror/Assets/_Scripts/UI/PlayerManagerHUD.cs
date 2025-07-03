using UnityEngine;

public class PlayerManagerHUD : MonoBehaviour
{
    [SerializeField] private GameObject m_InteractionUI;
    [SerializeField] private GameObject m_InGameMenuUI;
    [SerializeField] private GameObject m_EndGameUI;

    private void Start()
    {
        m_InGameMenuUI.SetActive(true);
        m_InteractionUI.SetActive(true);
        m_EndGameUI.SetActive(false);
    }


}
