using UnityEngine;
using System.Collections;

public class BasicSlash : PlayerMove
{
    public Slash slash;
    public float spawnDistance = 0.8f; 
    bool canAttack = true;
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
        if (!canAttack || player.GetMana() < manaCost)
            yield break;

        player.setState(state.attacking);

        Vector2 shootDirection = player.CalcShootDir();
        Vector2 spawnPos = (Vector2)player.transform.position + shootDirection * spawnDistance;

        Slash newSlash = Instantiate(slash,spawnPos,Quaternion.identity);
        newSlash.setTarget(player.opponentTag,shootDirection);
 
        player.UseMana(manaCost);
        player.setState(state.idle); 
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
