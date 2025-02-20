using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController_test : NetworkBehaviour
{
    public float Speed = 0.1f;
    public GameObject PlayerModel;

    // A flag to ensure we only invoke once.
    private bool positionInvoked = false;

    private void Start()
    {
        PlayerModel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (!PlayerModel.activeSelf && !positionInvoked)
            {
                positionInvoked = true;
                // Invoke the ActivatePlayer method after .5 seconds
                Invoke("ActivatePlayer", .5f);
            }

            if (isOwned)
            {
                Movement();
            }
        }
    }

    private void ActivatePlayer()
    {
        SetPosition();
        PlayerModel.SetActive(true);
    }

    private void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-5, 5), 0.8f, Random.Range(7, 15));
    }

    private void Movement()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection);
        transform.position += moveDirection * Speed;
    }
}
