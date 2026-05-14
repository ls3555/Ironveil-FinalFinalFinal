using UnityEngine;
using System.Collections;

public class BasicSpecial : PlayerMove
{
    public float damage;
    bool canSpecial = true;
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
        Debug.Log("Zip");
        //Bullet newBullet = Instantiate(bullet,transform.position,Quaternion.identity);
        //newBullet.setTarget(opponentTag,shootDirection,shootForce);
        canSpecial = false;
        yield return new WaitForSeconds(cooldown);
        canSpecial = true;
    }
}
