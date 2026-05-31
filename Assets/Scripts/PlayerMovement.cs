using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class PlayerMovement : Entity
{
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
    state currentState;
    private Animator animator;

    public Image healthBar;
    public Image manaBar;

    protected override void Awake()
    {
        base.Awake();
        input = new PlayerInput();
        moveAction = input.Player.Move;
        lookAction = input.Player.Look;
        attackAction = input.Player.Attack;
        specialAction = input.Player.Special;
        dodgeAction = input.Player.Dodge;
        utilAction = input.Player.Utility;
        interactAction = input.Player.Interact;
        Instance = this;
        maxMana = mana;
        currentState = state.idle;
    }

    void Start ()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        input.Player.Enable();
    }

    void OnDisable()
    {
        input.Player.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState){
            case state.idle:
                if(interactAction.WasPressedThisFrame()){TryInteract();}
                if(attackAction.WasPressedThisFrame() && moveAttack != null){StartCoroutine(moveAttack.Execute());}
                if(specialAction.WasPressedThisFrame()&& moveSpecial != null){StartCoroutine(moveSpecial.Execute());}
                if(dodgeAction.WasPressedThisFrame() && moveDash != null){StartCoroutine(moveDash.Execute());}
                if(utilAction.WasPressedThisFrame() && moveUtil != null){StartCoroutine(moveUtil.Execute());}
            break;
            case state.attacking:
                if(utilAction.WasPressedThisFrame() && moveUtil != null){StartCoroutine(moveUtil.Execute());}
            break;
            case state.dashing:
                if(attackAction.WasPressedThisFrame() && moveAttack != null){StartCoroutine(moveAttack.Execute());}
                if(specialAction.WasPressedThisFrame()&& moveSpecial != null){StartCoroutine(moveSpecial.Execute());}
                if(utilAction.WasPressedThisFrame() && moveUtil != null){StartCoroutine(moveUtil.Execute());}
            break;
            case state.stun:
            break;

        }

        if (canMove) {
            moveDirection = moveAction.ReadValue<Vector2>().normalized;
        } else{
            moveDirection = Vector2.zero;
        }

        animator.SetBool("IsMoving", moveDirection.magnitude > 0);

        if (moveDirection.y > 0) {
            animator.SetInteger("Direction", 1);
        } else if (moveDirection.y < 0) {
            animator.SetInteger("Direction", 0);
        } else if (moveDirection.x > 0) {
            animator.SetInteger("Direction", 2);
        } else if (moveDirection.x < 0) {
            animator.SetInteger("Direction", 3);
        }

            if (health < maxHealth)
            {
                health += healthRegen * Time.deltaTime;
                health = Mathf.Clamp(health, 0, maxHealth);
                healthBar.fillAmount = health / maxHealth;
            }

            if (mana < maxMana)
            {
                mana += manaRegen * Time.deltaTime;
                mana = Mathf.Clamp(mana, 0, maxMana);
                manaBar.fillAmount = mana / maxMana;
            }
    }

    //fixed update friction
    protected override void Move()
    {
        if (currentState == state.dashing)
        {
            rigidBody.linearVelocity -= rigidBody.linearVelocity * friction;
            return;
        }

        if (moveDirection.magnitude > 0)
        {
            rigidBody.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            rigidBody.linearVelocity -= rigidBody.linearVelocity * friction;
        }
    }

    private void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,interactRange);

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
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        healthBar.fillAmount = health / maxHealth;
    }

    public void HealDamage(float damage)
    {
        health = Mathf.Clamp(health + damage, 0, maxHealth);
        healthBar.fillAmount = health / maxHealth;
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
        rigidBody.linearVelocity = velocity;
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
        currentState = newState;
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
                break;

            case MoveSlotType.Special:
                oldMove = moveSpecial;
                moveSpecial = newMove;
                break;

            case MoveSlotType.Dash:
                oldMove = moveDash;
                moveDash = newMove;
                break;

            case MoveSlotType.Util:
                oldMove = moveUtil;
                moveUtil = newMove;
                break;
        }

        if (oldMove != null)
        {
            Instantiate(oldMove.pickupPrefab,transform.position + transform.forward,Quaternion.identity);
            if(oldMove.gameObject != gameObject)
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
