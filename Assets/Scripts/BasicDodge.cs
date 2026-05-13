using UnityEngine;
using System.Collections;

public class BasicDodge : PlayerMove
{
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float dashTime = 0.15f;

    private PlayerMovement player;
    bool canDash = true;

    private void Start()
    {
        player = PlayerMovement.Instance;
    }

    public override IEnumerator Execute()
    {
        if (!canDash)
            yield break;

        canDash = false;
        player.isDashing = true;

        Vector2 dir = player.getMoveDirection().normalized;
        if (dir == Vector2.zero)
            dir = Vector2.right;

        player.SetVelocity(dir * dashSpeed);

        yield return new WaitForSeconds(dashTime);

        player.isDashing = false;

        yield return new WaitForSeconds(cooldown);
        canDash = true;
    }
}
