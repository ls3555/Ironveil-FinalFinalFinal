using UnityEngine;
using System.Collections;

public class RapidSnipe : PlayerMove
{
    public Shoot shoot;
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

        Shoot newShoot = Instantiate(shoot, spawnPos, Quaternion.identity);
        newShoot.setDamage(player.getAttackStat());
        newShoot.setTarget(player.opponentTag, shootDirection);

        player.UseMana(manaCost);
        player.setState(state.idle);
        yield return new WaitForSeconds(cooldown);
        canAttack = true;

        PlayerAudio audio = PlayerMovement.Instance.GetComponent<PlayerAudio>();
        if (audio != null) audio.PlayAttack();
    }
}
