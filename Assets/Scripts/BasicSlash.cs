using UnityEngine;
using System.Collections;

public class BasicSlash : PlayerMove
{
    //public Slash slash;
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
        if (!canAttack)
            yield break;

        player.setState(state.attacking);
        Debug.Log("Slash");

        //Vector2 shootDirection = CalcShootDir();
        //GameObject newSlash = Instantiate(slash,player.transform.position,Quaternion.identity);
        //newSlash.setTarget(player.opponentTag);
 
        player.setState(state.idle); 
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
        

        yield return new WaitForSeconds(cooldown);
    }
}
