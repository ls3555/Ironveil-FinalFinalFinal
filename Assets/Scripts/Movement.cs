using UnityEngine;
using UnityEngine.Tilemaps;


public abstract class Movement : PlayerObj, IDamagable
{
    protected Rigidbody2D rb;
    protected Vector2 moveDirection;
    protected PlayerState currentState = PlayerState.IDLE;

    [Header("Stats")]
    public float health;
    protected float maxHealth;

    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float friction;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        maxHealth = health;
        transform.localScale = new Vector3(1.5f, 1.5f, 1f);
    }

    protected override void Start()
    {
        base.Start(); // PlayerObj handles SPUM setup
    }

    protected virtual void Update()
    {
        if (!_initialized) return;

        // Z‑sorting
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.y * -0.01f
        );

        HandleState();
        PlayStateAnimation(currentState);
        
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    // ---------------------------
    // ABSTRACT METHODS
    // ---------------------------

    protected abstract void HandleState();   // Player = input, NPC = AI
    protected abstract void Move();          // Player = direct velocity, NPC = path

    // ---------------------------
    // SHARED METHODS
    // ---------------------------

    public virtual void TakeDamage(float dmg)
    {
        health = Mathf.Clamp(health - dmg, 0, maxHealth);
        currentState = health <= 0 ? PlayerState.DEATH : PlayerState.DAMAGED;
    }

    protected void FlipSprite(Vector2 dir)
    {
        if (dir.x > 0)
            _prefabs.transform.localScale = new Vector3(-Mathf.Abs(_prefabs.transform.localScale.x), _prefabs.transform.localScale.y, 1f);
        else if (dir.x < 0)
            _prefabs.transform.localScale  = new Vector3(Mathf.Abs(_prefabs.transform.localScale.x), _prefabs.transform.localScale.y, 1f);
    }
    

    public void SetVelocity(Vector2 vel)
    {
        rb.linearVelocity = vel;
    }

    public int CurrentGridLayer
    {
        get
        {
            foreach (var (map, layer) in PathFindingGrid.Instance.layers)
            {
                Vector3Int cell = map.WorldToCell(transform.position);
                if (map.HasTile(cell))
                    return layer;
            }
            return -1;
        }
    }

    public static int GridToUnityLayer(int gridLayer)
    {
        return gridLayer + 19;
    }
    protected int GetGridLayerAtPosition(Vector3 worldPos)
{
    int highestLayer = -1;

    foreach (var (map, layer) in PathFindingGrid.Instance.layers)
    {
        if (map.HasTile(map.WorldToCell(worldPos)))
        {
            if (layer > highestLayer)
                highestLayer = layer;
        }
    }

    return highestLayer;
}


}
