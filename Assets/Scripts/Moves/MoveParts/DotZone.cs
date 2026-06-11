using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DotZone : MonoBehaviour
{
    [SerializeField]private float LifeTime;
    private string target;
    protected Rigidbody2D rb;
    public ParticleSystem zone;
    public float damage;
    public float dmgInterval = 0.5f;
    public float dmgModifier = 0.2f;

    private Dictionary<Collider2D, float> nextDamageTime = new();

    public void setTarget(string name,Vector3 dir){
        target = name;
        rb = GetComponent<Rigidbody2D>();
        transform.up=dir;
        Quaternion rot = transform.rotation;
        Vector3 euler = rot.eulerAngles;
        euler.z += 180f;

        ParticleSystem particle = Instantiate(zone,transform.position,Quaternion.Euler(euler));
        particle.transform.SetParent(gameObject.transform);
        particle.Play();
        Destroy(particle.gameObject,LifeTime);
        StartCoroutine(startCountdown());
    }

    public IEnumerator startCountdown(){
        yield return new WaitForSeconds(LifeTime);
        Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(target))
            return;

        if (!other.TryGetComponent<IDamagable>(out var damageable))
            return;

        if (!nextDamageTime.ContainsKey(other))
            nextDamageTime[other] = Time.time;

        if (Time.time >= nextDamageTime[other])
        {
            damageable.TakeDamage(damage);
            nextDamageTime[other] = Time.time + dmgInterval;
        }
    }


    public void setDamage(float stat)
    {
        damage = stat * dmgModifier;
    }

}