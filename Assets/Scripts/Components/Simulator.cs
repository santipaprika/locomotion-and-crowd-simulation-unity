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

    Vector3 ComputeSteeringForces(Agent a, Vector3 target)
    {
        GridGenerator gridGenerator = GridGenerator._instance;
        Vector3 steeringForces = Seek(a, target) * gridGenerator.seekWeight;
        // Vector3 steeringForces = Seek(a, target);
        return steeringForces;
    }

    Vector3 Seek(Agent a, Vector3 target)
    {
        Vector3 desiredVelocity = (target - a.transform.position).normalized * velocity;
        return desiredVelocity - a.velocity;
    }

    Vector3 Truncate(Vector3 v, float max)
    {
        float size = Mathf.Min(v.magnitude, max);
        return v.normalized * size;
    }

}
