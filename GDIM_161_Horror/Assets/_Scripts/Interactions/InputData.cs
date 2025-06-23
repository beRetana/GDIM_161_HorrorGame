using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public enum InteractionType
{
    Tap = 0,
    Hold = 1,
    Other = 2
}

public struct InputData
{
    public InputData(InputAction.CallbackContext context)
    {
        InputPhase = context.phase;
        if (context.interaction is HoldInteraction) InputType = InteractionType.Hold;
        else if (context.interaction is TapInteraction) InputType = InteractionType.Tap;
        else InputType = InteractionType.Other;
    }

    public InputData(InputActionPhase phase, InteractionType interaction)
    {
        InputPhase = phase;
        InputType = interaction;
    }

    public InputActionPhase InputPhase;
    public InteractionType InputType;
}
