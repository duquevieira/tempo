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
    [SerializeField] TimelineControl[] timelineControllers;
    [SerializeField] Pathfinding pathfinder;
    [SerializeField] private GameObject footStep;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject clickFX;
    [SerializeField] private float holdTime = 1;
    private GameObject clickInstance;
    private Vector2 downClickPoint;
    private float timeOnClick;

    // Start is called before the first frame update
    void Start()
    {

        SetWalkingImage(false);
        SetTapImage(false);
        footSteps = new GameObject[FOOTCACHE];
        footCount = 0;
        foreach (TimelineControl controller in timelineControllers)
        {
            controller.Pause();
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool isMoving = true;
        bool canRewind = true;
        bool canPlay = true;

        foreach (TimelineControl controller in timelineControllers)
            if (pathfinder.RemainingDistance() <= pathfinder.StoppingDistance())
            {
                if (!controller.IsPaused())
                    isMoving = false;
                if (!controller.CanRewind())
                    canRewind = false;
                if (!controller.CanPlay())
                    canPlay = false;
            }
        if (isMoving)
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
                }

            }
            if (Input.GetButtonUp("Click"))
            {
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if(Time.realtimeSinceStartup - timeOnClick < holdTime)
                    {
                        pathfinder.Djikstra(hit);
                        //Debug.Log(pathFound.Count);
                    }
                    
                }

            }
        }
        if (Input.GetButtonDown("Click"))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            SetTapImage(true);
            if (Physics.Raycast(ray, out hit))
            {
                downClickPoint = playerCamera.WorldToScreenPoint(hit.point);
                timeOnClick = Time.realtimeSinceStartup;
                if (clickInstance != null)
                    Destroy(clickInstance);
                clickInstance = Instantiate(clickFX, hit.point, Quaternion.identity, this.gameObject.transform.parent);
                Debug.Log(timeOnClick);
            }
        }
        if (Input.GetButton("Click"))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                if (Time.realtimeSinceStartup - timeOnClick >= holdTime)
                {
                    Vector2 upClickPoint = playerCamera.WorldToScreenPoint(hit.point);
                    int height = Screen.height;
                    int width = Screen.width;
                    if (upClickPoint.x < downClickPoint.x)
                    {
                        if (canRewind)
                        {
                            foreach (TimelineControl controller in timelineControllers)
                            {
                                if (!controller.IsRewinding() || controller.IsPaused())
                                {
                                    controller.Rewind();
                                    controller.IgnoreTime(true);
                                    pathfinder.ClearPath();
                                    SetWalkingImage(false);
                                }
                            }
                            Debug.Log("rewind");
                        }
                    }
                    else
                    {
                        if (canPlay)
                        {
                            foreach (TimelineControl controller in timelineControllers)
                            {
                                if (controller.IsRewinding()||controller.IsPaused())
                                {
                                    controller.Play();
                                    controller.IgnoreTime(true);
                                    pathfinder.ClearPath();
                                    SetWalkingImage(false);
                                }
                            }
                            Debug.Log("play");
                        }
                    }
                }
            }
        }
        if (Input.GetButtonUp("Click"))
        {
            SetTapImage(false);
            foreach (TimelineControl controller in timelineControllers)
            {
                controller.Fast();
                controller.IgnoreTime(false);
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
