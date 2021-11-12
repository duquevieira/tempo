using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;


public class Player : MonoBehaviour
{
    public NavMeshAgent playerNavMeshAgent;
    public Camera playerCamera;
    public ThirdPersonCharacter character;

    //private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        playerNavMeshAgent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray myRay = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit myRaycastHit;

            if (Physics.Raycast(myRay, out myRaycastHit))
            {
                playerNavMeshAgent.SetDestination(myRaycastHit.point);
            }
        }
        if(playerNavMeshAgent.remainingDistance > playerNavMeshAgent.stoppingDistance)
        {
            character.Move(playerNavMeshAgent.desiredVelocity,false,false);
        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }
    }

}
