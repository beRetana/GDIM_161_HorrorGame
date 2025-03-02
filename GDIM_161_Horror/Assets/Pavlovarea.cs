using UnityEngine;

public class Pavlovarea : MonoBehaviour
{
    private bool ismonsterinarea= false;
    public bool Ismonsterinarea{
        get { return ismonsterinarea;}
        set{ismonsterinarea = value;}
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Ensure the player GameObject has the "Player" tag
        {
            ismonsterinarea= true;
            Debug.Log("Player is in!");
        }
    }
    

}
