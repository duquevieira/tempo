using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatterEffectHandler : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    private float maxX;
    private float maxY;
    private float minX;
    private float minY;
    private float maxZ;
    private float minZ;
    [SerializeField] Vector3 scale;
    [SerializeField] [Range(0, 1)] private float riples;
    private GameObject[] cache;
    private int index;
    [SerializeField] BoxCollider box;
    const int MAXCACHE = 30;

    // Start is called before the first frame update
    void Start()
    {
        maxX = box.bounds.max.x;
        minX = box.bounds.min.x;
        maxY = box.bounds.max.y;
        minY = box.bounds.min.y;
        maxZ = box.bounds.max.z;
        minZ = box.bounds.min.z;
        Debug.Log(maxX);
        Debug.Log(minX);
        Debug.Log(maxY);
        Debug.Log(minY);
        Debug.Log(maxZ);
        Debug.Log(minZ);
        cache = new GameObject[MAXCACHE];
        index = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float randomNum = Random.Range(0, 1f);
        if (randomNum < riples)
        {
            float x = Random.Range(minX, maxX);
            float y = maxY;
            float z = Random.Range(minX, maxX);
            if(cache[index] != null)
            {
                Destroy(cache[index]);
            }
            cache[index] = Instantiate(prefab,new Vector3(x,y,z), Quaternion.identity, transform);
            cache[index].transform.localScale = scale;
            if (index == MAXCACHE-1)
            {

                index = 0;
            }
            else
                index++;
        }
    }
}
