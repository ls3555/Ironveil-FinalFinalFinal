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
        cooldown = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override IEnumerator Execute()
    {
        if (!canUtil)
            yield break;


        canUtil = false;
        player.setState(state.attacking);

        Vector2 shootDirection = player.CalcShootDir();
        ParticleSystem newhealParticles = Instantiate(healParticles,player.transform.position,Quaternion.identity);
        Destroy(newhealParticles.gameObject,2);
        player.HealDamage(healAmt);


        player.setState(state.idle); 
        yield return new WaitForSeconds(cooldown);
        canUtil = true;;
    }
}
