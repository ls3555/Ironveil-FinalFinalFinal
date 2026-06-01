using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    public PlayerMove move;
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private Image skillIcon;

    public void setMove(PlayerMove newMove)
    {
        move = newMove;
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