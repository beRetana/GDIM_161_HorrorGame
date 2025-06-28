using UnityEngine;

public class FinalDoor : InteractableItem
{
    [SerializeField] protected int m_KeycardCount = 4;
    [SerializeField] protected string m_WarningText = "4 KeyCards Needed";

    private DoorState m_DoorState;
    private enum DoorState
    {
        Locked,
        Alert,
        Unlocked
    }

    protected override void Start()
    {
        base.Start();
        m_DoorState = DoorState.Locked;
    }

    public override void StartedInteraction(int playerID, InputData data)
    {
        if (m_DoorState != DoorState.Locked) return;

    }

    public override void CanceledInteraction(int playerID, InputData context)
    {
        
    }

    public override void PerformedInteraction(int playerID, InputData context)
    {
        base.PerformedInteraction(playerID, context);
    }
}