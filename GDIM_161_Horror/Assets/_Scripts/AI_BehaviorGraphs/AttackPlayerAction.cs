using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using StarterAssets;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackPlayer", story: "[Enemy] attacks [Player]", category: "Action", id: "e6b4af2c023fdb12db722fb12e2e7192")]
public partial class AttackPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        FirstPersonController firstPersonController = Player.Value.GetComponent<FirstPersonController>();
        MonsterAnimator monsterAnimator = Self.Value.GetComponent<MonsterAnimator>();
        monsterAnimator.TriggerJump();
        firstPersonController.DownPlayer();
        return Status.Success;
    }
}

