using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

#if MIRROR
public class PlayerMovementController_test : NetworkBehaviour
#else
public class PlayerMovementController_test : MonoBehaviour
#endif
{
    public float Speed = 0.1f;
    public GameObject PlayerModel;

    private bool positionInvoked = false;

    private bool IsOwned()
    {
#if MIRROR
        return isOwned;
#else
        return true;
#endif
    }

    private void Start()
    {
        if (PlayerModel != null)
            PlayerModel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (PlayerModel != null && !PlayerModel.activeSelf && !positionInvoked)
            {
                positionInvoked = true;
                Invoke("ActivatePlayer", .5f);
            }

            if (IsOwned())
            {
                Movement();
            }
        }
    }

    private void ActivatePlayer()
    {
        SetPosition();
        if (PlayerModel != null)
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
