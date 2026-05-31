using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.InputSystem;
#endif
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Party Members — drag all PF Players here")]
    public List<PlayerObj> _partyMembers = new List<PlayerObj>();

    [Header("Selection Indicator (optional)")]
    public Transform _selectionCircle; // a simple circle sprite under the active unit

    [Header("Animation Panel UI (optional)")]
    public RectTransform CommandPanel;
    public Button AnimationButton;
    public Transform AnimationPanelParent;
    public GameObject AnimationPanel;

    private PlayerObj _activePlayer;

    void Start()
    {
        // Default control to the first party member
        if (_partyMembers.Count > 0)
            SetActivePlayer(_partyMembers[0]);

        if (CommandPanel != null)
            CommandPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        // Move selection circle to active player
        if (_selectionCircle != null && _activePlayer != null)
            _selectionCircle.position = _activePlayer.transform.position;

        // Don't process clicks over UI elements
        if (EventSystem.current.IsPointerOverGameObject()) return;

#if UNITY_6000_0_OR_NEWER
        bool clicked = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        Vector2 mousePos = clicked ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        bool clicked = Input.GetMouseButtonDown(0);
        Vector2 mousePos = Input.mousePosition;
#endif
        if (!clicked) return;

        RaycastHit2D hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(mousePos), Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            PlayerObj clicked_player = hit.collider.GetComponent<PlayerObj>();

            if (clicked_player != null && _partyMembers.Contains(clicked_player))
            {
                SetActivePlayer(clicked_player);

                if (CommandPanel != null)
                {
                    CommandPanel.gameObject.SetActive(true);
                    CreateAnimationPanel(clicked_player);
                }
            }
        }
        else
        {
            // Clicked empty space — hide panel but keep control
            if (CommandPanel != null)
                CommandPanel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Transfers WASD control to the given party member.
    /// All others are set to idle.
    /// </summary>
    void SetActivePlayer(PlayerObj target)
    {
        foreach (var member in _partyMembers)
        {
            member.isControlled = false;
            member.isAction = false; // stop any playing animation lock
        }

        target.isControlled = true;
        _activePlayer = target;

        Debug.Log($"[PlayerManager] Now controlling: {target.name}");
    }

    // ─── Animation Panel ─────────────────────────────────────────────────────

    void CreateAnimationPanel(PlayerObj Unit)
    {
        foreach (Transform item in AnimationPanelParent)
            Destroy(item.gameObject);

        var Info = Unit._prefabs.StateAnimationPairs;
        foreach (var StateName in Info.Keys)
        {
            var Panel = Instantiate(AnimationPanel, AnimationPanelParent);
            Panel.GetComponentInChildren<Text>().text = $"{StateName} State";
            var parentTransform = Panel.GetComponentInChildren<ContentSizeFitter>().transform;

            foreach (var clip in Info[StateName])
            {
                var btn = Instantiate(AnimationButton, parentTransform);
                btn.GetComponentInChildren<Text>().text = clip.name;

                string capturedState = StateName;
                AnimationClip capturedClip = clip;

                btn.onClick.AddListener(() =>
                {
                    if (Enum.TryParse(capturedState, true, out PlayerState state))
                    {
                        Unit.isAction = true;
                        int index = Info[capturedState].FindIndex(x => x == capturedClip);
                        Unit._prefabs._anim.Rebind();
                        Unit.SetStateAnimationIndex(state, index);
                        Unit.PlayStateAnimation(state);
                    }
                });
            }
        }
    }
}