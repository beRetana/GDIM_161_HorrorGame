
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindClosestInFloor", story: "Find [Target] closest to [Agent] in the same floor with tag [Tag]", category: "Action/Find", id: "95a880068c180b09a538303ef461b910")]
public partial class FindClosestInFloorAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<string> Tag;
    [SerializeReference] public BlackboardVariable<int> DistanceBuffer;

    protected override Status OnStart()
    {
        if (Agent.Value == null)
        {
            LogFailure("No agent provided.");
            return Status.Failure;
        }

        Vector3 agentPosition = Agent.Value.transform.position;

        GameObject[] playersFound = GameObject.FindGameObjectsWithTag(Tag.Value);
        float closestDistanceSq = Mathf.Infinity;
        GameObject closestPlayer = null;
        foreach (GameObject player in playersFound)
        {
            //Debug.Log($" FIND_CLOSEST_PLAYER {agentPosition.y - player.transform.position.y}");
            if (Mathf.Abs(agentPosition.y - player.transform.position.y) >= DistanceBuffer.Value)
                continue;

            float distanceSq = Vector3.SqrMagnitude(agentPosition - player.transform.position);

            if (closestPlayer == null || distanceSq < closestDistanceSq)
            {
                closestDistanceSq = distanceSq;
                closestPlayer = player;
            }
        }

        Target.Value = closestPlayer;
        return Target.Value == null ? Status.Failure : Status.Success;
    }
}

