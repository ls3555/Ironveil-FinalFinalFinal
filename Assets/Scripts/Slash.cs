using System.Collections;
using UnityEngine;

public class Slash : MonoBehaviour
{

    [SerializeField]private float LifeTime;
    private string target;
    protected Rigidbody2D rb;
    public float damage;
    protected float speed;

    public void setTarget(string name,Vector3 dir){
        target = name;
        rb = GetComponent<Rigidbody2D>();
        transform.up=dir;
        StartCoroutine(startCountdown());
    }

    public IEnumerator startCountdown(){
        yield return new WaitForSeconds(LifeTime);
        Destroy(this.gameObject);}

    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.tag == target){
        if (other.transform.TryGetComponent(out IDamagable target))
        {
            target.TakeDamage(damage);
        }
        Destroy(this.gameObject);}
    }
}