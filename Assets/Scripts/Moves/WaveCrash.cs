using UnityEngine;
using System.Collections;

public class WaveCrash : PlayerMove
{
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    public DotZone wave;
    bool canDash = true;

    public override IEnumerator Execute()
    {
        if (!canDash || player.GetMana() < manaCost)
            yield break;

        canDash = false;
        cooldownRemaining = cooldown;
        player.setState(state.dashing);
        player.UseMana(manaCost);

        Vector2 dir = player.getMoveDirection().normalized;

        PlayerAudio audio = PlayerMovement.Instance.GetComponent<PlayerAudio>();
        if (audio != null) audio.PlayDash();

        if (dir == Vector2.zero)
            dir = Vector2.right;

        float timer = 0f;

        Vector2 shootDirection = player.CalcShootDir();
        Vector2 spawnPos = (Vector2)player.transform.position + shootDirection * spawnDistance;

        DotZone newWave = Instantiate(wave, spawnPos, Quaternion.identity);
        newWave.transform.SetParent(player.transform);
        newWave.setDamage(player.getSpecAttackStat());
        newWave.setTarget(player.opponentTag, shootDirection);


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