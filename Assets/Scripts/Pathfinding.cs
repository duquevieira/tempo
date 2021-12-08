using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.UI;


public class Pathfinding : MonoBehaviour
{
    // Start is called before the first frame update
    const int FOOTCACHE = 6;

    [SerializeField] private GameObject grid;
    int vertices;
    private LinkedList<Edge>[] adjacencylist;
    private Dictionary<GameObject,int> nodes;
    private GameObject[] nodesarray;
    [SerializeField] private float verticalModifier;
    [SerializeField] private float horizontalModifier;
    [SerializeField] private NavMeshAgent playerNavMeshAgent;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private ThirdPersonCharacter character;
    [SerializeField] private SphereCollider nodeInclusionSphere;
    [SerializeField] private Image tapImage;
    [SerializeField] private Image walkingImage;
    private int[] path;
    public LinkedList<GameObject> pathFound;
    private int tapCount;
    [SerializeField] private GameObject footStep;
    [SerializeField] private GameObject clickFX;
    private GameObject clickInstance;
    private int footCount;
    private GameObject[] footSteps;
    [SerializeField] TimelineControl[] timelineControllers;

    public void PrintAdjacency(LinkedList<Edge>[] adjacency)
    {
        foreach (LinkedList<Edge> node in adjacency)
        {
            String list = "node: ";
            foreach (Edge e in node)
            {
                list = list + " " + e.destination;
            }
            Debug.Log(list);
        }
    }

    public void PrintArray(int[] array)
    {
        String list = "path: ";
        foreach (int i in array)
        {
            list = list + " " + i;
        }
        Debug.Log(list);
    }
    public void PrintQueue(PriorityQueuePair q)
    {
        String list = "queue: ("+ q.Length()+")";
        Pair[] array = q.GiveArray();
        for(int i = 0; i<=q.Length();i++)
        {
            list = list + "/ " + array[i].index+" "+ array[i].value;
        }
        Debug.Log(list);

    }

    public class Edge
    {
        public int source;
        public int destination;
        public float weight;

        public Edge(int source, int destination, float weight)
        {
            this.source = source;
            this.destination = destination;
            this.weight = weight;
        }

    }

    public class Pair
    {
        public float value;
        public int index;

        public Pair(float value, int index)
        {
            this.value = value;
            this.index = index;
        }
        public float CompareTo(Pair o)
        {
            return value - o.value;
        }

        public bool Equal(Pair o)
        {
            return index.Equals(o.index);
        }
    }

    public class PriorityQueuePair
    {
        static int length;
        static Pair[] h;

        public PriorityQueuePair(int size)
        {
            length = -1;
            h = new Pair[size];
            h[0] = new Pair(int.MinValue,int.MinValue);
        }

        public Pair[] GiveArray()
        {
            return h;
        }

        public int Length()
        {
            return length;
        }

        static int Parent(int i)
        {
            return (i - 1) / 2;
        }

        static int LeftChild(int i)
        {
            return ((2 * i) + 1);
        }

        static int RightChild(int i)
        {
            return ((2 * i) + 2);
        }

        static void ShiftUp(int i)
        {
            while (i > 0 && h[Parent(i)].CompareTo(h[i]) < 0)
            {

                // Swap parent and current node
                Swap(Parent(i), i);

                // Update i to parent of i
                i = Parent(i);
            }
        }

        static void ShiftDown(int i)
        {
            int maxIndex = i;

            // Left Child
            int l = LeftChild(i);

            if (l <= length &&
                h[l].CompareTo(h[maxIndex]) >0)
            {
                maxIndex = l;
            }

            // Right Child
            int r = RightChild(i);

            if (r <= length &&
                h[r].CompareTo(h[maxIndex]) >0)
            {
                maxIndex = r;
            }

            // If i not same as maxIndex
            if (i != maxIndex)
            {
                Swap(i, maxIndex);
                ShiftDown(maxIndex);
            }
        }

        internal void Add(Pair pair)
        {
            length = length + 1;
            h[length] = pair;

            // Shift Up to maintain
            // heap property
            ShiftUp(length);
        }

        
        internal Pair Poll()
        {
            Pair result = h[0];

            // Replace the value
            // at the root with
            // the last leaf
            h[0] = h[length];
            length = length - 1;

            // Shift down the replaced
            // element to maintain the
            // heap property
            ShiftDown(0);
            return result;
        }

