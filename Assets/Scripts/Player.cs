
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class Player : Actor
{
    [Header("General setup")]
    public Transform groundCheck;
    public Animator animator;
    
    [Header("Player setup")]
    public float groundedMovementAcceleration;
    public float airMovementAcceleration;
    public float maxVelocityX;
    public float jumpForce;
    public bool isOnGround;
    public float dashCooldown;
    public float dashDistance;
    public float dashDuration;
    public int maxHealth;
    public int attackDamage;
    
    
    [Header("Player movement status")]
    public bool hasControl = false;
    public bool isFacingRight = true;
    public bool willJumpNextFixedFrame = false;
    public bool isDashing = false;
    public bool canDash = true;
    public bool canAttack = true;

    [Header("Player status")] 
    public int currentHealth;


    private Rigidbody2D rigidBody;
    private Vector2 movement;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    protected override void Shoot()
    {
        
    }

    private void Update()
    {
        isOnGround = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Floor"));
        animator.SetBool("isOnGround", isOnGround);
        
        if (hasControl && isOnGround && Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            willJumpNextFixedFrame = true;
        }

        if (hasControl && (Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.Joystick1Button5)) && canDash && !isDashing)
        {
            isDashing = true;
            animator.SetBool("isDashing", isDashing);
            StartCoroutine(Dash());
        }
        animator.SetFloat("xVelocity", rigidBody.velocity.x);

    }

    private void FixedUpdate()
    {
        if (willJumpNextFixedFrame)
        {
            rigidBody.AddForce(new Vector2(0, jumpForce));
            willJumpNextFixedFrame = false;
            isOnGround = false;
            animator.SetBool("isOnGround", isOnGround);
        }
        
        if (hasControl && !isDashing)
        {
            if (Input.GetAxis("Horizontal") > .15f)
            {
                if (!isFacingRight)
                {
                    FlipCharacter();
                }

                if (rigidBody.velocity.x < maxVelocityX)
                {
                    if (isOnGround)
                    {
                        rigidBody.AddForce(new Vector2(groundedMovementAcceleration, 0));
                    }
                    else
                    {
                        rigidBody.AddForce(new Vector2(airMovementAcceleration, 0));

                    }
                }
                
            }
            else if (Input.GetAxis("Horizontal") < -.15f)
            {
                if (isFacingRight)
                {
                    FlipCharacter();
                }
                if (rigidBody.velocity.x > -1 * maxVelocityX)
                {
                    if (isOnGround)
                    {
                        rigidBody.AddForce(new Vector2(-1 * groundedMovementAcceleration, 0));
                    }
                    else
                    {
                        rigidBody.AddForce(new Vector2(-1 * airMovementAcceleration, 0));
                    }
                }
            }
            else if (isOnGround && !willJumpNextFixedFrame && !isDashing)
            {
                rigidBody.velocity = Vector2.zero;
            }
 
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth - damage < 0)
        {
            currentHealth = 0;
            // TODO : Death
        }
        else
        {
            currentHealth -= damage;
        }
    }

    private void FlipCharacter()
    {
        isFacingRight = !isFacingRight;
        float newScaleX = transform.localScale.x * -1;
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
    }

    private IEnumerator Dash()
    {
        rigidBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        isDashing = true;
        animator.SetBool("isDashing", isDashing);
        float startX = transform.localPosition.x;
        float endX = 0;
        if (isFacingRight)
        {
            endX = startX + dashDistance;
        }
        else
        {
            endX = startX - dashDistance;
        }
        float lerpIncrement = 0;
        while (lerpIncrement < 1)
        {
           rigidBody.MovePosition(new Vector2(Mathf.Lerp(startX, endX, lerpIncrement), rigidBody.position.y));
            lerpIncrement += Time.deltaTime / dashDuration;
            yield return null;
        }
        isDashing = false;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}