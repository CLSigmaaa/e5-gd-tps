using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class ShootingZombieLocomotion : MonoBehaviour, IEnemy
{
    public Transform playerTransform;
    public EnemyRaycastWeapon enemyRaycastWeapon;
    public float lostSightRange = 30f;
    public float minDistanceToPlayer = 10f;
    public float attackRange = 15f; // Nouvelle variable pour la portée d'attaque
    public float speed = 0.5f;

    [Header("Enemy Rig")]
    public Rig aimingPos;

    NavMeshAgent agent;
    Animator anim;
    PlayerHealth playerHealth;
    bool playerMadeNoise = false;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        playerHealth = playerTransform.GetComponent<PlayerHealth>();

        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float angleToPlayer = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);

        if (distanceToPlayer > lostSightRange)
        {
            playerMadeNoise = false;
            anim.SetFloat("Speed", 0);
            agent.ResetPath();
            aimingPos.weight = 0f;
            enemyRaycastWeapon.StopFiring(); // Stop firing when the player is out of sight
        }
        else if (playerMadeNoise)
        {
            if (distanceToPlayer > minDistanceToPlayer)
            {
                Vector3 targetPosition = playerTransform.position - directionToPlayer * minDistanceToPlayer;

                agent.SetDestination(targetPosition);
                anim.SetFloat("Speed", agent.velocity.magnitude);
            }
            else
            {
                agent.ResetPath();
                anim.SetFloat("Speed", 0);
            }

            if (distanceToPlayer <= attackRange)
            {
                if (Mathf.Abs(angleToPlayer) > 80f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * agent.angularSpeed);
                }
                Shoot();
            }
            else
            {
                aimingPos.weight = 0f;
                enemyRaycastWeapon.StopFiring(); // Stop firing when the player is out of attack range
            }
        }
    }

    public void AggroPlayer()
    {
        playerMadeNoise = true;
    }

    public void OnPlayerNoise()
    {
        playerMadeNoise = true;
    }

    void Shoot()
    {
        aimingPos.weight = 1f;
        enemyRaycastWeapon.StartFiring();
    }
}
