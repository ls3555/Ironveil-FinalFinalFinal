using UnityEngine;
using System.Collections;

public class Burst : PlayerMove
{
    public AoeSplash splash;
    bool canSpecial = true;

    public override IEnumerator Execute()
    {
        if (!canSpecial || player.GetMana() < manaCost)
                yield break;


            canSpecial = false;
            cooldownRemaining = cooldown;
            player.setState(state.attacking);

            Vector2 shootDirection = player.CalcShootDir();
            Vector2 spawnPos = (Vector2)player.transform.position + shootDirection * spawnDistance;

            PlayerAudio audio = PlayerMovement.Instance.GetComponent<PlayerAudio>();
            if (audio != null) audio.PlaySpecial();

            AoeSplash newSplash = Instantiate(splash, spawnPos, Quaternion.identity);
            newSplash.setDamage(player.getSpecAttackStat());
            newSplash.setTarget(player.opponentTag, shootDirection);

            player.UseMana(manaCost);
            player.setState(state.idle);
            yield return new WaitForSeconds(cooldown);
            canSpecial = true;
    }
}
