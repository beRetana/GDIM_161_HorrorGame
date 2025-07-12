using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckForClearAttack", story: "[Self] check to attack [Target] in [Range]", category: "Action/Conditional", id: "6b9a31c2b2ff8c2666b595e9d268d2b9")]
public partial class CheckForClearAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Range;

    protected override Status OnStart()
    {
        RaycastHit hit;
        if (!Physics.Raycast(Self.Value.transform.position, Self.Value.transform.forward,
            out hit, Range.Value))
        {
            return Status.Failure;
        }
        
        if (hit.collider.gameObject != Self.Value.gameObject)
            return Status.Failure;

        return Status.Success;
    }
}

