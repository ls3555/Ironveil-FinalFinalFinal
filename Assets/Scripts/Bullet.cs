using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField]private float LifeTime;
    private string target;
    protected Rigidbody2D rb;
    public ParticleSystem hit;
    public float damage;
    protected float speed;

    public void setTarget(string name,Vector3 dir,float force){
        target = name;
        rb = GetComponent<Rigidbody2D>();
        speed=force;
        transform.up=dir;
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

        if(other.gameObject.tag != "Stairs") {
            ParticleSystem particle = Instantiate(hit,transform.position, Quaternion.identity);
            Destroy(particle.gameObject,2);
            Destroy(this.gameObject);
        }
    }


    public void setDamage(float stat)
    {
        damage = Mathf.Round(stat);
    }

    void FixedUpdate(){
        rb.linearVelocity = transform.up*speed;
    }
    
}