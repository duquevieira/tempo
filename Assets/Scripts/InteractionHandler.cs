using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionHandler : MonoBehaviour
{
    const int FOOTCACHE = 6;

    [SerializeField] private Image tapImage;
    [SerializeField] private Image walkingImage;
    private int footCount;
    private GameObject[] footSteps;
    [SerializeField] Pathfinding pathfinder;
    [SerializeField] private GameObject footStep;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject clickFX;
    //[SerializeField] private float holdTime = 1;
    private GameObject clickInstance;
    private Vector2 downClickPoint;
    private float timeOnClick;
    [SerializeField] private TimelineHandler[] timelineHandlers;
    [SerializeField] private TimelineHandler universalHandler;
    [SerializeField] LayerMask raycastMask;
    private TimelineHandler collidedTimeline;
    private int selectedrune;
    [SerializeField] Image runeimage;
    private bool ignoreRay;

    public void SetTime(float value)
    {
        if (selectedrune >= 0 && selectedrune < timelineHandlers.Length)
        {
            timelineHandlers[selectedrune].SetTime(value);
        }
        else if(universalHandler!=null)
        {
            universalHandler.SetTime(value);
        }
    }

    private void ChangeRuneImage(Sprite rune)
    {
        runeimage.sprite = rune;
    }
    
    public void ToggleRaycast(bool value)
    {
        ignoreRay = value;
        Debug.Log(value);
    }


    private Vector3 GetComponentInputPosition()
    {
        return Input.mousePosition;
        //Debug.DrawLine(Input.GetTouch(0).position, playerCamera.transform.position, Color.red);
        //return Input.GetTouch(0).position;
    }

    private bool CanMove()
    {
        return true;
        //return Input.touchCount > 0;
    }

    private bool CanSnap()
    {
        return Input.GetButtonUp("Click");
        //return Input.GetButtonUp("Click");
        /*bool value = true;
        if (Input.touchCount > 0)
            value = false;
        return value;*/
    }

    public void SelectRune(int index, Sprite rune)
    {
        timelineHandlers[selectedrune].Deactivate();
        if (index >= 0)
        {
            universalHandler.Deactivate();
        }
        else
        {
            universalHandler.Initialize();
        }
        selectedrune = index;
        ChangeRuneImage(rune);
        timelineHandlers[selectedrune].Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {

        SetWalkingImage(false);
        SetTapImage(false);
        footSteps = new GameObject[FOOTCACHE];
        footCount = 0;
        selectedrune = - 1;
    }

    // Update is called once per frame
    void Update()
    {
        /*bool isMoving = true;
        bool canRewind = true;
        bool canPlay = true;
        if (pathfinder.RemainingDistance() <= pathfinder.StoppingDistance())
            foreach (TimelineHandler controller in timelineHandlers)
            {
                if (!controller.IsPaused())
                    isMoving = false;
                if (!controller.CanRewind())
                    canRewind = false;
                if (!controller.CanPlay())
                    canPlay = false;
            }*/

        if (CanMove())
        {
            if (pathfinder.RemainingDistance() <= pathfinder.StoppingDistance())
            {
                
                if (pathfinder.PathAmount() > 0)
                {
                    pathfinder.SetNextDestination();
                    if (footCount == footSteps.Length)
                        footCount = 0;
                    if (footSteps[footCount] != null)
                        Destroy(footSteps[footCount]);
                    footSteps[footCount++] = Instantiate(footStep, this.gameObject.transform.position, this.gameObject.transform.rotation, this.gameObject.transform.parent);
                    SetWalkingImage(true);
                }
                else
                {
                    SetWalkingImage(false);
                    pathfinder.CheckRestrictions();
                }

            }
            if (Input.GetButtonUp("Click"))
            {
                if (!ignoreRay)
                {
                    Ray ray = playerCamera.ScreenPointToRay(GetComponentInputPosition());
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastMask))
                    {
                        /*if(Time.realtimeSinceStartup - timeOnClick < holdTime)
                        {*/
                        pathfinder.Djikstra(hit);
                        //Debug.Log(pathFound.Count);
                        //}

                    }
                }
                

            }
        }
        if (Input.GetButtonDown("Click"))
        {
            /*Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;*/
            SetTapImage(true);
            /*if(Physics.Raycast(ray, out hit)){
                collidedTimeline = hit.collider.gameObject.GetComponent<TimelineHandler>();
            }
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastMask))
            {
                downClickPoint = playerCamera.WorldToScreenPoint(hit.point);
                timeOnClick = Time.realtimeSinceStartup;
                if (clickInstance != null)
                    Destroy(clickInstance);
                clickInstance = Instantiate(clickFX, hit.point, Quaternion.identity, this.gameObject.transform.parent);
                Debug.Log(timeOnClick);
            }*/
        }
        /*if (Input.GetButton("Click"))
        {
            if (Time.realtimeSinceStartup - timeOnClick >= holdTime)
            {
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                
                    Vector2 upClickPoint = playerCamera.WorldToScreenPoint(hit.point);
                    int height = Screen.height;
                    int width = Screen.width;
                    TimelineHandler aux = hit.collider.gameObject.GetComponent<TimelineHandler>();
                    if (aux != null&&hit.collider.bounds.Contains(gameObject.transform.position))
                    {
                        if (upClickPoint.x < downClickPoint.x)
                        {
                            if (canRewind)
                            {
                                if (aux.Rewind())
                                {
                                    pathfinder.ClearPath();
                                    SetWalkingImage(false);
                                }
                                Debug.Log("rewind");
                            }
                        }
                        else
                        {
                            if (canPlay)
                            {
                                
                                if (aux.Play())
                                {
                                    pathfinder.ClearPath();
                                    SetWalkingImage(false);
                                }
                                Debug.Log("play");
                            }
                        }
                    }
                }
            }
        }*/
        if (Input.GetButtonUp("Click"))
        {
            SetTapImage(false);
            /*if (collidedTimeline != null&& Time.realtimeSinceStartup - timeOnClick >= holdTime)
            {
                collidedTimeline.Fast();
                collidedTimeline = null;
            }*/
        }
        if (CanSnap())
        {
            if (selectedrune >= 0 && selectedrune < timelineHandlers.Length)
            {
                timelineHandlers[selectedrune].Fast();
            }
            else if (universalHandler != null)
            {
                universalHandler.Fast();
            }
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
}
