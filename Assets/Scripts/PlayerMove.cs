using UnityEngine;
using System.Collections;

public abstract class PlayerMove : MonoBehaviour
{
    public MoveSlotType slotType;
    public float cooldown;
    protected float cooldownRemaining = 0f;
    public float manaCost;
    public float spawnDistance = 0.8f; 

    public GameObject pickupPrefab;
    protected PlayerMovement player;

    private void Start()
    {
        player = PlayerMovement.Instance;
    }

    void Update()
    {
        if (cooldownRemaining > 0)
        {
            cooldownRemaining -= Time.deltaTime;

            if (cooldownRemaining < 0)
                cooldownRemaining = 0;
        }
    }

    public abstract IEnumerator Execute();

    public float CooldownRemaining()
    {
        return cooldownRemaining;
    }

    public bool IsOnCooldown()
    {
        return cooldownRemaining > 0;
    }

    public float getManaCost()
    {
        return manaCost;
    }
}
