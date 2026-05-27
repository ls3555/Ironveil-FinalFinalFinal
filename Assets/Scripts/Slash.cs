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

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == target){
        IDamagable target = other.transform.GetComponent<IDamagable>();
        if(target != null){target.TakeDamage(damage);};
        Destroy(this.gameObject);}
    }
}