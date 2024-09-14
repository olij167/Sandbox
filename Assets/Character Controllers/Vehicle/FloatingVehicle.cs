using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingVehicle : Interactable
{
    public float speed = 10f;
    //public float rotationSpeed = 50f;

    public bool beingDriven;

    public Vector3 moveDirection;

    private PlayerController player;

    public Vector3 sittingPositionOffset;
    [SerializeField] private float distanceFromGround = 1f;
    public float verticalHoverDistance = 0.25f;
    public float frequency = 1f;


    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }


    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity))
        {
            Vector3 pos = hit.point;

            Ray returnRay = new Ray(pos, transform.position - pos);

            Vector3 floatDistance = returnRay.GetPoint(distanceFromGround);

            Vector3 tempPos = floatDistance;

            tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * verticalHoverDistance;

            if (beingDriven)
            {
                //move = false;

                moveDirection = (player.orientation.forward * Input.GetAxisRaw("Vertical")) + (player.orientation.right * Input.GetAxisRaw("Horizontal"));
                moveDirection = moveDirection.normalized * speed * Time.deltaTime;
                //moveDirection.y = floatDistance.y;
                //moveDirection.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * verticalHoverDistance;


                //transform.position += moveDirection * Time.deltaTime;
                transform.position = new Vector3(transform.position.x + moveDirection.x, tempPos.y, transform.position.z + moveDirection.z) ;
            }
            else transform.position = tempPos;
        }
    }

    public void SitDown(PlayerController playerController)
    {
        playerController.characterControllerMovement = false;
        playerController.GetComponent<CharacterController>().enabled = false;
        playerController.GetComponent<BoxCollider>().enabled = false;

        playerController.transform.position = transform.position + sittingPositionOffset;

        playerController.transform.parent = transform;

        playerController.animator.SetBool("isSitting", true);

        //playerController.model.SetActive( false);


        beingDriven = true;
    }

    public void StandUp(PlayerController playerController)
    {
        playerController.characterControllerMovement = true;
        playerController.GetComponent<CharacterController>().enabled = true;
        playerController.GetComponent<BoxCollider>().enabled = true;

        playerController.transform.parent = null;
        //playerController.model.SetActive(true);
        playerController.animator.SetBool("isSitting", false);


        beingDriven = false;
    }
}
