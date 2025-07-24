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
    private int player = 1 << 10;

    protected override Status OnStart()
    {
        if (Target.Value == null || Self.Value == null) return Status.Failure;

        float distSqr = (Target.Value.transform.position - Self.Value.transform.position).sqrMagnitude;
        if (!(distSqr < Range.Value * Range.Value)) return Status.Failure;
        if (distSqr <= 1f) return Status.Success;
        int detectables = ground | wall | door | player;

        RaycastHit hit;
        if (!Physics.SphereCast(Self.Value.transform.position + Vector3.up, 0.5f, (Target.Value.transform.position - Self.Value.transform.position), out hit, Range.Value, detectables)) return Status.Failure;
        
        bool isTargetObstructed = hit.collider.transform.root.tag != Target.Value.transform.tag;
        
        if (isTargetObstructed) return Status.Failure;
        
        return Status.Success;
    }
}

