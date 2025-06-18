using FMOD;
using System;
using Unity.Behavior;
using Unity.MLAgents;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindClosestPlayer", story: "Find [Player] closest to [Enemy] in the same floor with tag [PlayerTag]", category: "Action/Find", id: "95a880068c180b09a538303ef461b910")]
public partial class FindClosestPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<string> PlayerTag;
    [SerializeReference] public BlackboardVariable<int> DistanceBuffer;

    protected override Status OnStart()
    {
        if (Enemy.Value == null)
        {
            LogFailure("No agent provided.");
            return Status.Failure;
        }

        Vector3 agentPosition = Enemy.Value.transform.position;

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(PlayerTag.Value);
        float closestDistanceSq = Mathf.Infinity;
        GameObject closestGameObject = null;
        foreach (GameObject gameObject in gameObjects)
        {
            if (agentPosition.y - gameObject.transform.position.y >= DistanceBuffer.Value)
                continue;

            float distanceSq = Vector3.SqrMagnitude(agentPosition - gameObject.transform.position);

            if (closestGameObject == null || distanceSq < closestDistanceSq)
            {
                closestDistanceSq = distanceSq;
                closestGameObject = gameObject;
            }
        }

        Enemy.Value = closestGameObject;
        return Enemy.Value == null ? Status.Failure : Status.Success;
    }
}

