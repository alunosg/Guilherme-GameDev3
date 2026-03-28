using System.Runtime.CompilerServices;
using UnityEngine;

public class Bullet : MonoBehaviour

{
    public Rigidbody rig;
    public float speed = 50;
    public float lifeTime = 5;
    public GameObject hitParticle;
    public GameObject destroyParticle;

    private void Start()
    {
        rig.linearVelocity += transform.forward * speed;
        Invoke(nameof(AutoDestroy), lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(hitParticle, transform.position, Quaternion.identity);
            //Hit Player
        }
        else if (other.CompareTag("Enemy"))
        {
            Instantiate(hitParticle, transform.position, Quaternion.identity);
            //Hit Enemy
        }

        AutoDestroy();

    }

    private void AutoDestroy()

    {
        Instantiate(destroyParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
