using UnityEngine;

public class MovementBox : MonoBehaviour
{
    private float _movementBoxSpeed = 0.5f;
   
    void Update()
    {   
        MovementCube();
    }

     public void MovementCube()
    {
        transform.Translate(transform.forward* _movementBoxSpeed* Time.deltaTime);
    }
}
