using UnityEngine;
using System.Collections;

public class BasicSlash : PlayerMove
{
    public Slash slash;
    bool canAttack = true;


    public override IEnumerator Execute()
    {
        if (!canAttack || player.GetMana() < manaCost)
            yield break;

        canAttack = false;
        cooldownRemaining = cooldown;
        player.setState(state.attacking);

        Vector2 shootDirection = player.CalcShootDir();
        Vector2 spawnPos = (Vector2)player.transform.position + shootDirection * spawnDistance;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        Slash newSlash = Instantiate(slash, spawnPos, rotation);
        newSlash.setDamage(player.getAttackStat());
        newSlash.setTarget(player.opponentTag, shootDirection);

        player.UseMana(manaCost);
        player.setState(state.idle);
        yield return new WaitForSeconds(cooldown);
        canAttack = true;

        PlayerAudio audio = PlayerMovement.Instance.GetComponent<PlayerAudio>();
        if (audio != null) audio.PlayAttack();
    }
}
