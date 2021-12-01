using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCheck : MonoBehaviour
{
    [SerializeField] private GameObject Grid;
    [SerializeField] private Collider thisCollider;
    [SerializeField] private bool negative;
    [SerializeField] private Material negativeMaterial;
    [SerializeField] private Material positiveMaterial;



    void OnTriggerEnter(Collider col)
    {
        if (Grid.transform.Find(col.gameObject.name) != null)
        {
            col.gameObject.transform.GetChild(0).gameObject.SetActive(negative);
            ChangeMaterial(col.gameObject);
        }

    }

    void OnTriggerExit(Collider col)
    {
        if (Grid.transform.Find(col.gameObject.name) != null)
        {
            col.gameObject.transform.GetChild(0).gameObject.SetActive(!negative);
            ChangeMaterial(col.gameObject);
        }
            

    }

    private void ChangeMaterial(GameObject obj)
    {
        if(obj.transform.GetChild(0).gameObject.activeSelf)
            obj.GetComponent<MeshRenderer>().material = positiveMaterial;
        else
            obj.GetComponent<MeshRenderer>().material = negativeMaterial;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
