using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using StarterAssets;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackPlayer", story: "Enemy attacks [Player]", category: "Action", id: "e6b4af2c023fdb12db722fb12e2e7192")]
public partial class AttackPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    protected override Status OnStart()
    {
        FirstPersonController firstPersonController = Player.Value.GetComponent<FirstPersonController>();
        firstPersonController.DownPlayer();
        firstPersonController.SetInputState(false);
        return Status.Success;
    }
}

