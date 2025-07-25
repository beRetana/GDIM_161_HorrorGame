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
        switch (m_FinalDoor.State)
        {
            case FinalDoor.DoorState.Locked:
                PlayerManager.Instance.GetPlayer(playerID).
                        GetComponent<NetworkPlayerUI>().CancelHoldingUI();
                if (context.InputType != InteractionType.Hold) return;

                m_FinalDoor.EnterAlertState(playerID);
                SetDisplayMessage($"{m_FinalDoor.KeycardMax - m_FinalDoor.KeycardCount} More Keys Needed");
                break;
            case FinalDoor.DoorState.Alert:
                if (context.InputType != InteractionType.Tap) return;
                UnlockingDoor((byte)playerID);
                break;
            case FinalDoor.DoorState.Unlocked:
                Debugger($"Player {playerID} successfully held; Opening Doors");
                PlayerManager.Instance.GetPlayer(playerID).
                    GetComponent<NetworkPlayerUI>().CancelHoldingUI();

                if (context.InputType != InteractionType.Hold) return;
                m_FinalDoor.OpenDoors();
                break;
        }
    }

    private void UnlockingDoor(byte playerID)
    {
        bool hasKeycard = PlayerManager.Instance.GetPlayer(playerID).
            GetComponent<FirstPersonController>().HasKeyCard;
        
        if (!hasKeycard)
        {
            SetDisplayMessage($"You Need A Keycard");
            return;
        }

        if (m_FinalDoor.IsPlayerCheckedIn(playerID))
        {
            SetDisplayMessage($"One Keycard Per Person");
            return;
        }

        Debugger($"Player {playerID} successfully checked in; " +
                 $"Increasing count to {m_FinalDoor.KeycardCount}");

        if (!m_FinalDoor.TryUnlockDoor(playerID)) return;
        
        SetDisplayMessage($"HOLD To Open");
    }
}