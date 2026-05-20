using UnityEngine;
using System.Collections;

public class BasicDodge : PlayerMove
{
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    bool canDash = true;



    public override IEnumerator Execute()
    {
        if (!canDash)
            yield break;

        canDash = false;
        player.setState(state.dashing);

        Vector2 dir = player.getMoveDirection().normalized;

        if (dir == Vector2.zero)
            dir = Vector2.right;

        float timer = 0f;

        while (timer < dashTime)
        {
            float t = timer / dashTime;
            float currentSpeed = Mathf.Lerp(dashSpeed, 0f, t);

            player.SetVelocity(dir * currentSpeed);

            timer += Time.deltaTime;
            yield return null;
        }

        player.SetVelocity(Vector2.zero);
        player.setState(state.idle);

        yield return new WaitForSeconds(cooldown);
        canDash = true;
    }
}
