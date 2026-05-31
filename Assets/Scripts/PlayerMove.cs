using UnityEngine;
using System.Collections;

public abstract class PlayerMove : MonoBehaviour
{
    public MoveSlotType slotType;
    public float cooldown;
    public float manaCost;
    public float spawnDistance = 0.8f; 

    public GameObject pickupPrefab;
    protected PlayerMovement player;

    private void Start()
    {
        player = PlayerMovement.Instance;
    }

    public abstract IEnumerator Execute();
}
