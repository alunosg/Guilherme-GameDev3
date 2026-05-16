using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum EnemyStates { Idle, Chase, Attack }
    public EnemyStates state = EnemyStates.Idle;

    public NavMeshAgent agent;
    public Animator anim;

    public float chaseDistance = 30;
    public float attackDistance = 1.5f;

    public Transform attackPoint;
    public float attackRange = 1.5f;
    public float attackDelay = 0.5f;
    public float damage = 1.0f;

    public float attackDuration = 1.0f;
    public float hitDuration = 1.0f;
    public float deathDuration = 1.0f;

    public GameObject attackFX, attackHitFX, hitFX, deathFX;

    public float hp = 3;
    private float maxHp;

    private float distance;
    private PlayerController player;

    private bool locked = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHp = hp;
        player = FindFirstObjectByType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (locked) return;

        distance = Vector3.Distance(transform.position, player.transform.position);

        switch (state)
        {
            case EnemyStates.Idle:
                IdleState();
                break;
            case EnemyStates.Chase:
                ChaseState();
                break;
            case EnemyStates.Attack:
                AttackState();
                break;
        }
    }

    void IdleState()
    {
        if (distance < chaseDistance)
        {
            if (distance < attackDistance)
            {
                EnterAttack();
            }
            else
            {
                state = EnemyStates.Chase;
                agent.isStopped = false;
                anim.SetBool("IsWalking", true);
            }
        }
    }

    void ChaseState()
    {
        agent.isStopped = false;
        anim.SetBool("IsWalking", true);
        agent.SetDestination(player.transform.position);

        if (distance > chaseDistance)
        {
            state = EnemyStates.Idle;
            agent.isStopped = true;
            anim.SetBool("IsWalking", false);
        }
        else if (distance < attackDistance)
        {
            EnterAttack();
        }
    }

    void AttackState()
    {
        if (distance > chaseDistance)
        {
            state = EnemyStates.Idle;
            agent.isStopped = true;
            anim.SetBool("IsWalking", false);
        }
        else
        {
            if (distance < attackDistance)
            {
                EnterAttack();
            }
            else
            {
                state = EnemyStates.Chase;
                agent.isStopped = false;
                anim.SetBool("IsWalking", true);
            }
        }
    }

    void EnterAttack()
    {
        state = EnemyStates.Attack;
        agent.isStopped = true;
        anim.SetTrigger("Attack");
        anim.SetBool("IsWalking", false);
        locked = true;
        CancelInvoke(nameof(Unlock));
        Invoke(nameof(Unlock), attackDuration);
        CancelInvoke(nameof(DealDamage));
        Invoke(nameof(DealDamage), attackDelay);
        if (attackFX) Instantiate(attackFX, attackPoint.position, attackPoint.rotation);
    }

    void Unlock() => locked = false;

    void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].CompareTag("Player"))
            {
                hits[i].GetComponent<PlayerController>().GetHit(damage);
                if (attackHitFX) Instantiate(attackHitFX, attackPoint.position, attackPoint.rotation);
                break;
            }
        }
    }

    public void GetHit(float damage)
    {
        if (hp > 0)
        {
            locked = true;

            CancelInvoke(nameof(Unlock));
            CancelInvoke(nameof(DealDamage));

            hp -= damage;
            if (hp <= 0)
            {
                anim.SetTrigger("Death");
                Invoke(nameof(AutoDestroy), deathDuration);
                if (deathFX) Instantiate(deathFX, transform.position, transform.rotation);
            }
            else
            {
                anim.SetTrigger("Hit");
                Invoke(nameof(Unlock), hitDuration);
            }

            agent.isStopped = true;
            anim.SetBool("IsWalking", false);
            if (hitFX) Instantiate(hitFX, transform.position, transform.rotation);
        }
    }

    void AutoDestroy()
    {
        Destroy(gameObject);
    }
}