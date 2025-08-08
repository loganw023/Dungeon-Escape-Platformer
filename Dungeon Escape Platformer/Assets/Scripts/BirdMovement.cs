using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float flipInterval = 5f;
    float nextFlipTime;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        transform.localScale = new Vector2(-(Mathf.Sign(myRigidbody.velocity.x)), 1f);
        nextFlipTime = Time.time + flipInterval;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        myRigidbody.velocity = new Vector2(moveSpeed, 0f);

        if (Time.time >= nextFlipTime)
        {
            moveSpeed = -moveSpeed;
            FlipDirection();
            nextFlipTime = Time.time + flipInterval;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
    }

    void FlipDirection()
    {
        transform.localScale = new Vector2((Mathf.Sign(myRigidbody.velocity.x)), 1f);
    }
}
