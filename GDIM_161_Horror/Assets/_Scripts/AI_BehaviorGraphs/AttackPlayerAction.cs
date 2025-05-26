using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackPlayer", story: "Enemy attacks [Player]", category: "Action", id: "e6b4af2c023fdb12db722fb12e2e7192")]
public partial class AttackPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    protected override Status OnStart()
    {
        Player.Value.GetComponent<PlayerBase>().DownPlayer();
        return Status.Success;
    }
}

