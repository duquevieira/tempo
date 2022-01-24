using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeConnections : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject[] Connections;
    private int verify = 0;
    public GameObject myPreFab;
    [SerializeField] private bool auto;
    [SerializeField] private GameObject portal;
    [SerializeField] private GameObject forward;

    private void OnValidate()
    {
        if (myPreFab.GetComponent<NodeConnections>().auto)
            IntegrityCheck();
        
    }

    private void IntegrityCheck()
    {
        if (Connections.Length > verify)
        {
            for (int i = 0; i < Connections.Length; i++)
            {
                
                if (i >= verify)
                {
                    AddConnection(i);
                    verify++;
                }else if (Connections[i] == null)
                {
                    AddConnection(i);
                }

            }
        }else if(Connections.Length == verify)
        {
            for (int i = 0; i < Connections.Length; i++)
            {
                if (Connections[i] == null)
                {
                    AddConnection(i);
                }
            }
        }
        else
            verify = Connections.Length;

    }

    private void AddConnection(int i)
    {
        Connections[i] = Instantiate(myPreFab, this.transform.position, Quaternion.identity, this.transform.parent);
        Connections[i].GetComponent<NodeConnections>().myPreFab = myPreFab;
        Connections[i].GetComponent<NodeConnections>().Connections[0] = this.transform.gameObject;
        Connections[i].GetComponent<NodeConnections>().verify++;
        Connections[i].name = "Node(" + Connections[i].GetInstanceID() + ")";
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject getPortal()
    {
        return portal;
    }

    public GameObject getForward()
    {
        return forward;
    }
}
