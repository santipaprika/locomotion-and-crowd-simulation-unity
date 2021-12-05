using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdGenerator : MonoBehaviour {
    public Agent agent;
    public int numberOfAgents = 20;
    public Collider plane;

    private List<Agent> agents;

    // Start is called before the first frame update
    void Start() {
        Vector3 minBound = plane.bounds.min;
        Vector3 maxBound = plane.bounds.max;

        for (int i = 0; i < numberOfAgents; i++) {
            Agent agentInstance = Instantiate(agent, new Vector3(Random.Range(minBound.x, maxBound.x), 1f, Random.Range(minBound.z, maxBound.z)), Quaternion.identity);
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
