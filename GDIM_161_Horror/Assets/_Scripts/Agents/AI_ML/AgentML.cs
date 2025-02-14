using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentML : Agent
{
    [SerializeField] private string _wallTag = "Wall";
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private GameObject _playerDummy;
    [SerializeField] private TrainningVisuals _trainingVisuals;

    private float _maxSteps;
    private float _commulativeReward;
    private float _reward;
    private float _episodeCount;

    private AgentController _agentController;

    void Start()
    {
        _agentController = GetComponent<AgentController>();
    }

    public override void Initialize()
    {
        base.Initialize();

        _maxSteps = MaxStep;

    }

    /// <summary>
    /// Reset the agent and the player dummy position to a random one.
    /// </summary>
    public override void OnEpisodeBegin()
    {
        _agentController.ResetAgent();
        float xDelimeter = 6.5f;
        float zDelimeter = 6.5f;
        float randomX = Random.Range(-xDelimeter, xDelimeter);
        float randomZ = Random.Range(-zDelimeter, zDelimeter);
        _playerDummy.transform.localPosition = new Vector3(randomX, 0, randomZ);
        _commulativeReward = 0;
        _episodeCount += 1;
    }

    /// <summary>
    /// Heuristic to mimic training or just testing the agent.
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Mimic training or just testing the agent
        var continiousActionsOut = actionsOut.ContinuousActions;
        continiousActionsOut[0] = Input.GetAxis("Vertical");
        continiousActionsOut[1] = Input.GetAxis("Horizontal");
    }

    /// <summary>
    /// Collect the observations of the agent.
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_agentController.Rb.transform);
    }

    /// <summary>
    /// Receive the actions from the agent/model.
    /// </summary>
    /// <param name="actions"></param>
    public override void OnActionReceived(ActionBuffers actions)
    {
        _agentController.Move(actions.ContinuousActions[0]);
        _agentController.Rotate(actions.ContinuousActions[1]);

        AddReward(-2 / _maxSteps);
        Debug.Log($"Reward:{GetCumulativeReward()}");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(_wallTag))
        {
            // Just visuals
            _trainingVisuals.SetFloorColor(Color.red);

            // Logic penalty for behavior
            AddReward(-1f);
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag(_playerTag))
        {
            // Just visuals
            _trainingVisuals.SetFloorColor(Color.green);

            // Logic reinforce behavior
            AddReward(5f);
            EndEpisode();
        }
    }

}
