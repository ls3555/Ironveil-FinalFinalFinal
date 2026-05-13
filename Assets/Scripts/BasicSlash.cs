using UnityEngine;
using System.Collections;

public class BasicSlash : PlayerMove
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        cooldown = .75f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override IEnumerator Execute()
    {
        Debug.Log("Slash");
        yield return new WaitForSeconds(cooldown);
    }
}
