
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class Player : Deathable
{
    [Header("General setup")]
    public Transform groundCheck;
    public Animator animator;
    
    [Header("Player setup")]
    public float groundedMovementAcceleration;
    public float airMovementAcceleration;
    public float maxVelocityX;
    public float jumpForce;
    public float dashCooldown;
    public float dashDistance;
    public float dashDuration;
    public int attackDamage;
    public float attackDelay;
    public float attackRecovery;
    public float attackHitboxDuration;
    public GameObject attackHitbox;
    public float attackDashCancel;
    
    
    [Header("Player movement status")]
    public bool hasControl = false;
    public bool isFacingRight = true;
    public bool willJumpNextFixedFrame = false;
    public bool isDashing = false;
    public bool canDash = true;
    public bool canAttack = true;
    public bool isAttacking = false;
    public bool isOnGround;
    public bool dashGroundReset = true;
    

    public Rigidbody2D rigidBody;
    private Vector2 movement;

    protected override void OnStart()
    {
        base.OnStart();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        bool oldIsOnGround = isOnGround;
        isOnGround = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Floor"));

        if (!oldIsOnGround && isOnGround)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Char_Fall", transform.position);
        }
        
        // Allow to dash if player didn't hit the ground yet after a dash
        if (isOnGround)
        {
            dashGroundReset = true;
        }
        
        animator.SetBool("isOnGround", isOnGround);
        
        if (hasControl && isOnGround && Input.GetKeyDown(KeyCode.Joystick1Button0) && !isDashing)
        {
            willJumpNextFixedFrame = true;
        }

        if (hasControl && isDashInputed() && canDash && !isDashing && dashGroundReset)
        {
            StartCoroutine(Dash());
        }
        
        if (hasControl && Input.GetKeyDown(KeyCode.Joystick1Button2) && canAttack)
        {
            StartCoroutine(Attack());
        }
        
        animator.SetInteger("xVelocity", Mathf.RoundToInt(Mathf.Abs(rigidBody.velocity.x * 100)));

    }

    private void FixedUpdate()
    {
        if (willJumpNextFixedFrame)
        {
            rigidBody.AddForce(new Vector2(0, jumpForce));
            FMODUnity.RuntimeManager.PlayOneShot("event:/Char_Jump", transform.position);
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

    private void FlipCharacter()
    {
        isFacingRight = !isFacingRight;
        float newScaleX = transform.localScale.x * -1;
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
    }

    private IEnumerator Dash()
    {
        dashGroundReset = false;
        rigidBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        isDashing = true;
        canDash = false;
        canAttack = false;
        animator.SetBool("isDashing", isDashing);
        float startX = transform.localPosition.x;
        float endX;
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
            if (lerpIncrement > attackDashCancel)
            {
                canAttack = true;
            }
            yield return null;
        }
        isDashing = false;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    
    private IEnumerator Attack()
    {
        isAttacking = true;
        canDash = false;
        canAttack = false;
        
        FMODUnity.RuntimeManager.PlayOneShot("event:/Char_Attack", transform.position);

        
        animator.SetBool("isAttacking", true);
        yield return null;
        animator.SetBool("isAttacking", false);

        
        yield return new WaitForSeconds(attackDelay);

        attackHitbox.SetActive(true);
        yield return new WaitForSeconds(attackHitboxDuration);
        attackHitbox.SetActive(false);

        yield return new WaitForSeconds(attackRecovery);
        canDash = true;
        canAttack = true;
        isAttacking = false;
    }

    private bool isDashInputed()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button4) 
            || Input.GetKeyDown(KeyCode.Joystick1Button5) 
            || Input.GetAxis("DashLeft") > 0.15f
            || Input.GetAxis("DashRight") > 0.15f)
        {
            return true;
        }
        return false;
    }

    public override void TakeDamage(int amount)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Char_Hit", transform.position);
        if (health - amount <= 0)
        {
            health = 0;
            OnDie();
        }
        else
        {
            health -= amount;
        }
        
        // base.TakeDamage(amount);
    }

    protected override void OnDie()
    {
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != null && other.gameObject.tag == "StartBossTrigger")
        {
            GameManager.instance.LockBossRoom();
            Destroy(other.gameObject);
        }
    }

    public void MakeStepSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Char_Moving", transform.position);
    }
}