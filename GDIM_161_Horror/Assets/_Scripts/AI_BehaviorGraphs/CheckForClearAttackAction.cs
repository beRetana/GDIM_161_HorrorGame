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
        float distSqr = (Target.Value.transform.position - Self.Value.transform.position).sqrMagnitude;
        Debug.Log($"Target: {Target.Value.gameObject} and Self: {Self.Value.gameObject}");
        Debug.Log($"Distance: {distSqr} needed {Range.Value * Range.Value}");
        if (!(distSqr < Range.Value * Range.Value)) return Status.Failure;
        Debug.Log("Target is in range");
        if (distSqr <= 1f) return Status.Success;
        Debug.Log("Target is +1 of distance");
        int detectables = ground | wall | door | player;

        RaycastHit hit;
        if (!Physics.SphereCast(Self.Value.transform.position + Vector3.up, 0.5f, (Target.Value.transform.position - Self.Value.transform.position), out hit, Range.Value, detectables)) return Status.Failure;
        
        bool isTargetObstructed = hit.collider.transform.root != Target.Value.transform;
        
        if (isTargetObstructed) return Status.Failure;
        
        Debug.Log("Target is in clear");
        return Status.Success;
    }
}

