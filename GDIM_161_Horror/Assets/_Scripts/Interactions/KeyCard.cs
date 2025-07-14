using StarterAssets;
using UnityEngine;

public class KeyCard : InteractableItem
{
    [SerializeField] private Transform m_Visuals;
    [SerializeField] private string m_WarningMessage;
    [SerializeField] private LayerMask m_LayerMask;

    protected override void Start()
    {
        base.Start();
        FinalDoor.Instance.OnDoorEnteredAlert += ChangeToAlertMode;
    }

    protected virtual void ChangeToAlertMode()
    {
        m_Visuals.gameObject.layer = m_LayerMask;
    }

    public override void PerformedInteraction(int playerID, InputData data)
    {
        FirstPersonController controller = (PlayerManager.Instance.GetPlayer(playerID) as FirstPersonController);
        if (controller.HasKeyCard) return;
        controller.HasKeyCard = true;
        gameObject.SetActive(false);
    }

    public override void Detected(int playerID)
    {
        if (!m_IsInteractable) return;
        FirstPersonController controller = (PlayerManager.Instance.GetPlayer(playerID) as FirstPersonController);
        PlayerInteractionsHUD interactable = controller.GetComponent<PlayerInteractionsHUD>();
        if (controller.HasKeyCard) interactable.DisplayInteractUI(m_WarningMessage);
        else interactable.DisplayInteractUI(m_DisplayText);
    }
}
