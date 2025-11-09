using UnityEngine;

public class MovementBox : MonoBehaviour
{
    private float _movementBoxSpeed = 0.5f;
   

    // Update is called once per frame
    void Update()
    {   MovementCube();
        
    }

     public void MovementCube()
    {
        transform.Translate(transform.forward* _movementBoxSpeed* Time.deltaTime);

        
    }
}
