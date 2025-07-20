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

    private int ground = 1 << 3;
    private int wall = 1 << 8;
    private int door = 1 << 11;

    protected override Status OnStart()
    {
        bool isTargetInRange = (Target.Value.transform.position - Self.Value.transform.position).sqrMagnitude < Range.Value * Range.Value;
        if (!isTargetInRange) return Status.Failure;
        Debug.Log("Target is in range");
        int obstacles = ground | wall | door;
        bool isTargetObstructed = Physics.SphereCast(Self.Value.transform.position + Vector3.up, 0.5f, (Target.Value.transform.position - Self.Value.transform.position), out _,Range.Value, obstacles);
        
        if (isTargetObstructed) return Status.Failure;
        Debug.Log("Target is in clear");
        return Status.Success;
    }
}

