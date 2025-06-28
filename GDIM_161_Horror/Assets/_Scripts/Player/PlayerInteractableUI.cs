using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractableUI : MonoBehaviour
{
    [Header("Interaction Display")]
    [SerializeField] private Animator m_InteractAnimator;
    [SerializeField] private TextMeshProUGUI m_DisplayText;

    [Space(5f), Header("Hold Interaction Display")]
    [SerializeField] private Animator m_HoldingAnimator;
    [SerializeField] private Slider m_HoldingSlider;

    private const string FADE = "FADE";

    protected const string START = "START_LOADING";
    protected const string STOP = "STOP_LOADING";

    public void DisplayInteractUI(string name)
    {
        m_InteractAnimator.SetBool(FADE, true);
        m_DisplayText.text = name;
    }

    public void HideInteractUI()
    {
        m_InteractAnimator.SetBool(FADE, false);
        m_DisplayText.text = "";
    }

    public void SetInteractDisplayText(string name)
    {
        m_DisplayText.text = name;
    }

    public void StartHoldingUI()
    {
        m_HoldingAnimator.SetTrigger(START);
    }

    public void CancelHoldingUI()
    {
        m_HoldingAnimator.SetTrigger(STOP);
        ResetHoldingUI();
    }

    public void ResetHoldingUI()
    {
        m_HoldingAnimator.ResetTrigger(START);
        m_HoldingAnimator.ResetTrigger(STOP);
        m_HoldingSlider.value = 0;
    }
}
