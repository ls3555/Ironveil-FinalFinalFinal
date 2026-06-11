using UnityEngine;
using System.Collections;

public class Friend : MonoBehaviour
{
    [SerializeField]private float LifeTime;
    [SerializeField] Vector3 offset;
    private string target;
    protected Rigidbody2D rb;
    public ParticleSystem healPulse;

    public float damage;
    public float speed;
    private Transform targetLocation;
    private Vector3 targetPos;

    private PlayerMovement player;  
    public float healCooldown = 3f;
    private float nextHealTime = 0f;

    public void setTarget(string name,Vector3 dir,float force, PlayerMovement follow){
        target = name;
        rb = GetComponent<Rigidbody2D>();
        speed=force;
        transform.up=dir;
        player = follow;
        targetLocation = player.transform;

        if (targetLocation == null) return;

        StartCoroutine(startCountdown());
    }

    public IEnumerator startCountdown(){
        yield return new WaitForSeconds(LifeTime);
        Destroy(this.gameObject);
    }

    public void setDamage(float stat)
    {
        damage = stat;
    }

    private void Update()
    {
        if (targetLocation == null) return;

        if (player.getDir().x > 0)
            offset.x = -1;
        else if (player.getDir().x < 0)
             offset.x = 1;


        targetPos = targetLocation.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);

        if (Time.time >= nextHealTime){
            ParticleSystem particle = Instantiate(healPulse,transform.position,Quaternion.identity);
            player.HealDamage(damage);
            Destroy(particle.gameObject,0.5f);
            nextHealTime = Time.time + healCooldown;
        }
    }
}