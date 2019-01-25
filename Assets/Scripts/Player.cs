
using UnityEngine;

public class Player : MonoBehaviour
{
    public float movementSpeed;

    private Rigidbody2D rigidBody;

    private Vector2 movement;

    private bool isOnGround;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            print("Entering ground");
            
            isOnGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            print("Leaving ground");
            
            isOnGround = false;
        }
    }

    private void Update()
    {
        movement = Vector2.zero;
        
        if (Input.GetAxis("Horizontal") > .15f)
        {
            movement.x += movementSpeed;
        }
        else if (Input.GetAxis("Horizontal") < -.15f)
        {
            movement.x -= movementSpeed;
        }

        if (isOnGround)
        {
            if (movement == Vector2.zero)
                rigidBody.velocity = Vector2.zero;
            else
                rigidBody.AddForce(movement);
        }
        else
            rigidBody.AddForce(movement);
    }
}