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

    private const float RADIUS = 1f;
    private int player = 1 << 10;
    private int ground = 1 << 3;
    private int wall = 1 << 8;
    private int door = 1 << 11;

    protected override Status OnStart()
    {
        int detectables = player | ground | wall | door;
        RaycastHit hit;
        if (!Physics.SphereCast(Self.Value.transform.position + Vector3.up , RADIUS,
            Self.Value.transform.forward, out hit, Range.Value, detectables))
        {
            return Status.Failure;
        }
        
        if (hit.collider.transform.root.gameObject != Target.Value.gameObject)
        {
            Debug.Log($"Collided against {hit.collider.transform.root.gameObject}, " +
                      $"current target is {Target.Value.gameObject}");
            return Status.Failure;
        }

        return Status.Success;
    }
}

