using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrbScript : MonoBehaviour
{
    public int healthAmount = 10;  // The amount of health the orb gives to the player

    private void OnTriggerEnter2D(Collider2D collision) //this happens twice per orb lol idk why
    {
        // Check if the colliding object is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Try to get the Player script component from the player object
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null)
            {
                // Add XP to the player
                player.AddHealth(healthAmount);

                // Debug log to confirm XP was added
                //Debug.Log($"Player collected health orb. Gained {healthAmount} health.");

                // Destroy the XP orb after collision
                Destroy(gameObject);
            }
            else
            {
                //Debug.LogWarning("Player script not found on the Player object!");
            }
        }
    }
}
