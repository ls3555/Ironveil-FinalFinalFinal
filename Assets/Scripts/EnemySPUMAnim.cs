using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemySpum : PlayerObj, IDamagable
{

    [Header("Entity Settings")]

    protected SpriteRenderer spriteRenderer;
    public float health;
    protected float maxHealth;
    [SerializeField]protected float moveSpeed;
    [SerializeField]protected float friction;
    protected Vector2 moveDirection;
    public string opponentTag;
    public System.Action OnDeath;


    [Space(10)]
    [Header("Enemy UI")]

    public Image healthBar;
    public GameObject healthContainer;
    public GameObject canvas;
    public float hideDelay = 2f;
    protected float lastDamageTime;
    protected Coroutine hideHPRoutine;
    public TextMeshProUGUI damagePopup;

    private enum EnemyState { Idle, Roam, Chase, Attack, Die }
    private EnemyState currentState = EnemyState.Idle;
    private Vector2 targetPosition;

    [Header("Movement")]
    [SerializeField] private float roamDist = 3f;
    [SerializeField] private float attackDist = 1.5f;

    [Header("Stats")]
    public int damage = 5;
    public float chaseDist = 10f;

    bool canAttack = true;
    float idleTimer = 0f;
    float idleWaitTime = 2f;

    protected void Awake()
    {
        maxHealth = health;
    }

    override protected void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            healthContainer.gameObject.SetActive(false);

            // _rb2D setup — prevents tipping/rotation and keeps it 2D-correct
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            if (_prefabs == null)
                _prefabs = GetComponentInChildren<SPUM_Prefabs>();

            if (_prefabs == null)
            {
                Debug.LogError($"[PlayerObj] No SPUM_Prefabs found on {name} or its children.");
                return;
            }

            if (_prefabs._anim == null)
            {
                Debug.LogError($"[PlayerObj] SPUM_Prefabs on {name} has no _anim assigned.");
                return;
            }

            _prefabs.OverrideControllerInit();

            foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
                IndexPair[state] = 0;

            Debug.Log("[PlayerObj] SPUM StateAnimationPairs keys: " +
                string.Join(", ", _prefabs.StateAnimationPairs.Keys));


            currentState = EnemyState.Idle;
            PickNewRoamTarget();

            _initialized = true;
        }

    void Update()
        {
            if (!_initialized) return;

            // Y-position drives Z so sprites closer to the bottom of screen render on top
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.y * -0.01f
            );

                UpdateHealth();
            if (_rb == null || PlayerMovement.Instance == null) return;

            float distToPlayer = Vector2.Distance(transform.position, PlayerMovement.Instance.transform.position);

            if (currentState == EnemyState.Idle)
            {
                _currentState = PlayerState.IDLE;
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
                _currentState = PlayerState.MOVE;
                moveDirection = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
                FlipSprite(moveDirection);

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
                _currentState = PlayerState.MOVE;
                moveDirection = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
                FlipSprite(moveDirection);

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
                _currentState = PlayerState.ATTACK;
                moveDirection = Vector2.zero;
                if (distToPlayer > attackDist)
                {
                    currentState = EnemyState.Chase;
                }
                if (canAttack)
                {
                    StartCoroutine(AttackCoroutine());
                }
            }
            PlayStateAnimation(_currentState);
        
        }

        private void FlipSprite(Vector2 direction)
        {
            if (direction.x > 0f)
                _prefabs.transform.localScale = new Vector3(-1.2f, 1.2f, 1f); // face right
            else if (direction.x < 0f)
                _prefabs.transform.localScale = new Vector3(1.2f, 1.2f, 1f);  // face left
        }

        protected void Move()
        {
            if (moveDirection.magnitude > 0)
            {
                _rb.linearVelocity = moveDirection * moveSpeed;
            }
            else
            {
                _rb.linearVelocity *= (1f - friction);
            }
        }

        void FixedUpdate() {Move();}

        private void PickNewRoamTarget()
        {
            targetPosition = (Vector2)transform.position + new Vector2(
                UnityEngine.Random.Range(-roamDist, roamDist),
                UnityEngine.Random.Range(-roamDist, roamDist)
            );
        }

        private IEnumerator AttackCoroutine()
        {
            if (!canAttack)
                yield break;
                
            canAttack = false;
            _rb.linearVelocity = Vector2.zero;

            yield return new WaitForSeconds(3);
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

        public void Die()
        {
            _currentState = PlayerState.DEATH;
            PlayStateAnimation(_currentState);
            _rb.linearVelocity = Vector2.zero;
            enabled = false;
        }

        public void TakeDamage(float damage)
        {
             _currentState = PlayerState.DAMAGED;
            PlayStateAnimation(_currentState);
            health = Mathf.Clamp(health - damage, 0, maxHealth);
            healthBar.fillAmount = health / maxHealth;
            if(health<=0) {Die();}
            lastDamageTime = Time.time;

            ShowDamage(damage);

            if (hideHPRoutine != null)
                StopCoroutine(hideHPRoutine);

            hideHPRoutine = StartCoroutine(HideHealthBar());
        }

        protected void UpdateHealth()
    {
        if (health < maxHealth)
        {
            health = Mathf.Clamp(health, 0, maxHealth);
            healthBar.fillAmount = health / maxHealth;
        }
    }

    protected void ShowDamage(float damage)
    {
        Vector3 randomOffset = new Vector3(
        UnityEngine.Random.Range(-0.5f, 0.5f),
        UnityEngine.Random.Range(-1f, 1f),
        0f);

        Vector3 spawnPos = transform.position + randomOffset;

        TextMeshProUGUI popup = Instantiate(damagePopup,transform.position,Quaternion.identity);
        popup.transform.SetParent(canvas.transform, false);
        popup.transform.position = spawnPos;
        popup.text = "" +damage;
    }

    protected IEnumerator HideHealthBar()
    {
        healthContainer.gameObject.SetActive(true);

        while (Time.time - lastDamageTime < hideDelay)
            yield return null;

        healthContainer.gameObject.SetActive(false);
    }

}
