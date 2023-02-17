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
        WaitChase,
        Waiting,
        Attacking
    }

    public State currentState;

    NavMeshAgent agent;

    public Transform[] destinationPoints;
    int destinationIndex = 0;
    public Transform player;
    [SerializeField] float visionRange;
    [SerializeField] [Range(0, 369)]
    float visionAngle;

    [SerializeField] float hitRange;

    [SerializeField] LayerMask obstacleMask;

    [SerializeField] float patrolRange = 10f;
    private Transform patrolZone;

    [SerializeField] float startWaitingTime;
    private float waitingTime;
    
    [SerializeField] float startWaitChaseTime;
    private float waitChaseTime;



    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Patrolling;
        
        destinationIndex = Random.Range(0, destinationPoints.Length);

        waitingTime = startWaitingTime;

        waitChaseTime = startWaitChaseTime; 
        
    }

    // Update is called once per frame
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

            case State.WaitChase:
                WaitChase();
            break;
            default:
                Chase();
            break;
            case State.Waiting:
                Waiting();
            break;
            case State.Attacking:
                Attacking();
            break;
        }
    }



    /*void Patrol() 
    {
        agent.destination = destinationPoints[destinationIndex].position;
        if(Vector3.Distance(transform.position, destinationPoints[destinationIndex].position) < 1)
        {
        destinationIndex = Random.Range(0, destinationPoints.Length);
        }

        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            currentState = State.Chasing;
        }
    }*/

    void Patrol() 
    {
        Vector3 randomPosition;
        if(RandomPoint(patrolZone.position, patrolRange, out randomPosition))
        {
            agent.destination = randomPosition;
            Debug.DrawRay(randomPosition, Vector3.up * 5, Color.blue, 5f);
        }

        if(FindTarget())
        {
            currentState = State.Chasing;
        }

    }


    bool RandomPoint(Vector3 center, float range, out Vector3 point) 
    {
        Vector3 RandomPoint = center + Random.insideUnitSphere * range; 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(RandomPoint, out hit, 4, NavMesh.AllAreas))
        {
            point = hit.position;
            return true;
        }
        point = Vector3.zero;
        return false;
    }


    void WaitChase()
    {
        agent.destination = transform.position;
        if(waitChaseTime <= 0)
        {
            waitChaseTime = startWaitChaseTime;
            currentState = State.Patrolling;
        }
        else
        {
            waitChaseTime -= Time.deltaTime;
        }
        if(Vector3.Distance(transform.position, player.position) < visionRange)
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

    void Attacking()
    {
        agent.destination = player.position;
        Debug.Log("Attack");

                if(Vector3.Distance(transform.position, player.position) > hitRange)
        {
            currentState = State.Chasing;
        }
        
    }

    void Waiting()
    {
                agent.destination = transform.position;
        if(waitChaseTime <= 0)
        {
            waitChaseTime = startWaitChaseTime;
            currentState = State.Patrolling;
        }
        else
        {
            waitChaseTime -= Time.deltaTime;
        }
        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            currentState = State.Chasing;
        }
    }

    bool FindTarget()
    {
        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            Vector3 directionToTarget  = (player.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, directionToTarget) < visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, player.position);
                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void OnDrawGizmos() 
    {
        foreach(Transform point in destinationPoints)
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
