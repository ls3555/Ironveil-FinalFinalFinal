using UnityEngine;
using System.Collections;

public class BasicUtility : PlayerMove
{
    public ParticleSystem healParticles;
    public float healAmt;
    bool canUtil = true;

    public override IEnumerator Execute()
    {
        if (!canUtil || player.GetMana() < manaCost)
            yield break;


        canUtil = false;
        cooldownRemaining = cooldown;
        player.setState(state.attacking);

        ParticleSystem newhealParticles = Instantiate(healParticles,player.transform.position,Quaternion.identity);
        Destroy(newhealParticles.gameObject,2);
        player.HealDamage(healAmt);

        player.UseMana(manaCost);
        player.setState(state.idle); 
        yield return new WaitForSeconds(cooldown);
        canUtil = true;
    }
}
