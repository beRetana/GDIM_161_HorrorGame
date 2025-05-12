using System;
using AI_FSM;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SightDetection", story: "Check if [AISightSensor] has dectected [Target]", category: "Action/Senses", id: "9850a59db79825cb388e547e987a92cf")]
public partial class SightDetectionAction : Action
{
    [SerializeReference] public BlackboardVariable<AISightSensor> AISightSensor;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    protected override Status OnStart()
    {
        if (AISightSensor.Value.TargetDetected != null)
        {
            Target.Value = AISightSensor.Value.TargetDetected;
            return Status.Success;
        }
        return Status.Failure;
    }
}

