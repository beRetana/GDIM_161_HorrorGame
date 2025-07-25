using Mirror;
using StarterAssets;
using System.Linq;
using UnityEngine;

public class FinalDoorInteractable : InteractableItem
{
    private FinalDoor m_FinalDoor;

    protected override void Start()
    {
        base.Start();
        m_FinalDoor = GetComponent<FinalDoor>();
    }

    public override void StoppedDetecting(int playerID)
    {
        base.StoppedDetecting(playerID);
        m_FinalDoor.OnCancelledInteraction(playerID);
    }

    public override void StartedInteraction(int playerID, InputData context)
    {
        m_FinalDoor.OnStartedInteraction(playerID);
    }

    public override void CanceledInteraction(int playerID, InputData context)
    {
        if (context.InputType != InteractionType.Hold) return;
        m_FinalDoor.OnCancelledInteraction(playerID);
    }

    public override void PerformedInteraction(int playerID, InputData context)
    {
        m_FinalDoor.OnPerformedInput(playerID, context);
    }
}