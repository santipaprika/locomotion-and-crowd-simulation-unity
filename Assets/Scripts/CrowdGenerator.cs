using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdGenerator : MonoBehaviour {
    public Agent agent;
    public int numberOfAgents = 20;
    public Collider plane;

    // Start is called before the first frame update
    void Start() {
        Vector3 minBound = plane.bounds.min;
        Vector3 maxBound = plane.bounds.max;

        Simulator simulator = Simulator.GetInstance();
        ref List<Agent> agents = ref simulator.agents;
        for (int i = 0; i < numberOfAgents; i++) {
            Agent agentInstance = Instantiate(agent, new Vector3(Random.Range(minBound.x, maxBound.x), agent.transform.position.y, Random.Range(minBound.z, maxBound.z)), Quaternion.identity);
            agents.Add(agentInstance);
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
