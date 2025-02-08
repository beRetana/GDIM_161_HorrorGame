using UnityEngine;
using Mirror;

public class AgentController : MonoBehaviour
{
    [Header("Agent Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float leapForce = 10f;
    [SerializeField] float rotationSpeed = 5f;

    private Rigidbody rb;

    public Rigidbody Rb { get => rb; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ResetAgent()
    {
        // Reset the agent's position
        transform.localPosition = Vector3.zero;
        // Reset the agent's velocity
        rb.linearVelocity = Vector3.zero;
        // Reset the agent's rotation
        transform.rotation = Quaternion.identity;
    }

    public void Move(float move)
    {
        rb.linearVelocity = transform.forward * Mathf.Abs(move) * moveSpeed;
    }

    public void Rotate(float rotate)
    {
        transform.Rotate(0, rotate * rotationSpeed, 0, Space.Self);
    }

    // Implement leap and speed up later

}
