using AI_FSM;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SoundDetector", story: "[Detector] received a [Sound]", category: "Action", id: "111f661ef4ace8185db483640674f5ba")]
public partial class SoundDetectorAction : Action
{
    [SerializeReference] public BlackboardVariable<SoundSensor> Detector;
    [SerializeReference] public BlackboardVariable<Vector3> Sound;

    protected override Status OnStart()
    {
        Sound.Value = Detector.Value.RecentPosition;
        return Status.Success;
    }
}

