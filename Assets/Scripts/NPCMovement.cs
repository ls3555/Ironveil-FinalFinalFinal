using UnityEngine;
using UnityEngine.Rendering;

public class NPCMovement : Movement
{
    [Header("NPC AI Settings")]
    public Transform player;
    public Transform enemy;
    public float followDistance = 4f;
    public float stopDistance = 0.8f;   // NEW: Stop before touching player
    public float attackRange = 1.2f;
    public float wanderRadius = 2f;
    public float wanderInterval = 3f;

    private float wanderTimer = 0f;
    private Vector3 wanderTarget;

    [Header("Rendering")]

    // COLLISION LOCKOUT (for interaction only)
    private bool collidedWithPlayer = false;
    private float collisionCooldown = 0.35f;
    private float collisionTimer = 0f;

    // Only allow layer switching when NPC is "active"
    private bool allowLayerChange = false;
    [SerializeField] private Collider2D dialogueTrigger;

    
    protected override void Awake()
    {
        base.Awake();
        dialogueTrigger = GetComponent<Collider2D>();
    }

    // ---------------------------------------------------------
    // PUBLIC CONTROL
    // ---------------------------------------------------------

    public void TurnOn()
    {
        isAsleep = false;
        allowLayerChange = true;

        TransportToPlayer();
        rb.mass = 1f;
        currentState = PlayerState.MOVE;

        SetToLayerPlayerOn();
    }

    public void TurnOff()
    {
        isAsleep = true;
        allowLayerChange = false;
        if (rb == null)
    {
        Debug.LogError("Rigidbody2D is null on " + gameObject.name + ". Assign it before calling Sleep().");
        return;
    }
        rb.linearVelocity = Vector2.zero;
        currentState = PlayerState.IDLE;
    }

