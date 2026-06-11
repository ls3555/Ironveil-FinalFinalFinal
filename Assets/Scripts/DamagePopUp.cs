using UnityEngine;
using System.Collections;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    public float lifetime = 1f;
    public float height = 0.5f;
    public float horizontalOffset = 0.3f;

    private Vector3 startPos;
    private Vector3 drift;
    private float timer;

    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetDamage(float dmg)
    {
        text.text = dmg.ToString("0.0");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        drift = Vector3.right * Random.Range(-horizontalOffset, horizontalOffset);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        float t = timer / lifetime;

        Vector3 pos = startPos;
        pos += Vector3.up * (height * t);

        pos += Vector3.up * Mathf.Sin(t * Mathf.PI) * 0.5f;

        pos += drift * t;
        transform.position = pos;
        
        if (text != null)
        {
            Color c = text.color;
            c.a = 1f - t;
            text.color = c;
        }

        if (t >= 1f)
            Destroy(gameObject);
    }
}
