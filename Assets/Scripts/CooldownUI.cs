using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    public PlayerMove move;
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private Image iconDisplay;

    void Start()
    {
        iconDisplay.sprite  = move.getIcon();
    }
    public void setMove(PlayerMove newMove)
    {
        move = newMove;
        iconDisplay.sprite  = move.getIcon();
    }

    void Update()
    {
        if (move.IsOnCooldown())
        {
            cooldownText.text = move.CooldownRemaining().ToString("F1");
            
        }
        else
        {
            manaText.text = move.getManaCost().ToString("F1");
            cooldownText.text = "";
        }
        cooldownImage.fillAmount = move.CooldownRemaining() / move.cooldown;
    }
}