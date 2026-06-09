using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;


public class NPCMovement : Movement
{
    [Header("NPC Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float waypointTolerance = 0.25f;
    [SerializeField] private float pathRefreshRate = 0.4f;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Fallback Settings")]
    [SerializeField] private Transform fallbackSpot;   // 🔵 ADDED
    [SerializeField] private float lostPlayerThreshold = 5f; // 🔵 ADDED

    private float lostPlayerTimer = 0f; // 🔵 ADDED

    private List<Vector3> currentPath;
    private int waypointIndex = 0;

    private float pathRefreshTimer = 0f;
    private float attackTimer = 0f;

    private readonly List<Transform> enemiesInRange = new();

    private SortingGroup sg;
    private int npcGridLayer;

    private PlayerState npcState = PlayerState.IDLE;

    // ---------------------------------------------------------
    // LIFECYCLE
    // ---------------------------------------------------------

    protected override void Awake()
    {
        base.Awake();
    }

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

        // 🔵 FLOOR MISMATCH CHECK
        npcGridLayer = GetGridLayerAtPosition(transform.position);
        int playerLayer = GetGridLayerAtPosition(player.position);
        

        if (npcGridLayer != playerLayer)
        {
            lostPlayerTimer += Time.deltaTime;

            if (lostPlayerTimer >= lostPlayerThreshold)
            {
                GoToFallbackSpot(); // 🔵 ADDED
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
        npcGridLayer = GetGridLayerAtPosition(transform.position);
        SyncSortingLayer();

        int playerLayer = GetGridLayerAtPosition(player.position);
        GridTile npcNode = PathFindingGrid.Instance.GetNodeFromWorld(npcGridLayer, transform.position);
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

        int gridLayer = npcGridLayer;
        sg.sortingLayerName = "Layer " + gridLayer;
    }

    

private void TransportToPlayer()
{
    int npcGridLayer = 1;
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