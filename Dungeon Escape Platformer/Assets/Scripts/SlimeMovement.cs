using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMovement : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] int enemyPoints = 250;
    [SerializeField] public int enemyHealth = 1;
    [SerializeField] AudioClip slimeSFX;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        myRigidbody.velocity = new Vector2(moveSpeed, 0f);
        if (enemyHealth < 1)
        {
            Destroy(gameObject);
            AudioSource.PlayClipAtPoint(slimeSFX, Camera.main.transform.position);
            FindObjectOfType<GameSession>().addScore(enemyPoints);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Enemy" && other.tag != "Player")
        {
            moveSpeed = -moveSpeed;
            FlipDirection();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Destroy(other.gameObject);
            enemyHealth -= 1;
        }
    }

    void FlipDirection()
    {
        transform.localScale = new Vector2(-(Mathf.Sign(myRigidbody.velocity.x)), 1f);
    }
}
