using UnityEngine;
using UnityEngine.AI; // Verplicht voor NavMesh!
using System.Collections.Generic;

public class NPCBrain : MonoBehaviour
{
    public enum AIMode { staticRoute, walkRandomly }
    [Header("Instellingen")]
    public AIMode mode = AIMode.walkRandomly;
    public float moveSpeed = 5f;
    public PlayerMovement _playerMovement; // To get the isActive state from the movement script

    [Header("For predefined route")]
    public List<Transform> routePoints; // List of empty game objects whose positions will be used as checkpoints
    private int currentPointIndex = 0;

    [Header("For random wandering")]
    public float randomRadius = 30f; // Walking distance between random points

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        if (mode == AIMode.staticRoute)
        {
            GoToNextPoint();
        }
        else
        {
            GoToRandomPoint();
        }
    }

    void Update()
    {
        // Don't do anything when the player is controlling this capsule
        if (_playerMovement.isActive)
        {
            agent.enabled = false;
            return;
        }
        else
        {
            agent.enabled = true;
        }

        // Check whether the agent has reached its destination
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            // No waiting time in between points
            if (mode == AIMode.staticRoute)
            {
                GoToNextPoint();
            }
            else
            {
                GoToRandomPoint();
            }
        }
    }

    // OPTIION 1: STATIC ROUTE
    void GoToNextPoint()
    {
        if (routePoints.Count == 0) return;

        // Set the destination to the current index
        agent.destination = routePoints[currentPointIndex].position;

        // Go to next point or restart
        currentPointIndex = (currentPointIndex + 1) % routePoints.Count;
    }

    // OPTIION 2: RANDOM WALKING
    void GoToRandomPoint()
    {
        // Choose random point within a sphere of radius randomRadius around the NPC's current position
        Vector3 randomDirection = Random.insideUnitSphere * randomRadius;
        randomDirection += transform.position;

        // Search for the closest valid point on the NavMesh floor
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, randomRadius, 1))
        {
            agent.destination = hit.position;
        }
    }
}