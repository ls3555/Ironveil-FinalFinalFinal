using UnityEngine;
using System.Collections;

public abstract class PlayerMove : MonoBehaviour
{
    public MoveSlotType slotType;
    public float cooldown;
    public float manaCost;
    public float spawnDistance = 0.8f; 
    protected PlayerMovement player;
    protected float cooldownRemaining;
    protected float cooldownDuration = 1f;
    public Sprite abilityIcon;
    [SerializeField] public GameObject pickupPrefab;



private void Update()
    {
        if (cooldownRemaining > 0)
            cooldownRemaining -= Time.deltaTime;

    }

private void Start()
    {
        player = PlayerMovement.Instance;
    }

    public abstract IEnumerator Execute();

    public bool IsOnCooldown()
{
    return cooldownRemaining > 0f;
}

public float CooldownRemaining()
{
    return cooldownRemaining;
}

public Sprite getIcon()
{
    return abilityIcon; 
}

public float getManaCost()
{
    return manaCost; 
}
}
