using TMPro;
using UnityEngine;

public class PlayerInteractableUI : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private TextMeshProUGUI m_DisplayText;

    private const string FADE = "FADE";

    public void DisplayInteractUI(string name)
    {
        m_Animator.SetBool(FADE, true);
        m_DisplayText.text = name;
    }

    public void HideInteractUI()
    {
        m_Animator.SetBool(FADE, false);
        m_DisplayText.text = "";
    }

    public void SetDisplayText(string name)
    {
        m_DisplayText.text = name;
    }
}
