using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentML : Agent
{
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private GameObject playerDummy;
    [SerializeField] private TrainningVisuals trainningVisuals;

    private AgentController agentController;

    void Start()
    {
        agentController = GetComponent<AgentController>();
    }

    public override void OnEpisodeBegin()
    {
        agentController.ResetAgent();
        float xDelimeter = 6.5f;
        float zDelimeter = 6.5f;
        float randomX = Random.Range(-xDelimeter, xDelimeter);
        float randomZ = Random.Range(-zDelimeter, zDelimeter);
        playerDummy.transform.localPosition = new Vector3(randomX, 0, randomZ);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Mimic training or just testing the agent
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(agentController.Rb.transform);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float rotation = actions.ContinuousActions[1];
        agentController.Move(move);
        agentController.Rotate(rotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(wallTag))
        {
            // Just visuals
            trainningVisuals.SetFloorColor(Color.red);

            // Logic penalty for behavior
            SetReward(-1f);
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag(playerTag))
        {
            // Just visuals
            trainningVisuals.SetFloorColor(Color.green);

            // Logic reinforce behavior
            SetReward(1f);
            EndEpisode();
        }
    }

}
