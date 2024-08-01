using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour
{
    public GameObject[] goalLocations;

    NavMeshAgent agent;

    Animator anim;

    float speedMult;

    float detectionRadius = 5;

    float fleeRadius = 10;

    float attractionRadius = 3;

    float maximumAttractionRange = 20;
    void ResetAgent()
    {
        speedMult = Random.Range(0.1f, 1.5f);
        agent.speed = 2 * speedMult;
        agent.angularSpeed = 120;
        anim.SetFloat("speedMult", speedMult);
        anim.SetTrigger("isWalking");
        agent.ResetPath();
    }
    // Start is called before the first frame update
    void Start()
    {
        goalLocations = GameObject.FindGameObjectsWithTag("goal");
        agent = this.GetComponent<NavMeshAgent>(); 
        agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);
        
        anim = this.GetComponent<Animator>();
        anim.SetFloat("wOffset", Random.Range(0.1f, 1f));  
        ResetAgent();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 1)
        {
            ResetAgent();
            agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);
        }
    }

    public void AttractToLocation(Vector3 location)
    {
        float distanceToLocation = Vector3.Distance(location, this.transform.position);
        if (distanceToLocation <= attractionRadius)
        {
            agent.SetDestination(location);
            anim.SetTrigger(distanceToLocation < detectionRadius ? "isWalking" : "isRunning");
            agent.speed = distanceToLocation < detectionRadius ? 2 : 5; // Adjust speed based on distance --> less than detection = 2 speed --> Equal or greater than 5 speed
            agent.angularSpeed = 360;
        }
        else if (distanceToLocation <= maximumAttractionRange) // maximum range for attraction
        {
            // Lessen the attraction effect as the agent is further away
            agent.SetDestination(location);
            anim.SetTrigger("isRunning");
            agent.speed = 8; 
            agent.angularSpeed = 400; 
        }
        
    }

    public void DetectNewObstacle(Vector3 location)
    {
        if (Vector3.Distance(location, this.transform.position) < detectionRadius)
        {
            Vector3 fleeDirection = (this.transform.position - location).normalized;
            Vector3 newGoal = this.transform.position + fleeDirection * fleeRadius;

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newGoal, path);

            if (path.status != NavMeshPathStatus.PathInvalid)
            {
                agent.SetDestination(path.corners[path.corners.Length - 1]);
                anim.SetTrigger("isRunning");
                agent.speed = 10;
                agent.angularSpeed = 500;
            }
        }
    }
}
