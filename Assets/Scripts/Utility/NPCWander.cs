using UnityEngine;
using UnityEngine.AI;

public class NPCWander : MonoBehaviour
{
    public float wanderRadius = 5f;
    public float wanderDelay = 3f;

    private NavMeshAgent agent;
    private Animator animator;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        timer = wanderDelay;
    }

    void Update()
    {
        timer += Time.deltaTime;

        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (timer >= wanderDelay)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDir = Random.insideUnitSphere * dist;
        randDir += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDir, out navHit, dist, NavMesh.AllAreas);

        return navHit.position;
    }
}
