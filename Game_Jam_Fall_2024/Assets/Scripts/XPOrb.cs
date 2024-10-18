using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPOrb : MonoBehaviour
{
    public int xpAmount = 10;  // The amount of XP the orb gives to the player

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Try to get the Player script component from the player object
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null)
            {
                // Add XP to the player
                player.AddXP(xpAmount);

                // Debug log to confirm XP was added
                Debug.Log($"Player collected XP orb. Gained {xpAmount} XP.");

                // Destroy the XP orb after collision
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Player script not found on the Player object!");
            }
        }
    }
}
