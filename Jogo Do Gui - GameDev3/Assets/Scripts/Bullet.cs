using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody rig;
    public float damage = 1;
    public float speed = 50;
    public float lifeTime = 5;
    public GameObject hitParticle;
    public GameObject destroyParticle;

    public bool hitPlayer = false;
    public bool hitEnemy = true;

    private void Start()
    {
        rig.linearVelocity += transform.forward * speed;
        Invoke(nameof(AutoDestroy), lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitPlayer && other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().GetHit(damage);
            Instantiate(hitParticle, transform.position, Quaternion.identity);
        }
        if (hitEnemy && other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().GetHit(damage);
            Instantiate(hitParticle, transform.position, Quaternion.identity);
        }

        AutoDestroy();
    }

    private void AutoDestroy()
    {
        Instantiate(destroyParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
