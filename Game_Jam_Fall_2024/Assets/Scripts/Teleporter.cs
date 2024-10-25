using System.Collections;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform destination; // Destination point for teleportation
    public float teleportDelay = 0.1f; // Delay before teleportation
    public bool spawnToLeft = true; // Flag to determine whether to spawn the player to the left or right of the destination object
    private CapsuleCollider2D destinationCollider; // Reference to the destination teleporter's collider
    private bool isTeleporting = false; // Flag to track whether teleportation is in progress

    AudioSource teleportationSound;

    private void Start()
    {
        // Get the destination teleporter's collider
        destinationCollider = destination.GetComponent<CapsuleCollider2D>();
        //teleportationSound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTeleporting)
        {
            //teleportationSound.Play();
            StartCoroutine(TeleportPlayer(collision.transform));
        }
    }

    IEnumerator TeleportPlayer(Transform player)
    {
        // Set the teleportation flag to true to prevent re-entry
        isTeleporting = true;

       

        // Disable the collider of the destination teleporter to prevent immediate re-entry
        destinationCollider.enabled = false;

        // Calculate the teleportation offset based on the spawnToLeft flag
        float xOffset = spawnToLeft ? -5f : 5f;
        Vector3 teleportOffset = new Vector3(xOffset, 0f, 0f);

        // Teleport the player to the destination point with the offset
        player.position = destination.position + teleportOffset;

        // Re-enable the collider of the destination teleporter after teleportation
        yield return new WaitForSeconds(teleportDelay);
        destinationCollider.enabled = true;

        // Reset the teleportation flag
        isTeleporting = false;

    
    }
}
