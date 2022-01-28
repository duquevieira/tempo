using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Mode { Loop=1, Reverse=2 }
    const int FOOTCACHE = 6;
    [SerializeField] private Mode mode;
    [SerializeField] private GameObject[] path;
    private int currentNode;
    private int factor;
    [SerializeField] private NavMeshAgent playerNavMeshAgent;
    [SerializeField] private GameObject footStep;
    private int footCount;
    private GameObject[] footSteps;
    void Start()
    {
        factor = -1;
        currentNode = 0;
        footCount = 0;
        footSteps = new GameObject[FOOTCACHE];
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNavMeshAgent.remainingDistance<=playerNavMeshAgent.stoppingDistance)
        {
            
            if (mode == Mode.Loop)
            {
                if (currentNode == path.Length)
                {
                    currentNode = 0;
                }
                playerNavMeshAgent.SetDestination(path[currentNode++].transform.position);
            }
            else if (mode == Mode.Reverse)
            {
                if (currentNode == path.Length - 1||currentNode==0)
                {
                    factor = -factor;
                }
                playerNavMeshAgent.SetDestination(path[currentNode].transform.position);
                currentNode = factor + currentNode;
            }
            if (footCount == footSteps.Length)
                footCount = 0;
            if (footSteps[footCount] != null)
                Destroy(footSteps[footCount]);
            footSteps[footCount++] = Instantiate(footStep, this.gameObject.transform.position, this.gameObject.transform.rotation, this.gameObject.transform.parent);

        }

    }
}
