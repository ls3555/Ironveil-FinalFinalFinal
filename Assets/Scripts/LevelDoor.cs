using UnityEngine;
using Cainos.PixelArtTopDown_Basic;

public class LevelDoor : MonoBehaviour, IInteractable
{
    [Header("Altar Requirements")]
    public PropsAltar[] altars;          // drag your 2 altars here
    public int requiredLitCount = 2;

    [Header("Teleport Destination")]
    public Transform destination;        // assign a Transform in the target location

    [Header("Feedback (optional)")]
    public SpriteRenderer doorRenderer;
    public Color lockedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
    public Color unlockedColor = new Color(1f, 0.9f, 0.3f, 1f);

    private void Update()
    {
        if (doorRenderer == null) return;
        doorRenderer.color = IsUnlocked() ? unlockedColor : lockedColor;
    }

    // Called by PlayerMovement.TryInteract()
    public void Interact(PlayerMovement player)
    {
        if (!IsUnlocked())
        {
            Debug.Log("Door is locked — light all altars first.");
            return;
        }

        Teleport(player);
    }

    // Also works as a walk-in trigger if you prefer no button press
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!IsUnlocked()) return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null) Teleport(player);
    }

    private bool IsUnlocked()
    {
        int lit = 0;
        foreach (var altar in altars)
            if (altar != null && altar.IsLit) lit++;

        return lit >= requiredLitCount;
    }

    private void Teleport(PlayerMovement player)
    {
        if (destination == null)
        {
            Debug.LogWarning("LevelDoor: No destination assigned!");
            return;
        }

        // Disable rigidbody interpolation briefly to avoid position lag
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        player.transform.position = destination.position;
        Debug.Log("Player teleported to " + destination.name);
    }
}