    // ---------------------------------------------------------
    // SET NPC TO LAYER 1 (PHYSICS + SORTING)
    // ---------------------------------------------------------

private void SetToLayerPlayerOn()
{
    if (!allowLayerChange || player == null)
        return;

    gameObject.layer = player.gameObject.layer;
}
public void DisableDialogueTrigger()
{
    if (dialogueTrigger != null)
        dialogueTrigger.enabled = false;
}
    // ---------------------------------------------------------
    // COLLISION HANDLING (Interaction Only)
    // ---------------------------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            HandlePlayerHit();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            HandlePlayerHit();
    }

    private void HandlePlayerHit()
    {
        if (!allowLayerChange)
            return; // Only interact when active

        collidedWithPlayer = true;
        collisionTimer = collisionCooldown;

        movementLocked = true;

        rb.linearVelocity = Vector2.zero;
        currentState = PlayerState.IDLE;
        moveDirection = Vector2.zero;

        SetToLayerPlayerOn();
    }

    // ---------------------------------------------------------
    // AI STATE MACHINE
    // ---------------------------------------------------------

    protected override void HandleState()
    {
        // COLLISION LOCKOUT (interaction only)
        if (collidedWithPlayer)
        {
            collisionTimer -= Time.deltaTime;

            if (collisionTimer > 0f)
            {
                currentState = PlayerState.IDLE;
                moveDirection = Vector2.zero;
                return;
            }

            collidedWithPlayer = false;
            movementLocked = false;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        // FAIL‑SAFE: NPC TOO FAR → TELEPORT
        if (dist > followDistance * 3f)
        {
            TransportToPlayer();
            currentState = PlayerState.MOVE;
            return;
        }

        // ATTACK ENEMY
        if (Vector2.Distance(transform.position, enemy.position) <= attackRange)
        {
            currentState = PlayerState.ATTACK;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // FOLLOW PLAYER
        if (dist <= followDistance)
        {
            // NEW: Stop before touching the player
            if (dist <= stopDistance)
            {
                currentState = PlayerState.IDLE;
                moveDirection = Vector2.zero;
                return;
            }

            currentState = PlayerState.MOVE;

            Vector2 dir = (player.position - transform.position).normalized;
            moveDirection = dir;
            FlipSprite(dir);
            return;
        }

        // IDLE / WANDER
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            wanderTimer = wanderInterval;

            Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
            wanderTarget = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
        }

        Vector2 wanderDir = (wanderTarget - transform.position);

        if (wanderDir.magnitude > 0.2f)
        {
            currentState = PlayerState.MOVE;
            moveDirection = wanderDir.normalized;
            FlipSprite(moveDirection);
        }
        else
        {
            currentState = PlayerState.IDLE;
            moveDirection = Vector2.zero;
        }
    }

    // ---------------------------------------------------------
    // MOVEMENT
    // ---------------------------------------------------------

    protected override void Move()
    {
        if (movementLocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (currentState == PlayerState.MOVE)
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // ---------------------------------------------------------
    // TELEPORT NEAR PLAYER (DEMO TRICK)
    // ---------------------------------------------------------

    private void TransportToPlayer()
    {
        var playerRenderer = player.GetComponentInChildren<SpriteRenderer>();
        Vector3 playerSize = playerRenderer.bounds.size;

        float facing = Mathf.Sign(player.localScale.x);

        Vector3 offset = new Vector3(
            playerSize.x * 1.5f * facing,
            playerSize.y * 0.5f,
            0f
        );

        transform.position = player.position + offset;
    }
}


/*
    [Header("NPC Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float waypointTolerance = 0.25f;
    [SerializeField] private float pathRefreshRate = 0.4f;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Fallback Settings")]
    [SerializeField] private Transform fallbackSpot;   
    [SerializeField] private float lostPlayerThreshold = 5f; 

    private float lostPlayerTimer = 0f; 

    private List<Vector3> currentPath;
    private int waypointIndex = 0;

    private float pathRefreshTimer = 0f;
    private float attackTimer = 0f;

    private readonly List<Transform> enemiesInRange = new();

    private SortingGroup sg;

    private PlayerState npcState = PlayerState.IDLE;

    // ---------------------------------------------------------
    // LIFECYCLE
    // ---------------------------------------------------------



    protected override void Start()
    {
    base.Start();
    sg = GetComponentInParent<SortingGroup>();
    Sleep();
    }

    // ---------------------------------------------------------
    // PUBLIC CONTROL
    // ---------------------------------------------------------

    public void WakeUp()
    {
        enabled = true;

        currentPath = null;
        waypointIndex = 0;
        pathRefreshTimer = 0f;
        attackTimer = 0f;
        lostPlayerTimer = 0f;
        TransportToPlayer();
        rb.mass = 1f;

        npcState = PlayerState.MOVE;
        RequestPath();
    }

    public void Sleep()
    {
        enabled = false;
        rb.linearVelocity = Vector2.zero;
    }

    // ---------------------------------------------------------
    // OVERRIDES FROM Movement.cs
    // ---------------------------------------------------------

    protected override void HandleState()
    {
        if (!_initialized) return;

        pathRefreshTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;

        int npcLayer = GetGridLayerAtPosition(transform.position);
        int playerLayer = GetGridLayerAtPosition(player.position);

        if (npcLayer != playerLayer)
        {
            lostPlayerTimer += Time.deltaTime;

            if (lostPlayerTimer >= lostPlayerThreshold)
            {
                GoToFallbackSpot(); 
                return;
            }
        }
        else
        {
            lostPlayerTimer = 0f;
        }

        // Attack logic
        if (IsEnemyInAttackRange())
        {
            rb.linearVelocity = Vector2.zero;
            npcState = PlayerState.ATTACK;

            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
            }

            return;
        }

        // Request new path
        if (pathRefreshTimer <= 0f)
        {
            RequestPath();
            pathRefreshTimer = pathRefreshRate;
        }

        // Animation state
        if (currentPath == null || waypointIndex >= currentPath.Count)
            npcState = PlayerState.IDLE;
        else
            npcState = PlayerState.MOVE;

        PlayStateAnimation(npcState);
    }

    protected override void Move()
    {
        FollowPath();
    }

    // ---------------------------------------------------------
    // PATHFINDING
    // ---------------------------------------------------------

    private void RequestPath()
    {
        int npcLayer = GetGridLayerAtPosition(transform.position);
        gameObject.layer = GridToUnityLayer(npcLayer);
        SyncSortingLayer();

        int playerLayer = GetGridLayerAtPosition(player.position);
        GridTile npcNode = PathFindingGrid.Instance.GetNodeFromWorld(npcLayer, transform.position);
        GridTile playerNode = PathFindingGrid.Instance.GetNodeFromWorld(playerLayer, player.position);
        
        if (npcNode == null || playerNode == null)
        {
            currentPath = null;
            waypointIndex = 0;
            return;
        }

        List<GridTile> newPath = AStarPathFinder.FindPath(npcNode, playerNode);

        if (newPath == null || newPath.Count == 0)
        {
            currentPath = null;
            waypointIndex = 0;
            return;
        }

        // Convert tiles → world positions
        List<Vector3> worldPath = new List<Vector3>();

        foreach (GridTile tile in newPath)
        {
            Tilemap map = PathFindingGrid.Instance.tilemaps[tile.layer];
            Vector3Int cell = new Vector3Int(
                tile.x + map.cellBounds.min.x,
                tile.y + map.cellBounds.min.y,
                0
            );
            worldPath.Add(map.GetCellCenterWorld(cell));
        }

        currentPath = worldPath;
        waypointIndex = 0;
    }

    private void FollowPath()
    {
        if (currentPath == null || waypointIndex >= currentPath.Count)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 target = currentPath[waypointIndex];
        Vector2 direction = target - rb.position;

        if (direction.sqrMagnitude < waypointTolerance * waypointTolerance)
        {
            waypointIndex++;
            return;
        }

        direction.Normalize();
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        FlipSprite(direction);
    }

    // ---------------------------------------------------------
    // FALLBACK LOGIC
    // ---------------------------------------------------------

    private void GoToFallbackSpot() // 🔵 ADDED
    {
        if (fallbackSpot == null)
        {
            Debug.LogWarning("NPC has no fallback spot assigned!");
            return;
        }

        TransportToPlayer();
        //npcState = PlayerState.MOVE;
    }

    // ---------------------------------------------------------
    // COMBAT
    // ---------------------------------------------------------

    private bool IsEnemyInAttackRange()
    {
        if (enemiesInRange.Count == 0)
            return false;

        foreach (Transform enemy in enemiesInRange)
        {
            if (enemy == null) continue;

            if (Vector2.Distance(transform.position, enemy.position) <= attackRange)
                return true;
        }

        return false;
    }

    private Transform GetClosestEnemy()
    {
        Transform closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Transform enemy in enemiesInRange)
        {
            if (enemy == null) continue;

            float dist = Vector2.Distance(transform.position, enemy.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    private void Attack()
    {
        Transform target = GetClosestEnemy();
        if (target != null)
        {
            Debug.Log("NPC attacks " + target.name + " for " + attackDamage);
        }
    }

    // ---------------------------------------------------------
    // ENEMY DETECTION
    // ---------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (!enemiesInRange.Contains(other.transform))
            enemiesInRange.Add(other.transform);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        enemiesInRange.Remove(other.transform);
    }

    private void SyncSortingLayer()
    {
        if (sg == null) return;

        int gridLayer = CurrentGridLayer;
        sg.sortingLayerName = "Layer " + gridLayer;
    }

    

private void TransportToPlayer()
{
    // Get player size
    var playerRenderer = player.GetComponentInChildren<SpriteRenderer>();
    Vector3 playerSize = playerRenderer.bounds.size;

    // Determine which side the player is facing
    float facing = Mathf.Sign(player.localScale.x);

    // Aesthetic offset
    Vector3 offset = new Vector3(
        playerSize.x * 1.2f * facing,   // left or right of player
        playerSize.y * 0.3f,            // slight vertical lift
        0f
    );

    // Teleport NPC to aesthetic position near player
    transform.position = player.position + offset;
}

}
/* int npcLayer = GetGridLayerAtPosition(transform.position);
        gameObject.layer = GridToUnityLayer(npcLayer);
        SyncSortingLayer();

        int playerLayer = GetGridLayerAtPosition(player.position);

        List<Vector3> newPath = AStarPathFinder.FindPath(
            npcLayer,
            transform.position,
            playerLayer,
            player.position
        );*/