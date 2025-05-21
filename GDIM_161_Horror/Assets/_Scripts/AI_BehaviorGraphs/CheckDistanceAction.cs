using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckDistance", story: "Is [Target] close to [Self]", category: "Action/Navigation", id: "57239a887e748c5b1229dca8447bbcfb")]
public partial class CheckDistanceAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Distance = new BlackboardVariable<float>(20f);

    protected override Status OnStart()
    {
        return DistanceCheck() ? Status.Success : Status.Failure;
    }

    private bool DistanceCheck()
    {
        return Mathf.Pow(Distance.Value,2 ) <= (Target.Value.transform.position - Self.Value.transform.position).sqrMagnitude;
    }
}

