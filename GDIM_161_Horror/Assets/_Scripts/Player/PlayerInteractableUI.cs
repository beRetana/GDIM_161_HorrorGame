using OtherUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractableUI : MonoBehaviour, IDebugger
{
    [Header("Interaction Display")]
    [SerializeField] private Animator m_InteractAnimator;
    [SerializeField] private TextMeshProUGUI m_DisplayText;

    [Space(5f), Header("Hold Interaction Display")]
    [SerializeField] private Animator m_HoldingAnimator;
    [SerializeField] private Slider m_HoldingSlider;

    protected const string FADE = "FADE";
    protected const string LOAD = "LOAD";

    protected bool m_Debug;

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
        m_HoldingAnimator.SetBool(LOAD, true);
    }

    public void CancelHoldingUI()
    {
        m_HoldingAnimator.SetBool(LOAD, false);
        ResetHoldingUI();
    }

    public void ResetHoldingUI()
    {
        Debugger($"Resetting Holding UI");
        m_HoldingSlider.value = 0;
    }

    public void Debugger(object log)
    {
        if (m_Debug) Debug.Log($"[{this.GetType().ToString()}]: {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_Debug = active;
    }
}
