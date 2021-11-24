using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NodeConnections : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject[] Connections;
    private DynamicArray<bool> verify = new DynamicArray<bool>(4);
    [SerializeField] private GameObject myPreFab;
    [SerializeField] private bool auto;

    private void OnValidate()
    {
        if(myPreFab.GetComponent<NodeConnections>().auto)
        for (int i= 0; i<Connections.Length||i<verify.size; i++)
        {
            if (i >= verify.size)
                verify.Add(false);
            if (!verify[i]&& i < Connections.Length)
            {
                Connections[i] = Instantiate(myPreFab, this.transform.position, Quaternion.identity, this.transform.parent);
                Connections[i].GetComponent<NodeConnections>().myPreFab = myPreFab;
                Connections[i].GetComponent<NodeConnections>().Connections[0] = this.transform.gameObject;
                Connections[i].GetComponent<NodeConnections>().verify[0]=true;
                Connections[i].name = "Node(" + this.GetInstanceID() + ")" ;
                verify[i] = true;
            }
            if (i >= Connections.Length)
            {
                verify[i] = false;
            }
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
