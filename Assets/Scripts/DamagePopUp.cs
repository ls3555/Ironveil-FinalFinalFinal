using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    public float floatSpeed = 1.5f;
    public float lifetime = 0.8f;

    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetDamage(float dmg)
    {
        text.text = dmg.ToString("0");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += Vector3.up * floatSpeed * Time.deltaTime;
    }
}
