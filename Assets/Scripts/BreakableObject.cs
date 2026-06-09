using UnityEngine;

public class BreakableObject : MonoBehaviour, IDamagable
{
    float health = 1;
    protected float maxHealth;

    public System.Action OnBroken;

    void OnDestroy()
    {
        OnBroken?.Invoke();
    }

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
