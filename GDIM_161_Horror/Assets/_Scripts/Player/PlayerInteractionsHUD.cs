using Mirror;
using OtherUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractionsHUD : NetworkBehaviour, IDebugger
{
    [Header("Tap Interaction")]
    [SerializeField] private Animator m_InteractAnimator;
    [SerializeField] private Image m_InteractionIcon;
    [SerializeField] private TextMeshProUGUI m_InteractionText;

    [Space(5f), Header("Hold Interaction")]
    [SerializeField] private Animator m_HoldingAnimator;
    [SerializeField] private Slider m_HoldingSlider;
    [SerializeField] private Image m_HoldingIcon;

    [Space(5f), Header("Interaction Icons")]
    [SerializeField] private Sprite m_KeyboardE;
    [SerializeField] private Sprite m_LeftClick;

    protected const string FADE = "FADE";
    protected const string LOAD = "LOAD";

    protected bool m_Debug;

    private void Start()
    {
        if (!isServer) enabled = false;
        SetIconKeyboardE();
    }

    public void DisplayInteractUI(string name)
    {
        m_InteractAnimator.SetBool(FADE, true);
        m_InteractionText.text = name;
    }

    public void HideInteractUI()
    {
        m_InteractAnimator.SetBool(FADE, false);
        m_InteractionText.text = "";
    }

    public void SetInteractDisplayText(string name)
    {
        m_InteractionText.text = name;
    }

    public void SetIconKeyboardE()
    {
        SetIcons(m_KeyboardE);
    }

    public void SetIconLeftClick()
    {
        SetIcons(m_LeftClick);
    }

    private void SetIcons(Sprite icon)
    {
        m_InteractionIcon.sprite = icon;
        m_HoldingIcon.sprite = icon;
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
