using UnityEngine;
using System.Collections;

public class HealBuddy : PlayerMove
{
    public float healAmt;
    public float speed;
    bool canUtil = true;
    public Friend friend;

    public override IEnumerator Execute()
    {
        if (!canUtil || player.GetMana() < manaCost)
            yield break;


        canUtil = false;
        cooldownRemaining = cooldown;
        player.setState(state.attacking);

        Vector2 shootDirection = player.CalcShootDir();
        Vector2 spawnPos = (Vector2)player.transform.position + shootDirection * spawnDistance;

        Friend newFriend = Instantiate(friend, spawnPos, Quaternion.identity);
        newFriend.setDamage(healAmt);
        newFriend.setTarget("Player", shootDirection, speed, player);


        player.UseMana(manaCost);
        player.setState(state.idle); 
        yield return new WaitForSeconds(cooldown);
        canUtil = true;
    }
}
