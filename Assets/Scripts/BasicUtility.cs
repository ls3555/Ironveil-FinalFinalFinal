using UnityEngine;
using System.Collections;

public class BasicUtility : PlayerMove
{
    public ParticleSystem healParticles;
    public float healAmt;
    bool canUtil = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override IEnumerator Execute()
    {
        if (!canUtil || player.GetMana() < manaCost)
            yield break;


        canUtil = false;
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
