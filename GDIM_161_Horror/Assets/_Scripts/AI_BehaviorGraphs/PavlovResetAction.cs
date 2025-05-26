using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TakePavlovFood", story: "[Enemy] takes food from [Pavlov]", category: "Action", id: "6194597f02e9a04048efcc50e720a895")]
public partial class PavlovResetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<GameObject> Pavlov;

    protected override Status OnStart()
    {
        Pavlov.Value.GetComponent<Pavlov>().TakeFood();
        return Status.Success;
    }
}

