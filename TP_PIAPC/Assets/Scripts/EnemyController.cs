using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private EnemyState currentState;
    private float currentHealth;
    private EnemyHealthBar healthBar;
    private float detectionRange = 10f;
    private float attackRange = 5f;
    private float avoidHealthThreshold = 30f;
    private float wanderRadius = 3f;
    private float wanderTimer = 1.5f;
    private float timer;
    private bool canFire;
    private float shootingTimer;
    private float timeBeforeFiring = 0.2f;
    private Quaternion targetRotation;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletTransform;
    [SerializeField] private Transform _rotatePoint;
    [SerializeField] private Transform _target;

    NavMeshAgent agent;
    void Start()
    {
        healthBar = GetComponent<EnemyHealthBar>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;

        timer = wanderTimer;
        currentState = EnemyState.Wander;
    }

    void FixedUpdate()
    {
        currentHealth = GetComponent<EnemyHealthBar>().currentHealth;

        checkState();
        SmoothRotate();
    }

    private void checkState()
    {
        float distanteToTarget = Vector3.Distance(transform.position, target.position);

        switch (currentState)
        {
            case EnemyState.Wander:
                Wander();
                if (distanteToTarget < detectionRange)
                {
                    currentState = EnemyState.Chase;
                }
            break;

            case EnemyState.Chase:
                Chase();
                if (distanteToTarget > detectionRange)
                {
                    currentState = EnemyState.Wander;
                }
                else if (distanteToTarget < attackRange)
                {
                    currentState = EnemyState.Attack;
                }
                else if (currentHealth < avoidHealthThreshold)
                {
                    currentState = EnemyState.Avoid;
                }
                break;

            case EnemyState.Attack:
                Attack();
                if(distanteToTarget > attackRange)
                {
                    currentState = EnemyState.Wander;
                }
                else if(currentHealth < avoidHealthThreshold)
                {
                    currentState = EnemyState.Avoid;
                }
                break;

            case EnemyState.Avoid:
                Avoid();
                if (currentHealth == healthBar.maxHealth)
                {
                    currentState = distanteToTarget > detectionRange ? EnemyState.Wander : EnemyState.Chase;
                }
                break;
        }
    }

    private void Wander()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(rotatePoint.position, wanderRadius, -1);
            RotateTo(newPos);

            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    private void Chase()
    {
        Vector3 targetPosition = target.position;
        RotateTo(targetPosition);

        agent.SetDestination(target.position);
    }

    private void Attack()
    {
        Vector3 targetPosition = target.position;
        RotateTo(targetPosition);
        agent.SetDestination(transform.position);

        shootingTimer += Time.deltaTime;
        if (shootingTimer > timeBeforeFiring)
        {
            canFire = true;
        }

        if (canFire)
        {
            canFire = false;
            shootingTimer = 0;
            GameObject bullet = Instantiate(bulletPrefab, bulletTransform.position, Quaternion.identity);
            BulletController bulletScript = bullet.GetComponent<BulletController>();
            bulletScript.owner = "enemy";
            bulletScript.enemyController = this;
        }
    }

    private void Avoid()
    {
        Vector3 dirToPlayer = transform.position - target.position;
        Vector3 newPos = transform.position + dirToPlayer;
        RotateTo(newPos);

        agent.SetDestination(newPos);

        timer += Time.deltaTime;

        if(timer >= 2f)
        {
            healthBar.UpdateHealth(10f);
            timer = 0;
        }
    }

    private void RotateTo(Vector3 target)
    {
        Vector3 direction = (target - rotatePoint.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetRotation = Quaternion.Euler(0, 0, angle);
    }

    private void SmoothRotate()
    {
        rotatePoint.rotation = Quaternion.Slerp(rotatePoint.rotation, targetRotation, Time.deltaTime * 4f);
    }

    private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layerMask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navhit;
        NavMesh.SamplePosition(randDirection, out navhit, dist, layerMask);
        return navhit.position;
    }



    //Properties
    public Transform rotatePoint
    {
        get { return _rotatePoint; }
    }
    public Transform target
    {
        get { return _target; }
    }
}
