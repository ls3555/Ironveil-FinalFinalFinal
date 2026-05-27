using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    private Animator animator;
    private enum EnemyState { Idle, Roam, Chase, Attack, Die }
    private EnemyState currentState = EnemyState.Idle;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveDirection, targetPosition;
    public AnimationClip hitClip, attackClip, dieClip;

    [Header("Movement")]
    [SerializeField] private float roamDist = 3f;
    [SerializeField] private float attackDist = 1.5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float friction = 0.1f;

    [Header("Stats")]
    public int damage = 5;
    public int health = 100;
    public float chaseDist = 10f;

    bool canAttack = true;
    float idleTimer = 0f;
    float idleWaitTime = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null) Debug.LogError("MISSING: Rigidbody2D on " + gameObject.name);
        if (animator == null) Debug.LogError("MISSING: Animator on " + gameObject.name);

        currentState = EnemyState.Idle;
        PickNewRoamTarget();
    }

    void Update()
    {
        if (rb == null || animator == null || PlayerController.Instance == null) return;

        float distToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

        if (currentState == EnemyState.Idle)
        {
            moveDirection = Vector2.zero;
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleWaitTime)
            {
                idleTimer = 0f;
                PickNewRoamTarget();
                currentState = EnemyState.Roam;
            }
            if (distToPlayer < chaseDist)
            {
                currentState = EnemyState.Chase;
            }
        }
        else if (currentState == EnemyState.Roam)
        {
            moveDirection = ((Vector2)targetPosition - (Vector2)transform.position).normalized;

            if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
            {
                currentState = EnemyState.Idle;
            }
            if (distToPlayer < chaseDist)
            {
                currentState = EnemyState.Chase;
            }
        }
        else if (currentState == EnemyState.Chase)
        {
            targetPosition = PlayerController.Instance.transform.position;
            moveDirection = ((Vector2)targetPosition - (Vector2)transform.position).normalized;

            if (distToPlayer < attackDist)
            {
                currentState = EnemyState.Attack;
            }
            else if (distToPlayer > chaseDist * 1.2f)
            {
                PickNewRoamTarget();
                currentState = EnemyState.Roam;
            }
        }
        else if (currentState == EnemyState.Attack)
        {
            moveDirection = Vector2.zero;
            if (distToPlayer > attackDist)
            {
                currentState = EnemyState.Chase;
            }
            if (canAttack)
            {
                canAttack = false;
                StartCoroutine(AttackCoroutine());
            }
        }

        Move();
        UpdateAnimator();
    }

    private void PickNewRoamTarget()
    {
        targetPosition = (Vector2)transform.position + new Vector2(
            Random.Range(-roamDist, roamDist),
            Random.Range(-roamDist, roamDist)
        );
    }

    private void Move()
    {
        if (moveDirection.magnitude > 0)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            rb.linearVelocity *= (1f - friction);
        }
    }

    private IEnumerator AttackCoroutine()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Attack");

        float clipLength = attackClip != null ? attackClip.length : 1f;
        yield return new WaitForSeconds(clipLength);
        float disToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        if (disToPlayer < attackDist + 0.5f)
        {
            PlayerController.Instance.TakeDamage(damage);
        }
        else
        {
            currentState = EnemyState.Chase;
        }
        canAttack = true;
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        enabled = false;
    }

    public void TakeHit()
    {
        animator.SetTrigger("Hit");
    }

    private void UpdateAnimator()
    {
        animator.SetBool("IsMoving", moveDirection.magnitude > 0.1f);
    }
}