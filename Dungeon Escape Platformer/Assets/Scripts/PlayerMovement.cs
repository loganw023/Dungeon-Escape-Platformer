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


    Animator myAnimator;
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;

    float originalGravity;
    bool isAlive = true;
    float lastFireTime = -Mathf.Infinity;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        originalGravity = myRigidbody.gravityScale;
        myFeetCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (!isAlive)
        {
            return;
        }
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
        if (value.isPressed && myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myRigidbody.velocity += new Vector2(0f, jumpHeight);
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
        if (myRigidbody.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazard")))
        {
            isAlive = false;
            myAnimator.SetTrigger("death");
            myRigidbody.velocity = deathdance;
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
}
