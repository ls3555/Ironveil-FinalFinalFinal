using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : Movement
{ [Header("Entity Settings")]

    protected SpriteRenderer spriteRenderer;
<<<<<<< HEAD
    public float health;
    protected float maxHealth;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float friction;
    protected Vector2 moveDirection;
=======
>>>>>>> test-merging
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
    [SerializeField] private Image healthBar;
    [SerializeField] private TMP_Text healthNum;
    [SerializeField] private Image manaBar;
    [SerializeField] private TMP_Text manaNum;

    [SerializeField] private CooldownUI attackUI;
    [SerializeField] private CooldownUI specUI;
    [SerializeField] private CooldownUI dashUI;
    [SerializeField] private CooldownUI utilUI;

    state actionState;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;

        maxHealth = health;
        maxMana = mana;
    }

    protected override void Start()
    {
        base.Start();

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
    }

    // ---------------------------------------------------------
    // OVERRIDES FROM Movement.cs
    // ---------------------------------------------------------

    protected override void HandleState()
    {
        if (isAction) return;

        if (isControlled)
        {
            moveDirection = canMove ? moveAction.ReadValue<Vector2>().normalized : Vector2.zero;
            isMoving = moveDirection.sqrMagnitude > 0.01f;

<<<<<<< HEAD
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
=======
            if (isMoving)
            {
                currentState = PlayerState.MOVE;
>>>>>>> test-merging
                FlipSprite(moveDirection);
            }
            else
            {
                currentState = PlayerState.IDLE;
            }
        }
        else
        {
<<<<<<< HEAD
            _currentState = PlayerState.IDLE;
        }

        switch (actionState)
        {
            case state.idle:
                if (interactAction.WasPressedThisFrame()) { TryInteract(); }
                if (attackAction.WasPressedThisFrame() && moveAttack != null) { StartCoroutine(moveAttack.Execute()); _currentState = PlayerState.ATTACK; }
                if (specialAction.WasPressedThisFrame() && moveSpecial != null) { StartCoroutine(moveSpecial.Execute()); _currentState = PlayerState.ATTACK; }
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
=======
            currentState = PlayerState.IDLE;
        }

        // Ability state machine
        switch (actionState)
        {
            case state.idle:
                if (interactAction.WasPressedThisFrame()) TryInteract();
                if (attackAction.WasPressedThisFrame() && moveAttack != null) { StartCoroutine(moveAttack.Execute()); currentState = PlayerState.ATTACK; }
                if (specialAction.WasPressedThisFrame() && moveSpecial != null) { StartCoroutine(moveSpecial.Execute()); currentState = PlayerState.ATTACK; }
                if (dodgeAction.WasPressedThisFrame() && moveDash != null) StartCoroutine(moveDash.Execute());
                if (utilAction.WasPressedThisFrame() && moveUtil != null) StartCoroutine(moveUtil.Execute());
                break;
            case state.attacking:
                if (utilAction.WasPressedThisFrame() && moveUtil != null) StartCoroutine(moveUtil.Execute());
                break;
            case state.dashing:
                if (attackAction.WasPressedThisFrame() && moveAttack != null) StartCoroutine(moveAttack.Execute());
                if (specialAction.WasPressedThisFrame() && moveSpecial != null) StartCoroutine(moveSpecial.Execute());
                if (utilAction.WasPressedThisFrame() && moveUtil != null) StartCoroutine(moveUtil.Execute());
                break;
>>>>>>> test-merging

            case state.stun:
                break;
        }

        RegenStats();
    }

    protected override void Move()
    {
        if (!_initialized) return;
        if (actionState == state.dashing)
        {
            rb.linearVelocity -= rb.linearVelocity * friction;
            return;
        }

        if (moveDirection.magnitude > 0)
            rb.linearVelocity = moveDirection * moveSpeed;
        else
            rb.linearVelocity -= rb.linearVelocity * friction;
    }

    // ---------------------------------------------------------
    // PLAYER-SPECIFIC METHODS
    // ---------------------------------------------------------

    private void RegenStats()
    {
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

<<<<<<< HEAD
    void FixedUpdate() { Move(); }

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

=======
>>>>>>> test-merging
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

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        healthBar.fillAmount = health / maxHealth;
        healthNum.text = Mathf.RoundToInt(health).ToString();

        GetComponent<PlayerAudio>()?.EnterCombat();
<<<<<<< HEAD

        if (health <= 0)
        {
            _currentState = PlayerState.DEATH;
            canMove = false;
            SetVelocity(Vector2.zero);
            StartCoroutine(DeathDelay());
        }
        else
            _currentState = PlayerState.DAMAGED;
=======
>>>>>>> test-merging
    }

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(1f);
        GameController.Instance.PlayerDied();
    }

    public void HealDamage(float damage)
    {
        health = Mathf.Clamp(health + damage, 0, maxHealth);
        healthBar.fillAmount = health / maxHealth;
        healthNum.text = Mathf.RoundToInt(health).ToString();
    }

    public Vector2 CalcShootDir()
    {
        Vector2 screenPos = lookAction.ReadValue<Vector2>();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return ((Vector2)worldPos - (Vector2)transform.position).normalized;
    }

    public void EquipMove(PlayerMove movePrefab)
    {
        // unchanged — this is purely player-specific
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


    public float getAttackStat()
    {
        return attack;
    }


    public float getSpecAttackStat()
    {
        return specAttack;
    }

}
