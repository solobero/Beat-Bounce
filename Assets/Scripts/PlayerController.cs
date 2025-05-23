using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    public float jumpForce = 10f;
    bool canJump;
    private StandaloneRhythmSystem rhythmSystem;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        rhythmSystem = FindObjectOfType<StandaloneRhythmSystem>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && canJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            canJump = true;
        }
        
        if(collision.gameObject.tag == "Obstacle")
        {
            GameOver();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            canJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Obstacle")
        {
           GameOver();
        }
    }
    
    private void GameOver()
    {
        Debug.Log("¡Colisión con obstáculo! Mostrando pantalla de Game Over...");
        
        if (rhythmSystem != null)
        {
            rhythmSystem.StopGame();
        }
        
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ShowGameOver();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}