using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraControl : MonoBehaviour
{
    private Vector2 previousPos;
    [SerializeField] [Range(0, 1)] private float strength;
    [SerializeField] [Range(1, 2)] private float acceleration;
    private Vector2 current;
    private Vector2 target;
    private float velocityx;
    private float velocityy;
    // Start is called before the first frame update
    void Start()
    {
        int x = Screen.width / 2;
        int y = Screen.height / 2;
        previousPos = new Vector2(x, y);
        velocityx = 1;
        velocityy = 1;
    }

    void currentx(float value)
    {
        current.x = value;
    }
    void currenty(float value)
    {
        current.y = value;
    }
    // Update is called once per frame
    void Update()
    {
        if (/*Input.touchCount > 0*/Input.GetButton("Click"))
        {
            //previousPos = Input.GetTouch(0).position;
            target = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
           
        }
        else
        {
            int x = Screen.width / 2;
            int y = Screen.height / 2;
            target = new Vector2(x, y);

        }
        current = previousPos;
        if (current.x > target.x)
        {
            
            if (velocityx * acceleration >= 1 && current.x - velocityx * acceleration > target.x)
            {
                velocityx = velocityx * acceleration;
            }
            else if (velocityx / acceleration >= 1)
            {
                velocityx = velocityx / acceleration;
            }
            if (current.x - velocityx > target.x)
            {
                current.x = current.x - velocityx;
            }
            else
            {
                current.x = target.x;
            }
        }
        else
        {
            
            if (velocityx * acceleration >= 1 && current.x + velocityx * acceleration < target.x)
            {
                velocityx= velocityx * acceleration;
            }
            else if (velocityx / acceleration >= 1)
            {
                velocityx = velocityx / acceleration;
            }
            if (current.x + velocityx < target.x)
            {
                current.x = current.x + velocityx;
            }
            else
            {
                current.x = target.x;
            }
        }
        if (current.y > target.y)
        {
            
            if (velocityy * acceleration >= 1 && current.y - velocityy * acceleration > target.y)
            {
                velocityy = velocityy * acceleration;
            }
            else if (velocityy / acceleration >= 1)
            {
                velocityy = velocityy / acceleration;
            }
            if (current.y - velocityy > target.y)
            {
                current.y = current.y - velocityy;
            }
            else
            {
                current.y = target.y;
            }
        }
        else
        {
            
            if (velocityy * acceleration >= 1 && current.y + velocityy * acceleration < target.y)
            {
                velocityy = velocityy * acceleration;
            }
            else if (velocityy / acceleration >= 1)
            {
                velocityy = velocityy / acceleration;
            }

            if (current.y + velocityy < target.y)
            {
                current.y = current.y + velocityy;
            }
            else
            {
                current.y = target.y;
            }
        }
        Vector2 diference = current - previousPos;
        diference = diference * (strength / 10);
        previousPos = current;
        gameObject.transform.Rotate(new Vector3(diference.y, diference.x, 0));
    }
}
