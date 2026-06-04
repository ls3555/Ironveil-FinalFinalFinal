using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : PlayerObj, IDamagable
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
    [Header("PM Settings")]

    public static PlayerMovement Instance;

    public float mana;

    public float attack;
    public float specAttack;
    protected float maxMana;

    public float healthRegen;
    public float manaRegen;

    private PlayerInput input;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction attackAction;
    private InputAction specialAction;
    private InputAction dodgeAction;
    private InputAction utilAction;

    private InputAction interactAction;
    [SerializeField] private float interactRange = 3f;

    public PlayerMove moveAttack;
    public PlayerMove moveSpecial;
    public PlayerMove moveDash;
    public PlayerMove moveUtil;

    private Vector2 lastMoveDir = Vector2.right;
    public bool isMoving;
    public bool canMove = true;
    state actionState;
    //private Animator animator;

    [SerializeField] private Image healthBar;
    [SerializeField] private TMP_Text healthNum;
    [SerializeField] private Image manaBar;
    [SerializeField] private TMP_Text manaNum;

    [SerializeField] private CooldownUI attackUI;
    [SerializeField] private CooldownUI specUI;
    [SerializeField] private CooldownUI dashUI;
    [SerializeField] private CooldownUI utilUI;


    protected void Awake()
    {
        Instance = this;
        maxHealth = health;
        maxMana = mana;
    }

    override protected void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Rigidbody2D setup — prevents tipping/rotation and keeps it 2D-correct
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

        input = GameController.Input;
        moveAction = input.Player.Move;
        lookAction = input.Player.Look;
        attackAction = input.Player.Attack;
        specialAction = input.Player.Special;
        dodgeAction = input.Player.Dodge;
        utilAction = input.Player.Utility;
        interactAction = input.Player.Interact;
        healthNum.text = Mathf.RoundToInt(health).ToString();
        manaNum.text = Mathf.RoundToInt(mana).ToString();

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

        if (isAction) return;

        if (isControlled)
        {

            if (canMove)
                {
                    moveDirection = moveAction.ReadValue<Vector2>().normalized;
                }
                else
                {
                    moveDirection = Vector2.zero;
                }
                isMoving = moveDirection.magnitude > 0;


                if (moveDirection.sqrMagnitude > 0.01f)
                {
                    _currentState = PlayerState.MOVE;
                    FlipSprite(moveDirection);
                }
                else
                {
                    _currentState = PlayerState.IDLE;
                }
            }
            else
            {
                _currentState = PlayerState.IDLE;
            }

            switch (actionState)
            {
                case state.idle:
                    if (interactAction.WasPressedThisFrame()) { TryInteract(); }
                    if (attackAction.WasPressedThisFrame() && moveAttack != null) { StartCoroutine(moveAttack.Execute());  _currentState = PlayerState.ATTACK; }
                    if (specialAction.WasPressedThisFrame() && moveSpecial != null) { StartCoroutine(moveSpecial.Execute());  _currentState = PlayerState.ATTACK;}
                    if (dodgeAction.WasPressedThisFrame() && moveDash != null) { StartCoroutine(moveDash.Execute()); }
                    if (utilAction.WasPressedThisFrame() && moveUtil != null) { StartCoroutine(moveUtil.Execute()); }
                    break;
                case state.attacking:
                    if (utilAction.WasPressedThisFrame() && moveUtil != null) { StartCoroutine(moveUtil.Execute()); }
                    break;
                case state.dashing:
                    if (attackAction.WasPressedThisFrame() && moveAttack != null) { StartCoroutine(moveAttack.Execute()); }
                    if (specialAction.WasPressedThisFrame() && moveSpecial != null) { StartCoroutine(moveSpecial.Execute()); }
                    if (utilAction.WasPressedThisFrame() && moveUtil != null) { StartCoroutine(moveUtil.Execute()); }
                    break;
                case state.stun:
                    break;
            }
           
        PlayStateAnimation(_currentState);

        if (health < maxHealth)
        {
            health += healthRegen * Time.deltaTime;
            health = Mathf.Clamp(health, 0, maxHealth);
            healthBar.fillAmount = health / maxHealth;
            healthNum.text = Mathf.RoundToInt(health).ToString();
        }

        if (mana < maxMana)
        {
            mana += manaRegen * Time.deltaTime;
            mana = Mathf.Clamp(mana, 0, maxMana);
            manaBar.fillAmount = mana / maxMana;
            manaNum.text = Mathf.RoundToInt(mana).ToString();
        }
    }

    void FixedUpdate() {Move();}

    protected void Move()
    {
        if (!_initialized) return;

       if (actionState == state.dashing)
        {
            _rb.linearVelocity -= _rb.linearVelocity * friction;
            return;
        }

        if (moveDirection.magnitude > 0)
        {
            _rb.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            _rb.linearVelocity -= _rb.linearVelocity * friction;
        }
    }


    private void FlipSprite(Vector2 direction)
    {
        if (direction.x > 0f)
            _prefabs.transform.localScale = new Vector3(-1.2f, 1.2f, 1f); // face right
        else if (direction.x < 0f)
            _prefabs.transform.localScale = new Vector3(1.2f, 1.2f, 1f);  // face left
    }

    private void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);

        foreach (Collider2D hit in hits)
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact(this);
                return;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        healthBar.fillAmount = health / maxHealth;
        healthNum.text = Mathf.RoundToInt(health).ToString();

        GetComponent<PlayerAudio>()?.EnterCombat();

        if (health <= 0)
            _currentState = PlayerState.DEATH;
        else
            _currentState = PlayerState.DAMAGED;
    }

    public void HealDamage(float damage)
    {
        health = Mathf.Clamp(health + damage, 0, maxHealth);
        healthBar.fillAmount = health / maxHealth;
        healthNum.text = Mathf.RoundToInt(health).ToString();
    }

    public float GetMana()
    {
        return mana;
    }

    public void UseMana(float use)
    {
        mana = Mathf.Clamp(mana - use, 0, maxMana);
        manaBar.fillAmount = mana / maxMana;
    }

    public void setAttack(PlayerMove attk)
    {
        moveAttack = attk;
    }

    public void setSpec(PlayerMove spec)
    {
        moveSpecial = spec;
    }

    public void setDodge(PlayerMove dodge)
    {
        moveDash = dodge;
    }

    public void setUtil(PlayerMove util)
    {
        moveUtil = util;
    }

    public void SetVelocity(Vector2 velocity)
    {
        _rb.linearVelocity = velocity;
    }

    public Vector2 getMoveDirection()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input != Vector2.zero)
            lastMoveDir = input.normalized;

        return lastMoveDir;
    }

    public void setState(state newState)
    {
        actionState = newState;
    }

    public Vector2 CalcShootDir()
    {
        //Debug.Log("INSIDE CALC MousePos: " + GetMousePos());
        Vector2 shootDirection = (GetMousePos() - new Vector2(transform.position.x, transform.position.y)).normalized;
        //Debug.Log("shootDirection: " + GetMousePos());
        //changes to player rotation based on mouse pos
        //player.transform.eulerAngles = new Vector3(0, 0, -90 + Mathf.Atan2(shootDirection.y, shootDirection.x) * 180 / Mathf.PI);
        return shootDirection;
    }

    public Vector2 GetMousePos()
    {
        Vector2 screenPos = lookAction.ReadValue<Vector2>();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return new Vector2(worldPos.x, worldPos.y);
    }

    public void EquipMove(PlayerMove movePrefab)
    {
        PlayerMove newMove = Instantiate(movePrefab);
        PlayerMove oldMove = null;

        switch (newMove.slotType)
        {
            case MoveSlotType.Attack:
                oldMove = moveAttack;
                moveAttack = newMove;
                attackUI.setMove(moveAttack);
                break;

            case MoveSlotType.Special:
                oldMove = moveSpecial;
                moveSpecial = newMove;
                specUI.setMove(moveSpecial);
                break;

            case MoveSlotType.Dash:
                oldMove = moveDash;
                moveDash = newMove;
                dashUI.setMove(moveDash);
                break;

            case MoveSlotType.Util:
                oldMove = moveUtil;
                moveUtil = newMove;
                utilUI.setMove(moveUtil);
                break;
        }

        if (oldMove != null)
        {
            GameObject pickup = Instantiate(oldMove.pickupPrefab, transform.position + transform.forward, Quaternion.identity);
            SpriteRenderer sr = pickup.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = GetComponent<SortingGroup>().sortingLayerName;
            }

            if (oldMove.gameObject != gameObject)
            {
                Destroy(oldMove.gameObject);
            }
        }

        newMove.transform.SetParent(transform);
        newMove.transform.localPosition = Vector3.zero;
    }

    public float getAttackStat()
    {
        return attack;
    }


    public float getSpecAttackStat()
    {
        return specAttack;
    }
}