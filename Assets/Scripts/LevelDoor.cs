using UnityEngine;
using Cainos.PixelArtTopDown_Basic;
using System.Collections;

public class LevelDoor : MonoBehaviour, IInteractable
{
    [Header("Altar Requirements")]
    public PropsAltar[] altars;
    public int requiredLitCount = 2;

    [Header("Teleport Destination")]
    public Transform destination;
    public Vector2 spawnOffset = new Vector2(0f, -1f);
    public int targetLevel = 2;

    [Header("Feedback (optional)")]
    public SpriteRenderer doorRenderer;
    public Color lockedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
    public Color unlockedColor = new Color(1f, 0.9f, 0.3f, 1f);

    private bool isTeleporting = false;

    public FadeImg fader;

    private void Update()
    {
        if (doorRenderer == null) return;
        doorRenderer.color = IsUnlocked() ? unlockedColor : lockedColor;
    }

    public void Interact(PlayerMovement player)
    {
        if (!IsUnlocked())
        {
            Debug.Log("Door is locked — light all altars first.");
            return;
        }
        Teleport(player);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!IsUnlocked()) return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (!isTeleporting)
        if (player != null) StartCoroutine(Teleport(player));;
    }

    private bool IsUnlocked()
    {
        int lit = 0;
        foreach (var altar in altars)
            if (altar != null && altar.IsLit) lit++;
        return lit >= requiredLitCount;
    }

    private IEnumerator Teleport(PlayerMovement player)
    {
        isTeleporting = true;
        yield return StartCoroutine(fader.FadeOut());

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        player.transform.position =
            destination.position + new Vector3(spawnOffset.x, spawnOffset.y, 0f);

        PlayerAudio audio = player.GetComponent<PlayerAudio>();
        if (audio != null)
        {
            audio.currentLevel = targetLevel;
            audio.SwitchLevelMusic(targetLevel);
        }

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(fader.FadeIn());
    }
}