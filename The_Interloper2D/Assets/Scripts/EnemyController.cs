using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private int lifes = 10;
    private EnemyState currentState;
    private float detectionRange = 10f;
    private float attackRange = 5f;
    private float avoidHealthThreshold = 3;
    private float wanderRadius = 5f;
    private float wanderTimer = 1f;
    private float timer;
    private bool canFire;
    private float shootingTimer;
    private float timeBeforeFiring = 0.2f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletTransform;
    [SerializeField] private Transform rotatePoint;
    [SerializeField] private Transform target;
    NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;

        timer = wanderTimer;
        currentState = EnemyState.Wander;
    }

    void FixedUpdate()
    {
        if (lifes <= 0)
        {
            Destroy(gameObject);
        }

        //agent.SetDestination(target.position);
        checkState();
    }

    private void checkState()
    {
        switch (currentState)
        {
            case EnemyState.Wander:
                Wander();
                if (Vector3.Distance(transform.position, target.position) < detectionRange)
                {
                    currentState = EnemyState.Chase;
                }
            break;

            case EnemyState.Chase:
                Chase();
                if (Vector3.Distance(transform.position, target.position) > detectionRange)
                {
                    currentState = EnemyState.Wander;
                }
                if (Vector3.Distance(transform.position, target.position) < attackRange)
                {
                    currentState = EnemyState.Attack;
                }
                if (lifes < avoidHealthThreshold)
                {
                    currentState = EnemyState.Avoid;
                }
                break;

            case EnemyState.Attack:
                Attack();
                if(Vector3.Distance(transform.position, target.position) > attackRange)
                {
                    currentState = EnemyState.Wander;
                }
                if(lifes < avoidHealthThreshold)
                {
                    currentState = EnemyState.Avoid;
                }
                break;

            case EnemyState.Avoid:
                Avoid();
                if (Vector3.Distance(transform.position, target.position) > detectionRange && lifes == 10)
                {
                    currentState = EnemyState.Wander;
                }
                break;
        }
    }

    private void Wander()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    private void Chase()
    {
        agent.SetDestination(target.position);
    }

    private void Attack()
    {
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(target.position);
        Vector3 rotation = targetPosition - rotatePoint.position;
        float angle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        rotatePoint.rotation = Quaternion.Euler(0,0,angle);

        if (!canFire)
        {
            shootingTimer = Time.deltaTime;
            if(shootingTimer > timeBeforeFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        canFire = false;
        GameObject bullet = Instantiate(bulletPrefab, bulletTransform.position, Quaternion.identity);
        bullet.gameObject.GetComponent<BulletController>().owner = "enemy";
    }

    private void Avoid()
    {
        Vector3 dirToPlayer = transform.position - target.position;
        Vector3 newPos = transform.position + dirToPlayer;
        agent.SetDestination(newPos);

        timer += Time.deltaTime;

        if(timer >= 2f)
        {
            lifes++;
            Debug.Log(lifes);
            timer = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BulletController>().owner == "player")
        {
            if (lifes > 0)
            {
                lifes--;
            }
        }
    }

    private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layerMask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navhit;
        NavMesh.SamplePosition(randDirection, out navhit, dist, layerMask);
        return navhit.position;
    }
}
