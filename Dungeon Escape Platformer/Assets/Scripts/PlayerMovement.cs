using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathdance = new Vector2(5f, 5f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    [SerializeField] float fireCooldown = 1f;
    [SerializeField] AudioClip soundtrack;
    [SerializeField] float dashSpeed = 2f;
    [SerializeField] float dashDuration = 0.5f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] int maxJumps = 1;
    int jumpsRemaining;

    Animator myAnimator;
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;

    float originalGravity;
    bool isAlive = true;
    float lastFireTime = -Mathf.Infinity;
    float dashTimeRemaining = 0f;
    float lastDashTime = -Mathf.Infinity;
    bool isDashing = false;
    bool wasGrounded = false;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        originalGravity = myRigidbody.gravityScale;
        myFeetCollider = GetComponent<BoxCollider2D>();
        AudioSource.PlayClipAtPoint(soundtrack, Camera.main.transform.position);
        jumpsRemaining = maxJumps - 1;
    }

    void Update()
    {
        if (!isAlive)
        {
            return;
        }
        if (isDashing)
        {
            float direction = transform.localScale.x;
            myRigidbody.velocity = new Vector2(direction * dashSpeed, 0f);
            dashTimeRemaining -= Time.deltaTime;

            if (dashTimeRemaining <= 0f)
            {
                isDashing = false;
                myAnimator.SetBool("isDashing", false);
            }
            return;
        }

        bool isGrounded = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (GameSession.hasDoubleJump)
        {
            maxJumps = 2;
        }
        if (isGrounded && !wasGrounded)
        {
            jumpsRemaining = maxJumps;
        }
        wasGrounded = isGrounded;
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive)
        {
            return;
        }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive)
        {
            return;
        }
        if (value.isPressed && jumpsRemaining > 0)
        {
            myRigidbody.velocity += new Vector2(0f, jumpHeight);
            jumpsRemaining--;
        }
    }

    void Run()
    {
        if (!isAlive)
        {
            return;
        }
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerMoving = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerMoving)
        {
            myAnimator.SetBool("isRunning", true);
        }
        else
        {
            myAnimator.SetBool("isRunning", false);
        }

    }

    void FlipSprite()
    {
        if (!isAlive)
        {
            return;
        }
        bool playerMoving = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerMoving)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }

    }

    void ClimbLadder()
    {
        if (!isAlive)
        {
            return;
        }
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            myRigidbody.gravityScale = 0f;
            myAnimator.SetBool("isClimbing", true);
            Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
            myRigidbody.velocity = climbVelocity;
        }
        else
        {
            myRigidbody.gravityScale = originalGravity;
            myAnimator.SetBool("isClimbing", false);
        }
    }

    void Die()
    {
        if (myRigidbody.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazard")) && isDashing == false)
        {
            isAlive = false;
            myAnimator.SetTrigger("death");
            myRigidbody.velocity = deathdance;
            FindObjectOfType<GameSession>().PlayerDeath();
        }
    }

    void OnFire(InputValue value)
    {
        if (!isAlive)
        {
            return;
        }

        if (Time.time - lastFireTime >= fireCooldown)
        {
            Instantiate(bullet, gun.position, transform.rotation);
            lastFireTime = Time.time;
        }
    }

    void OnDash(InputValue value)
    {
        if (!isAlive || isDashing)
        {
            return;
        }
        else if (Time.time - lastDashTime >= dashCooldown && GameSession.hasDash)
        {
            isDashing = true;
            myAnimator.SetBool("isDashing", true);
            dashTimeRemaining = dashDuration;
            lastDashTime = Time.time;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Dash")
        {
            FindObjectOfType<GameSession>().collectDash();
            Destroy(other.gameObject);
        }
        if (other.tag == "DoubleJump")
        {
            FindObjectOfType<GameSession>().collectDoubleJump();
            Destroy(other.gameObject);
        }
    }
}
