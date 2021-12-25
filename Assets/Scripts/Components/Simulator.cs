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

    public float velocity = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

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
            agents[i].velocity = velocity * (goal3d - agents[i].transform.position).normalized;
        }
    }

}
