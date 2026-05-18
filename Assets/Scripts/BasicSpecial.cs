using UnityEngine;
using System.Collections;

public class BasicSpecial : PlayerMove
{
    public float damage;
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
        Vector2 shootDirection = CalcShootDir();
        

        Debug.Log("Zip");
        //Bullet newBullet = Instantiate(bullet,transform.position,Quaternion.identity);
        //newBullet.setTarget(opponentTag,shootDirection,shootForce);
        canSpecial = false;
        yield return new WaitForSeconds(cooldown);
        canSpecial = true;
    }
}
