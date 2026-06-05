using UnityEngine;

public class NPCMoveToPlayer : MonoBehaviour 
{
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float movingMass = 3f;
    [SerializeField] private float originalMass = 9999f;
    [SerializeField] private float stopDistance = 1.5f;


    public SPUM_Prefabs _prefabs;

    private bool moveToPlayer = false;
    private Rigidbody2D rb;
    private PlayerState _currentState;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = originalMass;

        if (_prefabs == null)
            _prefabs = GetComponentInChildren<SPUM_Prefabs>();

        _prefabs.OverrideControllerInit();
    }

    public void StartMoving()
    {
        rb.mass = movingMass;
        moveToPlayer = true;
    }

    public void StopMoving()
    {
        moveToPlayer = false;
        _prefabs.PlayAnimation(PlayerState.IDLE, 0);
    }

    private void Update()
    {
        if (_prefabs == null) return;
        PlayerState newState = moveToPlayer ? PlayerState.MOVE : PlayerState.IDLE;
        if (newState != _currentState)
        {
            _currentState = newState;
            _prefabs.PlayAnimation(_currentState, 0);
        }

        // Flip sprite based on direction like PlayerObj does
        Vector2 direction = (player.position - transform.position);
        if (direction.x > 0f)
            _prefabs.transform.localScale = new Vector3(-1.5f, 1.5f, 1f);
        else if (direction.x < 0f)
            _prefabs.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
    }
private void FixedUpdate()
{
    if (!moveToPlayer)
    {
        rb.linearVelocity = Vector2.zero;
        return;
    }

    float distance = Vector2.Distance(rb.position, player.position);

    if (distance <= stopDistance)
    {
        rb.linearVelocity = Vector2.zero;
        moveToPlayer = false;
        return;
    }

    Vector2 dir = ((Vector2)player.position - rb.position).normalized;

    rb.linearVelocity = dir * moveSpeed;
}
}