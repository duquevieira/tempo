using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MouseOver : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject obj;

    void Start()
    {
        obj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        bool test = false;
        for(int i=0; i<raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject == this.gameObject)
                test = true;
        }
        obj.SetActive(test);
    }
}
