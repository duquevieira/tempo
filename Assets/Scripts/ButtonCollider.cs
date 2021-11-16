using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonCollider : MonoBehaviour
{

    [SerializeField] private UnityEvent[] Events;
    [SerializeField] private string[] Buttons;
    [SerializeField] private GameObject[] Colliders;


    public void OnTriggerStay()
    {
        for (int i = 0; i < Events.Length || i < Buttons.Length; i++)
        {
            if (i < Colliders.Length)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.Equals(Colliders[i]))
                    {
                        if (Input.GetButtonDown(Buttons[i]))
                        {
                            if (Events[i] != null)
                            {
                                Events[i].Invoke();
                            }
                        }
                    }
                }
            }
            else
            {
                if (Input.GetButtonDown(Buttons[i]))
                {
                    if (Events[i] != null)
                    {
                        Events[i].Invoke();
                    }
                }
            }
        }
    }
}