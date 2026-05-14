using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : Entity
{
    public static PlayerMovement Instance;

    private PlayerInput input;
    private InputAction moveAction; 
    private InputAction attackAction; 
    private InputAction specialAction; 
    private InputAction dodgeAction; 
    private InputAction utilAction; 

    [SerializeField] private PlayerMove moveAttack;
    [SerializeField] private PlayerMove moveSpecial;
    [SerializeField] private PlayerMove moveDash;
    [SerializeField] private PlayerMove moveUtil;

    private Vector2 lastMoveDir = Vector2.right;
    public bool isMoving;
    public bool canMove = true;

    private enum state{idle,attacking,dashing,stun};
    state currentState;
    private Animator animator;

    protected override void Awake()
    {
        base.Awake();
        input = new PlayerInput();
        moveAction = input.Player.Move;
        attackAction = input.Player.Attack;
        specialAction = input.Player.Special;
        dodgeAction = input.Player.Dodge;
        utilAction = input.Player.Utility;
        Instance = this;
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
    
    public override void TakeDamage(int damage)
    {
        health-=damage;
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

    public void stateDashing()
    {
        currentState = state.dashing;
    }
    public void stateStun()
    {
        currentState = state.stun;
    }
    public void stateAttack()
    {
        currentState = state.attacking;
    }
    public void stateIdle()
    {
        currentState = state.idle;
    }
}
