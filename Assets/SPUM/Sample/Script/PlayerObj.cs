using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerObj : MonoBehaviour
{
    public SPUM_Prefabs _prefabs;
    public float _charMS;

    [HideInInspector] public bool isControlled = false;
    [HideInInspector] public bool isAction = false;

    protected PlayerState _currentState;
    protected bool _initialized = false;
    protected Rigidbody2D _rb;

    public Dictionary<PlayerState, int> IndexPair = new();

    protected virtual void Start()
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
            

        _initialized = true;
    }

    public void SetStateAnimationIndex(PlayerState state, int index = 0)
    {
        if (IndexPair.ContainsKey(state))
            IndexPair[state] = index;
    }

    public void PlayStateAnimation(PlayerState state)
    {
        if (!_initialized || _prefabs == null) return;

        string stateName = state.ToString();
        if (!_prefabs.StateAnimationPairs.ContainsKey(stateName))
        {
            foreach (var key in _prefabs.StateAnimationPairs.Keys)
            {
                if (string.Equals(key, stateName, StringComparison.OrdinalIgnoreCase))
                {
                    int idx = IndexPair.ContainsKey(state) ? IndexPair[state] : 0;
                    _prefabs.PlayAnimation(state, idx);
                    return;
                }
            }
            return;
        }

        int index = IndexPair.ContainsKey(state) ? IndexPair[state] : 0;
        _prefabs.PlayAnimation(state, index);
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
            Vector2 input = GetMovementInput();
            if (input.sqrMagnitude > 0.01f)
            {
                _currentState = PlayerState.MOVE;
                FlipSprite(input);
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

        PlayStateAnimation(_currentState);
    }

    void FixedUpdate()
    {
        if (!_initialized) return;

        // Movement via Rigidbody2D so physics/colliders are respected
        if (isControlled && !isAction)
        {
            Vector2 input = GetMovementInput();
            _rb.linearVelocity = input * _charMS;
        }
        else
        {
            // Make sure non-controlled or locked players don't slide
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private Vector2 GetMovementInput()
    {
#if UNITY_6000_0_OR_NEWER
        if (Keyboard.current == null) return Vector2.zero;
        float x = 0f, y = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)  x -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)  y -= 1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)    y += 1f;
#else
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
#endif
        return new Vector2(x, y).normalized;
    }

    private void FlipSprite(Vector2 direction)
    {
        if (direction.x > 0f)
            _prefabs.transform.localScale = new Vector3(-1.2f, 1.2f, 1f); // face right
        else if (direction.x < 0f)
            _prefabs.transform.localScale = new Vector3(1.2f, 1.2f, 1f);  // face left
    }

}