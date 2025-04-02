using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicZombieLocomotion : MonoBehaviour, IEnemy
{
    public Transform playerTransform;
    public int damage = 10;
    public float speedWhenFast = 3f; // Vitesse rapide modifiable dans l'inspecteur
    public float speedWhenSlow = 0.75f;  // Vitesse lente modifiable dans l'inspecteur
    public int chanceOutOf = 4;
    public float lostSightRange = 20.0f; // Portée maximale de vision
    public float attackRange = 1.5f; // Portée d'attaque
    public float wanderRadius = 10.0f; // Rayon de déplacement aléatoire
    public float wanderInterval = 5.0f; // Intervalle de temps pour le déplacement aléatoire
    NavMeshAgent agent;
    Animator anim;
    PlayerHealth playerHealth;
    bool playerMadeNoise = false;
    bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        playerHealth = playerTransform.GetComponent<PlayerHealth>();

        // Définir la vitesse de l'agent avec une probabilité de 1 sur chanceOutOf d'être speedWhenFast, sinon speedWhenSlow
        agent.speed = (Random.Range(0, chanceOutOf) == 0) ? speedWhenFast : speedWhenSlow;

        // Commencer la coroutine pour le déplacement aléatoire
        //StartCoroutine(Wander());
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        Debug.Log("Distance to player: " + distanceToPlayer);
        Debug.Log("Player made noise: " + playerMadeNoise);
        if (playerMadeNoise && distanceToPlayer <= lostSightRange)
        {

            if (distanceToPlayer <= attackRange)
            {
                Debug.Log("Attacking player");
                // Attaquer le joueur
                agent.isStopped = true;
                anim.SetTrigger("Attack");
                isAttacking = true;
                //DealDamage();
            }
            else if (!isAttacking)
            {
                // Suivre le joueur
                agent.isStopped = false;
                anim.SetFloat("Speed", agent.speed);
                agent.SetDestination(playerTransform.position);
            }
            else if (isAttacking)
            {
                // Arrêter de suivre le joueur
                Debug.Log("Stopping to follow player");
                // reset destination
                agent.isStopped = true;
            }
            else
            {
                // Arrêter de suivre le joueur
                agent.isStopped = true;
            }
        }
    }

    // Méthode appelée par l'événement d'animation
    public void DealDamage()
    {
        Debug.Log("Dealing damage");
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        // Dessine un gizmo pour indiquer que le joueur a été touché
        //Debug.DrawLine(transform.position, playerTransform.position, Color.red, 1.0f);
        if (distanceToPlayer <= attackRange)
        {
            playerHealth.TakeDamage(damage);
            // Dessine un gizmo pour indiquer que le joueur a été touché
            Debug.Log("Player hit");
            Debug.DrawLine(transform.position, playerTransform.position, Color.red, 1.0f);
        }
        isAttacking = false;
    }

    // Dessine un cercle au sol pour montrer la portée d'attaque
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void OnPlayerNoise()
    {
        //Debug.Log("Heard player noise");
        playerMadeNoise = true;
    }
    public void AggroPlayer()
    {
        playerMadeNoise = true;
        lostSightRange = 100.0f;
    }


    // Coroutine pour le déplacement aléatoire
    //IEnumerator Wander()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(wanderInterval);
    //        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
    //        randomDirection += transform.position;
    //        NavMeshHit hit;
    //        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
    //        Vector3 finalPosition = hit.position;
    //        agent.SetDestination(finalPosition);
    //    }
    //}
}
