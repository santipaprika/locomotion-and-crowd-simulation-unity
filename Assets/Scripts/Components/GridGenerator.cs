﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public Agent agent;

    // USE LOW NUMBER OF AGENTS IF YOU WANT TO DEBUG WITH GIZMOS (1 or 2, otherwise it will be a bit messy)
    public int numberOfAgents = 20;
    public float agentsVelocity = 2f;

    public Vector2Int cellsPerDim = new Vector2Int(10, 10);
    public Vector2 gridExtents = new Vector2(10, 10);
    public float obstacleRate = 0.2f;
    public bool doSeek = true;
    public float seekWeight = 1.5f;
    public bool doAvoid = true;
    public float avoidWeight = 1f;
    public bool doDynamicAvoid = true;
    public float dynamicAvoidWeight = 1f;

    public bool debugGrid = true;
    public bool debugNodeDensity = true;
    public bool debugOpenNodes = true;
    public bool debugPath = true;
    public bool debugAllAgents = false;
    public GridGraph grid;

    // singleton
    public static GridGenerator _instance = null;

    // Start is called before the first frame update
    void Awake()
    {
        grid = new GridGraph(cellsPerDim, new Vector2(gridExtents.x, gridExtents.y), obstacleRate);

        // Ensure all agents will have a room to be placed
        if (numberOfAgents >= grid.nodes.Count)
        {
            Debug.LogError("Number of Agents must be smaller than total number of nodes!");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // get simulator agents list as reference
        Simulator simulator = Simulator.GetInstance();
        simulator.velocity = agentsVelocity;

        ref List<Agent> agents = ref simulator.agents;

        // instantiate agents
        GameObject agentsGO = new GameObject("Agents");
        List<int> occupiedIndices = new List<int>();
        for (int i = 0; i < numberOfAgents; i++)
        {
            int idx = Random.Range(0, grid.availableNodes.Count - 1);

            // avoid instantiating two agents in the same node
            while (occupiedIndices.Contains(idx))
            {
                idx = Random.Range(0, grid.availableNodes.Count - 1);

            }

            // instantiate agents
            GridCell spawnNode = grid.availableNodes[idx];
            Agent agentInstance = Instantiate(agent, spawnNode.getCenter(), Quaternion.identity, agentsGO.transform);
            agents.Add(agentInstance);
        }

        // assign singleton instance
        _instance = this;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < grid.nodes.Count; i++)
        {
            GridCell cell = grid.nodes[i];
            for (int j = 0; j < grid.connections[i].connections.Count; j++)
            {
                if (debugGrid) Gizmos.DrawLine(cell.getCenter(), grid.connections[i].connections[j].toNode.getCenter());
                if (debugNodeDensity && cell.density > 0) Gizmos.DrawSphere(cell.getCenter(), cell.density * 0.2f);
            }
        }
    }
}