        static void ChangePriority(int i, Pair p)
        {
            Pair oldp = h[i];
            h[i] = p;

            if (p.CompareTo(oldp) > 0)
            {
                ShiftUp(i);
            }
            else
            {
                ShiftDown(i);
            }
        }

        static Pair GetMax()
        {
            return h[0];
        }

        internal void Remove(Pair i)
        {
            Pair removed=null;
            int index = 0;
            int count=0;
            foreach (Pair p in h)
            {
                if (p.Equal(i))
                {
                    removed = p;
                    index = count;
                }
                count++; ;
            }
            if (removed != null)
            {
                removed = new Pair(GetMax().value - 1, removed.index);
                ShiftUp(index);
                Poll();
            }
            
        }

        internal bool IsEmpty()
        {
            return length < 0;
        }

        static void Swap(int i, int j)
        {
            Pair temp = h[i];
            h[i] = h[j];
            h[j] = temp;
        }
    }

    public void AddEdge(int source, int destination, float weight)
    {
        Edge edge = new Edge(source, destination, weight);
        adjacencylist[source].AddFirst(edge); 
    }

    public float calculateWeight(GameObject source, GameObject destination)
    {
        float x1 = horizontalModifier*source.transform.position.x;
        float x2 = horizontalModifier * destination.transform.position.x;
        float y1 = verticalModifier * source.transform.position.y;
        float y2 = verticalModifier * destination.transform.position.y;
        float z1 = horizontalModifier * source.transform.position.z;
        float z2 = horizontalModifier * destination.transform.position.z;
        float weight = Mathf.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
        return weight;
    }

    

    void Start()
    {
        vertices = grid.transform.childCount;
        adjacencylist = new LinkedList<Edge>[vertices];
        nodes = new Dictionary<GameObject, int>();
        nodesarray = new GameObject[vertices];
        for (int i = 0; i < vertices; i++)
        {
            adjacencylist[i] = new LinkedList<Edge>();
            GameObject child = grid.transform.GetChild(i).gameObject;
            nodes.Add(child,i);
            nodesarray[i] = child;
        }
        for (int i = 0; i < vertices; i++)
        {
            GameObject node = nodesarray[i];
            GameObject[] connect = node.GetComponent<NodeConnections>().Connections;
            for (int j = 0; j<connect.Length; j++)
            {
                Debug.Log(i+"-"+j);
                AddEdge(i, nodes[connect[j]], calculateWeight(node, connect[j]));
                
            }
        }
        pathFound = new LinkedList<GameObject>();
        PrintAdjacency(adjacencylist);
        playerNavMeshAgent.updateRotation = false;
        SetWalkingImage(false);
        SetTapImage(false);
        footSteps = new GameObject[FOOTCACHE];
        footCount = 0;
        foreach (TimelineControl controller in timelineControllers)
        {
            controller.Pause();
        }
    }

    private void SetWalkingImage(bool v)
    {
        var walkingColor = walkingImage.color;
        walkingColor.a = v ? 1f : .3f;
        walkingImage.color = walkingColor;
    }

    private void SetTapImage(bool v)
    {
        var tapColor = tapImage.color;
        tapColor.a = v ? 1f : .3f;
        tapImage.color = tapColor;
    }

    private void FixedUpdate()
    {
        if (tapCount<10)
            tapCount++;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Click"))
        {
            SetTapImage(true);
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (clickInstance != null)
                    Destroy(clickInstance);
                clickInstance = Instantiate(clickFX, hit.point, Quaternion.identity, this.gameObject.transform.parent);
                djikstra(hit);
                foreach(TimelineControl controller in timelineControllers)
                {
                    controller.Play();
                }
            }
            
        }
        if (Input.GetButtonDown("Time"))
        {
            foreach (TimelineControl controller in timelineControllers)
            {
                controller.setRewind(!controller.isRewinding);
            }
        }
        if (tapCount == 10)
        {
            SetTapImage(false);
            tapCount = 0;
        }
           
