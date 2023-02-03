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
        Travelling
    }

    public State currentState;

    NavMeshAgent agent;

    public Transform[] destinationPoints;
    int destinationIndex = 0;
    public Transform player;
    [SerializeField] float visionRange;
    [SerializeField]float patrolRange = 10f;
    [SerializeField] Transform patrolZone;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Patrolling;
        
        destinationIndex = Random.Range(0, destinationPoints.Length);
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
            case State.Travelling:
                Travel();
            break;
            default:
                Chase();
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


        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            currentState = State.Chasing;
        }

        currentState = State.Travelling;
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


    void Travel()

    {
        if(agent.remainingDistance <= 0.2f)
        {
            currentState = State.Patrolling;
        }

        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            currentState = State.Chasing;
        }
    }


    void Chase()
    {
        agent.destination = player.position;

        if(Vector3.Distance(transform.position, player.position) > visionRange)
        {
            currentState = State.Patrolling;
        }
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
