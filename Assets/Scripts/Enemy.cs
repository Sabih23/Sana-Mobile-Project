using UnityEngine;

public class Enemy : MonoBehaviour
{
    // [SerializeField] private AudioSource popSound;

    // This script handles what happens when an object (likely an enemy) collides with something in a 2D game.
    public void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player exists in the game.
        if (PlayerLauncher.Instance.player == null)
        {
            return; // If there is no player, exit the function to avoid null reference errors.
        }

        // Check if the collision's impact was strong (magnitude > 2) and if the player has been launched.
        if (collision.relativeVelocity.magnitude > 1 && PlayerLauncher.Instance.player.isLaunched)
        {
            AudioManager.instance.Play("EnemyDeath");
            // If the conditions are met, call the GameManager to destroy this enemy object.
            GameManager.instance.DestroyEnemy(this);
        }
    }

}
