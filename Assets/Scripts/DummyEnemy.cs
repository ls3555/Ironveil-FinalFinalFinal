using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class DummyEnemy : Entity
{
    public Image healthBar;
    public GameObject healthContainer;
    public GameObject canvas;
    public float healthRegen;
    public float hideDelay = 2f;
    private float lastDamageTime;
    private Coroutine hideHPRoutine;
    public TextMeshProUGUI damagePopup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthContainer.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (health < maxHealth)
            {
                health += healthRegen * Time.deltaTime;
                health = Mathf.Clamp(health, 0, maxHealth);
                healthBar.fillAmount = health / maxHealth;
            }
    }

    public override void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        healthBar.fillAmount = health / maxHealth;
        lastDamageTime = Time.time;

        ShowDamage(damage);

        if (hideHPRoutine != null)
            StopCoroutine(hideHPRoutine);

        hideHPRoutine = StartCoroutine(HideHealthBar());
    }

    void ShowDamage(float damage)
    {
        Vector3 randomOffset = new Vector3(
        Random.Range(-0.5f, 0.5f),
        Random.Range(0.5f, 1.5f),
        0f);

        Vector3 spawnPos = transform.position + randomOffset;

        TextMeshProUGUI popup = Instantiate(damagePopup,transform.position,Quaternion.identity);
        popup.transform.SetParent(canvas.transform, false);
        popup.transform.position = spawnPos;
        popup.text = "" +damage;
    }

    IEnumerator HideHealthBar()
    {
        healthContainer.gameObject.SetActive(true);

        while (Time.time - lastDamageTime < hideDelay)
            yield return null;

        healthContainer.gameObject.SetActive(false);
    }

    protected override void Move()
    {
        rigidBody.linearVelocity -= rigidBody.linearVelocity * friction;
    }
}
