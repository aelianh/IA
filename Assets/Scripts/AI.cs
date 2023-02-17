using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public enum State
    {
        Patrolling,
        Chasing,
        Traveling, 
        Waiting,
        Attacking,
    }

    public State currentState;

    NavMeshAgent agent;

    public Transform[] patrolPoints;
    public Transform player;
    [SerializeField] float visionRange;
    [SerializeField] float patrolRange = 10f; 
    [SerializeField] Transform patrolZone;
    [SerializeField] float visionAngle;
    [SerializeField] [Range(0, 360)]
    public LayerMask obstaclesMask;
    public Transform[] points;
    private int destinationPoint = 0;
    public float timer = 5.0f;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    void Start()
    {
        currentState = State.Patrolling;
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        GotoNextPoint();
    }


    void Update()
    {
        switch(currentState)
        {
            case State.Patrolling:
                Patrol();
            break;

            case State.Chasing:
                Chase();
            break;

            case State.Traveling:
                    Travel();
            break;

            case State.Waiting:
                    Wait();
            break;

            case State.Attacking:
                    Attack();
            break; 

            default:
                Chase();
            break;
        }

    }

    void GotoNextPoint() 
    {
        if (points.Length == 0)
        return;
        agent.destination = points[destinationPoint].position;
        destinationPoint = (destinationPoint + 1) % points.Length;
    }

    void Patrol() 
    {
        if (agent.remainingDistance < 0.5f)
        GotoNextPoint();

        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            currentState = State.Chasing;
        }

        currentState = State.Traveling;
    }

    void Patrol2() 
    {
        Vector3 destinationPoint;
        if(Destination(patrolZone.position, patrolRange, out destinationPoint))
        {
            agent.destination = destinationPoint; 
            Debug.DrawRay(destinationPoint, Vector3.up * 5, Color.blue, 5f);
        }

        if(FindTarget())
        {
            currentState = State.Chasing;
        }

        currentState = State.Traveling; 
    }

    bool Destination(Vector3 center, float range, out Vector3 point)
    {
        Vector3 Destination = center * destinationPoint * range;
        NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(Destination, out hit, 4, NavMesh.AllAreas))
        {
            point = hit.position;
            return true; 
        }

        point = Vector3.zero;
        return false; 
    }
    

    void Travel()
    {
        if(agent.remainingDistance <= 0.2)
        {
            currentState = State.Waiting;
        }

        if(FindTarget())
        {
            currentState = State.Chasing;
        }
    }

    void Chase()
    {
        agent.destination = player.position;

        if(!FindTarget())
        {
            currentState = State.Patrolling;
        }
    }

    void Wait()
    {
        timer -= Time.deltaTime;

        if (!FindTarget())
        {
            if(timer <= 0)
            {
                currentState = State.Patrolling;
                timer = 5;
            }  
        }
    }

    void Attack()
    {
        if(FindTarget())
        {
            currentState = State.Attacking;
            Debug.Log("Attack");
        }
    }

    bool FindTarget()
    {
        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            Vector3 directionToTarget = (player.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, directionToTarget) < visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, player.position);
                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstaclesMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void OnDrawGizmos() 
    {
        foreach(Transform point in patrolPoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(point.position, 1);
        }


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(patrolZone.position, patrolRange);
        
    }
}
