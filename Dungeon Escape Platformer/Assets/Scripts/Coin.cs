using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] AudioClip coinSFX;
    [SerializeField] int coinPoints = 100;
    bool collected = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && collected == false)
        {
            FindObjectOfType<GameSession>().addScore(coinPoints);
            AudioSource.PlayClipAtPoint(coinSFX, Camera.main.transform.position);
            Destroy(gameObject);
            collected = true;
            gameObject.SetActive(false);
        }
    }
}