using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public abstract class EnemyUI : Entity
{
    public Image healthBar;
    public GameObject healthContainer;
    public GameObject canvas;
    public float hideDelay = 2f;
    protected float lastDamageTime;
    protected Coroutine hideHPRoutine;
    public TextMeshProUGUI damagePopup;

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
        Random.Range(-0.5f, 0.5f),
        Random.Range(-1f, 1f),
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
