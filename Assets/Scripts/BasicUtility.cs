using UnityEngine;
using System.Collections;

public class BasicUtility : PlayerMove
{
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
        Debug.Log("Wroooroohhh");
        yield return new WaitForSeconds(cooldown);
    }
}
