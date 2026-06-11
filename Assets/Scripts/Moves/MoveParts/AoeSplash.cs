using System.Collections;
using UnityEngine;

public class AoeSplash : MonoBehaviour
{
    [SerializeField]private float LifeTime;
    private string target;
    protected Rigidbody2D rb;
    public ParticleSystem splash;
    public float damage;

    public void setTarget(string name,Vector3 dir){
        target = name;
        rb = GetComponent<Rigidbody2D>();
        transform.up=dir;
        Quaternion rot = transform.rotation;
        Vector3 euler = rot.eulerAngles;
        euler.z += 180f;

        ParticleSystem particle = Instantiate(splash,transform.position,Quaternion.Euler(euler));
        Destroy(particle.gameObject,3);
        StartCoroutine(startCountdown());
    }

    public IEnumerator startCountdown(){
        yield return new WaitForSeconds(LifeTime);
        Destroy(this.gameObject);}

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == target){
            if (other.transform.TryGetComponent(out IDamagable target))
            {
                target.TakeDamage(damage);
            }
        }
    }


    public void setDamage(float stat)
    {
        damage = stat * 1.5f;
    }


}