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

    public float[] lowestTs = new float[100];

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

            // Exercise 3 & 4
            if (GridGenerator._instance)
            {
                Vector3 steeringForce = ComputeSteeringForces(agents[i], goal3d);

                // Apply steering forces
                Vector3 force = Truncate(steeringForce, maxForce); // limit the force to apply
                Vector3 acceleration = force / agents[i].GetComponent<Rigidbody>().mass; // update acceleration with Newton’s 2nd law
                agents[i].velocity += acceleration * Time.deltaTime; // update velocity
                agents[i].velocity = Truncate(agents[i].velocity, maxSpeed); // limit agent speed
            }
            else    // Exercise 2
            {
                agents[i].velocity = (goal3d - agents[i].transform.position).normalized * velocity;
            }
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
        Vector3 steeringForces = Vector3.zero;

        if (gridGenerator == null) return steeringForces;

        if (gridGenerator.doSeek) steeringForces += Seek(a, target) * gridGenerator.seekWeight;
        else a.velocity = (target - a.transform.position).normalized * velocity;

        if (gridGenerator.doAvoid) steeringForces += Avoid(a, target) * gridGenerator.avoidWeight;
        if (gridGenerator.doDynamicAvoid) steeringForces += UnalignedCollisionAvoidance(a, target) * gridGenerator.dynamicAvoidWeight;


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

        // Check potential collisions with obstacles (walls)
        List<GridCell> gridNodes = GridGenerator._instance.grid.nodes;
        float wallRadius = GridGenerator._instance.grid.cellSize.magnitude / 1.7f;  // slightly greater (1.7 instead of 2) to have a lower maximum error
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
        Vector2 agentPos = XZ(a.transform.position);
        Vector2 agentVelocity = XZ(a.velocity);
        float distanceToGoal = Vector2.Distance(a.pathManager.goal, agentPos);

        float lowestT = 9999f;
        float lowestU = 9999f;
        Agent closestAgent = null;
        bool colinear = false;

        // Check potential collisions with other agents
        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i] == a) continue;

            // Get intersection parameters for 2D line-line intersection 
            float t = getIntersectionInstant(agentPos, agentVelocity, XZ(agents[i].transform.position), XZ(agents[i].velocity));
            float u = getIntersectionInstant(XZ(agents[i].transform.position), XZ(agents[i].velocity), agentPos, agentVelocity);
            
            // we want to correct future intersections but not too far
            if (t > 0f && u > -0.5f && t < 1f && u < 1f)
            {
                // Intersection parameters for both agents should be close
                if (Mathf.Abs(u - t) < 1f && t < lowestT)
                {
                    // update closest agent and distance
                    lowestT = t;
                    lowestU = u;
                    closestAgent = agents[i];
                }
            }

            // velocity vectors might be colinear and opposite. behavior should be different in this special case
            if (Vector2.Dot(agentVelocity.normalized, XZ(agents[i].velocity.normalized)) < -0.8 && Vector2.Distance(agentPos, XZ(agents[i].transform.position)) < 1.5f*distanceToGoal
                && Vector2.Dot((XZ(agents[i].transform.position) - agentPos).normalized, a.transform.forward) > 0.8)
            {
                closestAgent = agents[i];
                colinear = true;
                break;
            }
        }

        // intersection for both paths happen at the same time
        if (closestAgent != null)
        {
            lowestTs[a.id] = lowestT;
            if (colinear)
            {
                // if directions are opposite, push agent towards the corresponding side
                return 150f * a.transform.right * (Vector2.Dot((XZ(closestAgent.transform.position) - agentPos).normalized, a.transform.right) > 0 ? -1f : 1f) - 50f*a.transform.forward;
            }

            // steer laterally to turn away from the potential collision. It will also accelerate forward or decelerate backwards
            // (quote from http://www.red3d.com/cwr/steer/gdc99/) 
            Vector3 avoidanceForce = Vector3.zero;
            Vector2 agentFuturePos = agentPos + lowestT * agentVelocity;
            Vector2 obstacleFuturePos = XZ(closestAgent.transform.position) + lowestT * (XZ(closestAgent.velocity));
            Vector2 agentObstacleDiff = obstacleFuturePos - agentFuturePos;
            
            // if at the same future instant, the closest agent is more to the right, push current one to the left 
            if (Vector2.Dot(agentObstacleDiff, XZ(a.transform.right)) > 0)
            {
                avoidanceForce -= 3f * a.transform.right;
            }
            else    // and viceversa
            {
                avoidanceForce += 3f * a.transform.right;
            }
            
            
            // if the same position was reached before by the closest agent, decelerate the current one 
            if (lowestT > lowestU)
            {
                avoidanceForce -= 1f * a.transform.forward;
            }
            else    // and viceversa
            {
                avoidanceForce += 1f * a.transform.forward;
            }

            return avoidanceForce;
        }

        return Vector3.zero;
    }

    Vector2 XZ(Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.z);
    }

    float getIntersectionInstant(Vector2 p1, Vector2 v1, Vector2 p2, Vector2 v2)
    {
        // line-line intersection (https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect)
        return Vec2Cross(p2 - p1, v2) / Vec2Cross(v1, v2);
    }

    float Vec2Cross(Vector2 v, Vector2 w)
    {
        return v.x * w.y - v.y * w.x;
    }

    void OnDrawGizmos()
    {
        // draw lines to intersection points
        for (int i = 0; i < agents.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(agents[i].transform.position, agents[i].transform.position + agents[i].velocity * lowestTs[i]);
            lowestTs[i] = 0f;
        }
    }

}
