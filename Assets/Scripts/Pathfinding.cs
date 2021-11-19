using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject grid;
    int vertices;
    private LinkedList<Edge>[] adjacencylist;
    private Dictionary<GameObject,int> nodes;
    private GameObject[] nodesarray;
    [SerializeField] private float verticalModifier;
    [SerializeField] private float horizontalModifier;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private SphereCollider nodeInclusionSphere;
    private int[] path;
    public LinkedList<GameObject> pathFound;

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
        public int value;
        public int index;

        public Pair(int value, int index)
        {
            this.value = value;
            this.index = index;
        }
        public int CompareTo(Pair o)
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
            length = 0;
            h = new Pair[size];
            h[0] = new Pair(int.MinValue,int.MinValue);
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
            return ((2 * i) + 1);
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
            return length <= 0;
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
        float y1 = horizontalModifier * source.transform.position.y;
        float y2 = horizontalModifier * destination.transform.position.y;
        float z1 = verticalModifier*source.transform.position.z;
        float z2 = verticalModifier*destination.transform.position.z;
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
                AddEdge(i, nodes[connect[j]], calculateWeight(node, connect[j]));
            }
        }
        pathFound = new LinkedList<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Click"))
        {
            
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (nodes.ContainsKey(hit.transform.gameObject))
                {

                    int dest = nodes[hit.transform.gameObject];

                    /*Collider[] objects = Physics.OverlapSphere(nodeInclusionSphere.transform.position, nodeInclusionSphere.radius, 10);
                    
                    int origin = nodes[objects[0].gameObject];*/
                    int origin = 0;
                    float maxDistance = float.MaxValue;
                    foreach (GameObject collider in nodes.Keys)
                    {
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
                    }

                    pathFound.Clear();

                    bool[] selected = new bool[vertices];
                    int[] length = new int[vertices];
                    path = new int[vertices];
                    PriorityQueuePair connected = new PriorityQueuePair(vertices);
                    for (int v = 0; v < vertices; v++)
                    {
                        selected[v] = false;
                        length[v] = int.MaxValue;
                        path[v] = int.MinValue;
                    }
                    length[origin] = 0;
                    connected.Add(new Pair(0, origin));
                    do
                    {
                        int node = connected.Poll().index;
                        Console.WriteLine(node);
                        selected[node] = true;
                        foreach (Edge e in adjacencylist[node])
                        {
                            int secnode = e.destination;
                            if (!selected[secnode])
                            {
                                int newLength = length[node] + Mathf.RoundToInt(e.weight);
                                if (newLength < length[secnode])
                                {
                                    bool nodeIsInQueue = length[secnode] < int.MaxValue;
                                    length[secnode] = newLength;
                                    path[node] = secnode;
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
                                }
                            }
                        }

                    } while (!connected.IsEmpty());
                    int current = origin;
                    do
                    {
                        pathFound.AddLast(nodesarray[current]);
                        Console.WriteLine(current);
                        current = path[current];
                    } while (current != dest);
                    pathFound.AddLast(nodesarray[dest]);
                    Console.WriteLine(current);
                }
            }
            
        }
    }
}
