using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour
{   
    [SerializeField]private float LifeTime;
    private string target;
    protected Rigidbody2D rb;
    public ParticleSystem fire;
    public float damage;
    protected float speed;

    public void setTarget(string name,Vector3 dir){
        target = name;
        rb = GetComponent<Rigidbody2D>();
        transform.up=dir;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
        Vector3 euler = rotation.eulerAngles;

        ParticleSystem particle = Instantiate(fire,transform.position,Quaternion.Euler(euler) );
        Destroy(particle.gameObject,LifeTime);
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
        Destroy(this.gameObject);}
    }

    public void setDamage(float stat)
    {
        damage = stat * 1.2f;
    }
}