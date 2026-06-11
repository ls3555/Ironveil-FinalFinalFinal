using UnityEngine;
using System.Collections;

public class StatBoost : PlayerMove
{
    public ParticleSystem statRaise, boosting;
    public float buffTime;
    public float buffStr;
    public float buffRegen;
    public float buffMana;
    bool canUtil = true;

    public override IEnumerator Execute()
    {
        if (!canUtil || player.GetMana() < manaCost)
            yield break;

        canUtil = false;
        cooldownRemaining = cooldown;
        player.setState(state.attacking);
        player.UseMana(manaCost);

        ParticleSystem newhealParticles = Instantiate(statRaise,player.transform.position,Quaternion.identity);
        ParticleSystem newboostParticles = Instantiate(boosting,player.transform.position,Quaternion.identity);
        newboostParticles.transform.SetParent(player.transform);
        newboostParticles.Play();
        Destroy(newhealParticles.gameObject,2);
        Destroy(newboostParticles.gameObject,buffTime);

        StartCoroutine(player.StatBuffRoutine(buffStr, buffRegen, buffMana, buffTime));


        player.setState(state.idle); 
        yield return new WaitForSeconds(cooldown);
        canUtil = true;
    }


}
