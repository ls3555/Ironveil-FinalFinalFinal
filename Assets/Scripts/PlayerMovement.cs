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
    public bool isDashing;

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
        moveDirection = moveAction.ReadValue<Vector2>().normalized;
        if(attackAction.WasPressedThisFrame() && moveAttack != null){StartCoroutine(moveAttack.Execute());}
        if(specialAction.WasPressedThisFrame()&& moveSpecial != null){StartCoroutine(moveSpecial.Execute());}
        if(dodgeAction.WasPressedThisFrame() && moveDash != null){StartCoroutine(moveDash.Execute());}
        if(utilAction.WasPressedThisFrame() && moveUtil != null){StartCoroutine(moveUtil.Execute());}
    }

    protected override void Move()
    {
        if (isDashing)
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
}
