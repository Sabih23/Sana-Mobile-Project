using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    public bool isLaunched =false;

    private void Awake()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
    private void Update()
    {
        if (isLaunched && rb.IsSleeping())
        {

            GameManager.instance.PlayerFinished();
            Destroy(gameObject);
        }


    }
    public void Launch(Vector2 vector)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(vector *5 , ForceMode2D.Impulse);
    }
}