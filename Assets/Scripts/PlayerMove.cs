using UnityEngine;
using System.Collections;

public abstract class PlayerMove : MonoBehaviour
{
    public MoveSlotType slotType;
    public float cooldown;
    public float manaCost;
    protected PlayerMovement player;

    private void Start()
    {
        player = PlayerMovement.Instance;
    }

    public abstract IEnumerator Execute();

    protected Vector2 CalcShootDir()
    {
        Vector2 shootDirection = (player.GetMousePos() - new Vector2(transform.position.x, transform.position.y)).normalized;
        //changes to player rotation based on mouse pos
        //player.transform.eulerAngles = new Vector3(0, 0, -90 + Mathf.Atan2(shootDirection.y, shootDirection.x) * 180 / Mathf.PI);
        return shootDirection;
    }
}
