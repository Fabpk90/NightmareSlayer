
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

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
    public float attackDistance;
    public int attackDamage;
    public float attackCooldown;
    public float attackDashCancel;
    public float dashInvulnerabilityPercentage;
    public GameObject raycastPosition;
    public GameObject particleHitSwordRight;
    public GameObject particleHitSwordLeft;
    public float invincibilityTimeAfterDamage;
    
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
    public bool hasJustTouchedGround = false;
    public bool isDead = false;

    [Header("Life")] 
    public Image lifeImage;    
    public List<Sprite> lifeSpriteList;

    public Rigidbody2D rigidBody;
    private Vector2 movement;


    private float startTimeBlinking;

    protected override void OnStart()
    {
        base.OnStart();
        rigidBody = GetComponent<Rigidbody2D>();
        lifeImage = GameManager.instance.lifeImage;
        lifeImage.gameObject.SetActive(true);
        lifeSpriteList = GameManager.instance.lifeSpriteList;
    }

    private void Update()
    {
        bool oldIsOnGround = isOnGround;
        isOnGround = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Floor"));

        if (!oldIsOnGround && isOnGround)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Char/Char_Fall", transform.position);
            hasJustTouchedGround = true;
        }
        
        // Allow to dash if player didn't hit the ground yet after a dash
        if (isOnGround)
        {
            dashGroundReset = true;
        }
        
        animator.SetBool("isOnGround", isOnGround);
        
        if (hasControl && isOnGround && IsJumpInputed() && !isDashing)
        {
            willJumpNextFixedFrame = true;
        }

        if (hasControl && IsDashInputed() && canDash && !isDashing && dashGroundReset && !isAttacking)
        {
            dashGroundReset = false;
            rigidBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
            isDashing = true;
            canDash = false;
            animator.SetBool("isDashing", true);
            canTakeDamage = false;
            StartCoroutine(Dash());
        } else if (hasControl && IsAttackInputed() && canAttack && !isAttacking && !isDashing)
        {
            Attack();
        }
        
        animator.SetInteger("xVelocity", Mathf.RoundToInt(Mathf.Abs(rigidBody.velocity.x * 100)));
    }

    private void FixedUpdate()
    {
        if (hasJustTouchedGround)
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            hasJustTouchedGround = false;
        }
        if (willJumpNextFixedFrame)
        {
            rigidBody.AddForce(new Vector2(0, jumpForce));
            FMODUnity.RuntimeManager.PlayOneShot("event:/Char/Char_Jump", transform.position);
            willJumpNextFixedFrame = false;
            isOnGround = false;
            animator.SetBool("isOnGround", isOnGround);
        }
        
        if (hasControl && !isDashing)
        {
            var horizontalAxisInput = MovemementInput();
            if (horizontalAxisInput > .15f)
            {
                if (!isFacingRight)
                {
                    FlipCharacter();
                }

                if (rigidBody.velocity.x < maxVelocityX * Mathf.Abs(horizontalAxisInput))
                {
                    if (isOnGround)
                    {
                        rigidBody.AddForce(new Vector2(groundedMovementAcceleration * Mathf.Sqrt(Mathf.Abs(horizontalAxisInput)), 0));
                    }
                    else
                    {
                        rigidBody.AddForce(new Vector2(airMovementAcceleration * Mathf.Sqrt(Mathf.Abs(horizontalAxisInput)), 0));
                    }
                }
                
            }
            else if (horizontalAxisInput < -.15f)
            {
                if (isFacingRight)
                {
                    FlipCharacter();
                }
                if (rigidBody.velocity.x > -1 * maxVelocityX * Mathf.Abs(horizontalAxisInput))
                {
                    if (isOnGround)
                    {
                        rigidBody.AddForce(new Vector2(-1 * groundedMovementAcceleration * Mathf.Sqrt(Mathf.Abs(horizontalAxisInput)), 0));
                    }
                    else
                    {
                        rigidBody.AddForce(new Vector2(-1 * airMovementAcceleration * Mathf.Sqrt(Mathf.Abs(horizontalAxisInput)), 0));
                    }
                }
            }
            else if (isOnGround && !willJumpNextFixedFrame && !isDashing)
            {
                rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
            }
        }
    }

    private void FlipCharacter()
    {
        isFacingRight = !isFacingRight;
        if (isOnGround)
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }
        float newScaleX = transform.localScale.x * -1;
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
    }

    private IEnumerator Dash()
    {
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
            // if (lerpIncrement > attackDashCancel)
            // {
            //     canAttack = true;
            // }
            if (lerpIncrement > dashInvulnerabilityPercentage)
                {
                    canAttack = true;
                }
            yield return null;
        }
        rigidBody.velocity = Vector2.zero;
        isDashing = false;
        animator.SetBool("isDashing", false);
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        canTakeDamage = true;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    
    private void Attack()
    {
        isAttacking = true;
        canAttack = false;

        
        FMODUnity.RuntimeManager.PlayOneShot("event:/Char/Char_Attack", transform.position);
        animator.SetBool("isAttacking", true);

    }

    private bool IsDashInputed()
    {
        return Input.GetKeyDown(KeyCode.Joystick1Button4) 
               || Input.GetKeyDown(KeyCode.Joystick1Button5) 
               || Input.GetAxis("DashLeft") > 0.15f
               || Input.GetAxis("DashRight") > 0.15f
               || Input.GetKeyDown(KeyCode.Mouse1)
               || Input.GetKeyDown(KeyCode.LeftShift);
    }

    private float MovemementInput()
    {
        if (Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.Q))
        {
            return -1;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            return 1;
        }

        return Input.GetAxis("Horizontal");
    }

    private bool IsAttackInputed()
    {
        return Input.GetKeyDown(KeyCode.Joystick1Button2)
               || Input.GetKeyDown(KeyCode.Mouse0);
    }

    private bool IsJumpInputed()
    {
        return Input.GetKeyDown(KeyCode.Joystick1Button0)
               || Input.GetKeyDown(KeyCode.Space)
               || Input.GetKeyDown(KeyCode.Z)
               || Input.GetKeyDown(KeyCode.W);
    }

    public override void TakeDamage(int amount)
    {
        if (canTakeDamage)
        {
            StartCoroutine(BlinkingDamage());
            FMODUnity.RuntimeManager.PlayOneShot("event:/Char/Char_Hit", transform.position);
            if (health - amount <= 0)
            {
                OnDie();
            }
            else
            {
                health -= amount;
                var ratio = Mathf.FloorToInt(lifeSpriteList.Count - 1 - (lifeSpriteList.Count -1) * health / maxHealth);
                lifeImage.sprite = lifeSpriteList[ratio];
            }
        }
        
    }

    IEnumerator BlinkingDamage()
    {
        startTimeBlinking = Time.time;
        canTakeDamage = false;

        while (Time.time - startTimeBlinking < invincibilityTimeAfterDamage)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.1f);
            GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        canTakeDamage = true;
    }

    protected override void OnDie()
    {
        isDead = true;
        lifeImage.sprite = lifeSpriteList[14];
        health = 0;
        animator.SetBool("isDead", true);
        hasControl = false;
    }
    
    public void DeathAnimationEnd()
    {
        GameManager.instance.RetryBoss();
    }

    public void MakeStepSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Char/Char_Moving", transform.position);
    }
    
    public void MakeDashSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Char/Char_Dash", transform.position);
    }

    public void AttackAnimation()
    {
        animator.SetBool("isAttacking", false);

        RaycastHit2D[] hits;

        hits = Physics2D.RaycastAll(raycastPosition.transform.position
            , Vector2.right * (isFacingRight ? 1 : -1), attackDistance);

        foreach (var hit in hits)
        {
            var Boss = hit.transform.GetComponent<Boss>();
            if (Boss)
            {
                var leftrightParticle = isFacingRight ? particleHitSwordRight : particleHitSwordLeft;
                var particle = Instantiate(leftrightParticle, new Vector3(hit.point.x, hit.point.y, 0),
                    Quaternion.identity);
                particle.transform.localScale = new Vector3(particle.transform.localScale.x,
                    particle.transform.localScale.y, particle.transform.localScale.z);
                Destroy(particle,1f);
                Boss.TakeDamage(attackDamage);
            }
        }
    }

    public void StopAttackAnimation()
    {
        // attackHitbox.SetActive(false);
        foreach (var go in GameObject.FindGameObjectsWithTag("Hitbox"))
        {
            Destroy(go);
        }
        isAttacking = false;
        Invoke(nameof(AttackCooldown), attackCooldown);
    }

    private void AttackCooldown()
    {
        canAttack = true;
    }
    
    
}