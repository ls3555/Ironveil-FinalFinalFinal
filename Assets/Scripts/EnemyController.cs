using UnityEngine;
using System.Collections;

public class EnemyController : EnemyUI
{
    private Animator animator;
    private enum EnemyState { Idle, Roam, Chase, Attack, Die }
    private EnemyState currentState = EnemyState.Idle;
    private Vector2 targetPosition;
    public AnimationClip hitClip, attackClip, dieClip;

    [Header("Movement")]
    [SerializeField] private float roamDist = 3f;
    [SerializeField] private float attackDist = 1.5f;

    [Header("Stats")]
    public int damage = 5;
    public float chaseDist = 10f;

    bool canAttack = true;
    float idleTimer = 0f;
    float idleWaitTime = 2f;

    void Start()
    {
        healthContainer.gameObject.SetActive(false);
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rigidBody == null) Debug.LogError("MISSING: Rigidbody2D on " + gameObject.name);
        if (animator == null) {Debug.LogError("MISSING: Animator on " + gameObject.name);} else {animator.SetBool("isAlive", true);}

        currentState = EnemyState.Idle;
        PickNewRoamTarget();
    }

    void Update()
    {
        UpdateHealth();
        if (rigidBody == null || animator == null || PlayerMovement.Instance == null) return;

        float distToPlayer = Vector2.Distance(transform.position, PlayerMovement.Instance.transform.position);

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
            targetPosition = PlayerMovement.Instance.transform.position;
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
        UpdateAnimator();
    }

    private void PickNewRoamTarget()
    {
        targetPosition = (Vector2)transform.position + new Vector2(
            Random.Range(-roamDist, roamDist),
            Random.Range(-roamDist, roamDist)
        );
    }

    protected override void Move()
    {
        if (moveDirection.magnitude > 0)
        {
            rigidBody.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            rigidBody.linearVelocity *= (1f - friction);
        }
    }

    private IEnumerator AttackCoroutine()
    {
        rigidBody.linearVelocity = Vector2.zero;
        animator.SetTrigger("Attack");

        float clipLength = attackClip != null ? attackClip.length : 1f;
        yield return new WaitForSeconds(clipLength);
        float disToPlayer = Vector2.Distance(transform.position, PlayerMovement.Instance.transform.position);
        if (disToPlayer < attackDist + 0.5f)
        {
            PlayerMovement.Instance.TakeDamage(damage);
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
        animator.SetBool("isAlive", false);
        rigidBody.linearVelocity = Vector2.zero;
        enabled = false;
    }

    public void TakeHit()
    {
        animator.SetTrigger("Hit");
    }

    public override void TakeDamage(float damage)
    {
        TakeHit();
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        healthBar.fillAmount = health / maxHealth;
        if(health<=0) {Die();}
        lastDamageTime = Time.time;

        ShowDamage(damage);

        if (hideHPRoutine != null)
            StopCoroutine(hideHPRoutine);

        hideHPRoutine = StartCoroutine(HideHealthBar());
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isMoving", moveDirection.magnitude > 0.1f);
    }
}