        if (playerNavMeshAgent.remainingDistance > playerNavMeshAgent.stoppingDistance)
        {

            character.Move(playerNavMeshAgent.desiredVelocity, false, false);
            SetWalkingImage(true);
        }
        else
        {
            if (pathFound.Count > 0)
            {
                playerNavMeshAgent.SetDestination(pathFound.Last.Value.transform.position);
                Debug.Log(pathFound.Last.Value.name);
                pathFound.RemoveLast();
                if (footCount == footSteps.Length)
                    footCount = 0;
                if (footSteps[footCount] != null)
                    Destroy(footSteps[footCount]);
                footSteps[footCount++] = Instantiate(footStep,this.gameObject.transform.position, this.gameObject.transform.rotation,this.gameObject.transform.parent);
                SetWalkingImage(true);

            }
            else
            {
                character.Move(Vector3.zero, false, false);
                SetWalkingImage(false);
                foreach (TimelineControl controller in timelineControllers)
                {
                    controller.Pause();
                }
            }
           
        }
    }

    public void djikstra(RaycastHit hit)
    {
        int dest = 0;

        /*Collider[] objects = Physics.OverlapSphere(nodeInclusionSphere.transform.position, nodeInclusionSphere.radius, 10);

        int origin = nodes[objects[0].gameObject];*/
        int origin = 0;

        float maxDistance = float.MaxValue;
        float maxDistanceDest = float.MaxValue;
        foreach (GameObject collider in nodes.Keys)
        {
            //Debug.Log("run : "+collider.name);
            GameObject source = nodeInclusionSphere.gameObject;
            GameObject destination = collider;
            float x1 = source.transform.position.x;
            float x2 = destination.transform.position.x;
            float y1 = source.transform.position.y;
            float y2 = destination.transform.position.y;
            float z1 = source.transform.position.z;
            float z2 = destination.transform.position.z;
            float distance = Mathf.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
            if (distance <= maxDistance)
            {
                maxDistance = distance;
                origin = nodes[collider.gameObject];
            }
            x1 = hit.point.x;
            //Debug.Log("x - "+ x1);
            y1 = hit.point.y;
            //Debug.Log("y - " + y1);
            z1 = hit.point.z;
            //Debug.Log("z - " + z1);
            distance = Mathf.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
            if (distance <= maxDistanceDest)
            {
                maxDistanceDest = distance;
                dest = nodes[collider.gameObject];
            }
        }

        pathFound.Clear();

        bool[] selected = new bool[vertices];
        float[] length = new float[vertices];
        path = new int[vertices];
        //Debug.Log("origin : " + origin + " / destination : " + dest);
        PriorityQueuePair connected = new PriorityQueuePair(vertices);
        for (int v = 0; v < vertices; v++)
        {
            selected[v] = false;
            length[v] = float.MaxValue;
            path[v] = int.MinValue;
        }
        length[origin] = 0;
        connected.Add(new Pair(0, origin));
        do
        {
            int node = connected.Poll().index;
            //Debug.Log("finding : "+node);
            selected[node] = true;
            foreach (Edge e in adjacencylist[node])
            {
                int secnode = e.destination;
                if(nodesarray[secnode].transform.GetChild(0).gameObject.activeSelf)
                    if (!selected[secnode])
                    {
                        float newLength = length[node] + e.weight;
                        if (newLength < length[secnode])
                        {
                            bool nodeIsInQueue = length[secnode] < float.MaxValue;
                            length[secnode] = newLength;
                            path[secnode] = node;
                            //Debug.Log(node+ " leads to " +secnode);
                            //Debug.Log(nodeIsInQueue);

                            if (nodeIsInQueue)
                            {
                                Pair p = new Pair(newLength, secnode);
                                connected.Remove(p);
                                connected.Add(p);
                            }
                            else
                            {
                                connected.Add(new Pair(newLength, secnode));
                            }
                            //PrintQueue(connected);
                        }
                    }
            }

        } while (!connected.IsEmpty());
        int current = dest;
        //PrintArray(path);
        do
        {
            
            Debug.Log("found : " + current);
            if (current < 0)
            {
                pathFound.Clear();
                return;
            }
            pathFound.AddLast(nodesarray[current]);

            current = path[current];
            
        } while (current != origin);
    }
}
