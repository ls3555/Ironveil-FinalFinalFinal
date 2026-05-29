using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamagable
{
    float health = 1;
    protected float maxHealth;

    void Awake()
    {
        maxHealth = health;
    }
    public void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - 1, 0, maxHealth);
        if(health<=0) { Destroy(this.gameObject);}
    }

}
