using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;
    private int leftExtreme = -15;
    public bool isLaunched =false;
    private float destroyDelay = 5f;

    private void Awake()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
    private void Update()
    {
        if (isLaunched && (rb.IsSleeping() || rb.velocity.magnitude < 0.1f || transform.position.x > 150))
        {

            GameManager.instance.PlayerFinished();
            Destroy(gameObject);
        }

        OnMoveLeft();

    }
    public void Launch(Vector2 vector)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(vector *5 , ForceMode2D.Impulse);
        StartCoroutine(DestroyAfterDelay(destroyDelay));
    }

    //destroy player when it moves out of left side of the screen
    private void OnMoveLeft()
    {
        if(transform.position.x < leftExtreme)
        {
            GameManager.instance.PlayerFinished();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Checks if the ball touches the ground and is in slow speed or it touches the ground behind the obstacles
        if ((collision.gameObject.CompareTag("ground") && rb.velocity.magnitude < 0.05f) || (collision.gameObject.CompareTag("ground") && transform.position.x > 100) || (collision.gameObject.CompareTag("ground") && transform.position.x < 7))
        {
            // Handle collision with an object tagged as "Obstacle"
            Debug.Log("Collided with an obstacle!");
            GameManager.instance.PlayerFinished();
            Destroy(gameObject);
            // Implement your collision handling logic here
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isLaunched) // Ensure player is still launched before destroying
        {
            GameManager.instance.PlayerFinished();
            Destroy(gameObject);
        }
    }
}