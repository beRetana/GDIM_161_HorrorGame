using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Mirror;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "NotifyPavlov", story: "[Enemy] connects to [Pavlov]", category: "Action", id: "6ecdf7575415074d9e67cc4ba0103fe6")]
public partial class NotifyPavlovAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<GameObject> Pavlov;

    protected override Status OnStart()
    {
        Pavlov.Value.GetComponent<Pavlov>().ConnectEnemyToPavlov(Enemy.Value.GetComponent<NetworkIdentity>());
        return Status.Success;
    }
}

