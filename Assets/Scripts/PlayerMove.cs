using UnityEngine;
using System.Collections;

public abstract class PlayerMove : MonoBehaviour
{
    public float cooldown;
    public float manaCost;
    protected PlayerMovement player;

    private void Start()
    {
        player = PlayerMovement.Instance;
    }

    public abstract IEnumerator Execute();
}
