using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdGenerator : MonoBehaviour {
    public Agent agent;
    public int numberOfAgents = 20;
    public Collider plane;
    public float agentsVelocity = 2f;

    // Start is called before the first frame update
    void Start() {
        Vector3 minBound = plane.bounds.min;
        Vector3 maxBound = plane.bounds.max;

        // create simulator
        Simulator simulator = Simulator.GetInstance();
        simulator.velocity = agentsVelocity;

        // get simulator agents list as reference
        ref List<Agent> agents = ref simulator.agents;

        // instantiate agents
        for (int i = 0; i < numberOfAgents; i++) {
            Agent agentInstance = Instantiate(agent, new Vector3(Random.Range(minBound.x, maxBound.x), agent.transform.position.y, Random.Range(minBound.z, maxBound.z)), Quaternion.identity);
            agents.Add(agentInstance);

            // avoid instantiating two agents too close
            for (int j = 0; j < i; j++) {
                if (Vector3.Distance(agents[j].transform.position, agents[i].transform.position) < agent.gameObject.GetComponent<Collider>().bounds.size.x) {
                    agents.RemoveAt(i);
                    i--;
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
