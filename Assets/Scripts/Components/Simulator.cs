using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    // singleton
    public static Simulator _instance = null;
    public static Simulator GetInstance()
    {
        if (_instance == null)
        {
            GameObject _simulatorGameObject = new GameObject("Simulator");
            _instance = _simulatorGameObject.AddComponent<Simulator>();
        }
        return _instance;
    }

    [HideInInspector]
    public List<Agent> agents = new List<Agent>();

    public float velocity = 2f;
    public float maxForce = 10f;
    private float maxSpeed = 4.73f;


    // Update is called once per frame
    void Update()
    {
        UpdateSimulation(Time.deltaTime);
    }

    void UpdateSimulation(float timestep)
    {
        for (int i = 0; i < agents.Count; i++)
        {
            Vector3 goal3d = new Vector3(agents[i].pathManager.goal.x, agents[i].transform.position.y, agents[i].pathManager.goal.y);
            Vector3 steeringForce = ComputeSteeringForces(agents[i], goal3d);

            // Apply steering forces
            Vector3 force = Truncate(steeringForce, maxForce); // limit the force to apply
            Vector3 acceleration = force / agents[i].GetComponent<Rigidbody>().mass; // update acceleration with Newton’s 2nd law
            agents[i].velocity += acceleration * Time.deltaTime; // update velocity
            agents[i].velocity = Truncate(agents[i].velocity, maxSpeed); // limit agent speed
        }
    }
    Vector3 Truncate(Vector3 v, float max)
    {
        float size = Mathf.Min(v.magnitude, max);
        return v.normalized * size;
    }

    Vector3 ComputeSteeringForces(Agent a, Vector3 target)
    {
        GridGenerator gridGenerator = GridGenerator._instance;
        Vector3 steeringForces = new Vector3(0, 0, 0);

        if (gridGenerator.doSeek) steeringForces += Seek(a, target) * gridGenerator.seekWeight;
        else a.velocity = (target - a.transform.position).normalized * velocity;

        if (gridGenerator.doAvoid) steeringForces += Avoid(a, target) * gridGenerator.avoidWeight;


        return steeringForces;
    }

    Vector3 Seek(Agent a, Vector3 target)
    {
        Vector3 desiredVelocity = (target - a.transform.position).normalized * velocity;
        return desiredVelocity - a.velocity;
    }

    Vector3 Avoid(Agent a, Vector3 target)
    {
        float radius = a.GetComponent<Collider>().bounds.extents.x;
        Vector3 agentPos = a.transform.position;
        float distanceToGoal = Vector2.Distance(a.pathManager.goal, new Vector2(agentPos.x, agentPos.z));

        float closestDistance = 9999f;
        float closestDistanceProjection = 0f;

        // Check potential collisions with other agents
        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i] == a) continue;

            // Notice ref closestDistance. It will be updated if necessary.
            GetProjectedDistanceIfCloser(a.transform, radius, agents[i].transform.position, 
                    agents[i].velocity, radius, distanceToGoal, ref closestDistance, ref closestDistanceProjection);
        }

        // Check potential collisions with walls
        List<GridCell> gridNodes = GridGenerator._instance.grid.nodes;
        float wallRadius = GridGenerator._instance.grid.cellSize.magnitude / 3f;  // slightly smaller (2.3 instead of 2) to have a lower maximum error
        bool closestObstacleIsWall = false; // if a closest wall is detected, the radius for walls will be used instead of the agents one
        for (int i = 0; i < gridNodes.Count; i++)
        {
            if (!gridNodes[i].isBlocked()) continue;

            // Notice ref closestDistance and closestDistanceProjection. These will be updated if necessary.
            closestObstacleIsWall = GetProjectedDistanceIfCloser(a.transform, radius, gridNodes[i].getCenter(), 
                        Vector3.zero, wallRadius, distanceToGoal, ref closestDistance, ref closestDistanceProjection);
        }

        if (closestDistance < distanceToGoal)
        {
            // opposite (-) projected distance to avoid obstacle
            // the smaller the distance, the greater the force to apply to ensure avoidance
            float sign = closestDistanceProjection > 0 ? -1f : 1f;
            float closestObstacleRadius = closestObstacleIsWall ? wallRadius : radius;
            return (distanceToGoal - closestDistance) * a.transform.right * sign;
        }

        return Vector3.zero;
    }

    bool GetProjectedDistanceIfCloser(Transform agentTransform, float agentRadius, Vector3 obstaclePos,
                                            Vector3 obstacleSpeed, float obstacleRadius, float distanceToGoal,
                                            ref float closestDistance, ref float closestDistanceProjection)
    {
        bool closerObjectFound = false;

        // guess future position of the other agent
        Vector3 obstaclePosition = obstaclePos + obstacleSpeed * Time.deltaTime;
        Vector3 obstacleAgentDiff = obstaclePosition - agentTransform.position;

        // check if projected distance is smaller that the radius of both imaginary cylinder and agent sphere (which are the same, then 2*R)
        // and discard agents that are in the back
        float projectedDistanceRight = Vector3.Dot(obstacleAgentDiff, agentTransform.right);
        float projectedDistanceForward = Vector3.Dot(obstacleAgentDiff, agentTransform.forward);
        if (Mathf.Abs(projectedDistanceRight) < (agentRadius + obstacleRadius) && projectedDistanceForward > 0)
        {
            // we want to avoid the agent only if the distance to it is closer than the distance to the current goal
            // and closer than the closes hit found yet
            float obstacleAgentDistance = obstacleAgentDiff.magnitude;
            if (obstacleAgentDistance < distanceToGoal && obstacleAgentDistance < closestDistance)
            {
                // update closest distance and its lateral (side-up) projection
                closestDistance = obstacleAgentDistance;
                closestDistanceProjection = projectedDistanceRight;
                closerObjectFound = true;
            }
        }

        return closerObjectFound;
    }

    Vector3 UnalignedCollisionAvoidance(Agent a, Vector3 target)
    {
        float radius = a.GetComponent<Collider>().bounds.extents.x;
        Vector3 agentPos = a.transform.position;
        float distanceToGoal = Vector2.Distance(a.pathManager.goal, new Vector2(agentPos.x, agentPos.z));

        float closestDistance = 9999f;
        float closestDistanceProjection = 0f;

        // Check potential collisions with other agents
        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i] == a) continue;

            // Notice ref closestDistance. It will be updated if necessary.
            GetProjectedDistanceIfCloser(a.transform, radius, agents[i].transform.position, 
                    agents[i].velocity, radius, distanceToGoal, ref closestDistance, ref closestDistanceProjection);
        }

        if (closestDistance < distanceToGoal)
        {
            // opposite (-) projected distance to avoid obstacle
            // the smaller the distance, the greater the force to apply to ensure avoidance
            float sign = closestDistanceProjection > 0 ? -1f : 1f;
            return (distanceToGoal - radius) * a.transform.right * sign;
        }

        return Vector3.zero;
    }

}
