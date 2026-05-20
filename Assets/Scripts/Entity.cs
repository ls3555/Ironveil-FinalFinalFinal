using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public abstract class Entity : MonoBehaviour, IDamagable
{
    protected Rigidbody2D rigidBody;
    protected SpriteRenderer spriteRenderer;
    public float health;
    protected float maxHealth;
    [SerializeField]protected float moveSpeed;
    [SerializeField]protected float friction;
    protected Vector2 moveDirection;
    public string opponentTag;
    public System.Action OnDeath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.gravityScale = 0;

        maxHealth = health;
    }    

    abstract public void TakeDamage(float damage);
    abstract protected void Move();
    void FixedUpdate() {Move();}
}
