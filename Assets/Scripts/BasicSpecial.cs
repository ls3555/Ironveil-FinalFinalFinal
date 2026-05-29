using UnityEngine;
using System.Collections;

public class BasicSpecial : PlayerMove
{
    public float shootForce;
    public Bullet bullet;
    bool canSpecial = true;

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public override IEnumerator Execute()
    {
        if (!canSpecial || player.GetMana() < manaCost)
            yield break;


        canSpecial = false;
        player.setState(state.attacking);

        Vector2 shootDirection = player.CalcShootDir();
        Vector2 spawnPos = (Vector2)player.transform.position + shootDirection * spawnDistance;

        Bullet newBullet = Instantiate(bullet,spawnPos,Quaternion.identity);
        newBullet.setDamage(player.getSpecAttackStat());
        newBullet.setTarget(player.opponentTag,shootDirection,shootForce);
 
        player.UseMana(manaCost);
        player.setState(state.idle); 
        yield return new WaitForSeconds(cooldown);
        canSpecial = true;
    }
}
