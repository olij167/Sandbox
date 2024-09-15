using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingVehicle : Interactable
{
    public float speed = 10f;
    public float jumpHeight = 10f;
    public bool isJumping;
    public bool isFalling;
    public bool isGrounded;

    [SerializeField] private float currentGravScale = 1.0f;

    //public float rotationSpeed = 50f;

    public bool beingDriven;

    public Vector3 moveDirection;

    private PlayerController player;

    public Vector3 sittingPositionOffset;
    [SerializeField] private float distanceFromGround = 1f;
    public float verticalHoverDistance = 0.25f;
    public float frequency = 1f;

    public LayerMask playerLayer;
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }


    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity, ~playerLayer))
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
                moveDirection = moveDirection.normalized * speed;
                //moveDirection.y = floatDistance.y;
                //moveDirection.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * verticalHoverDistance;


                if (Input.GetButtonDown("Jump") && isGrounded)
                {
                    isJumping = true;
                    isFalling = false;
                    isGrounded = false;

                    //moveDirection.y = jumpHeight;
                }

                if (isJumping)
                {

                    if (transform.position.y - floatDistance.y < jumpHeight)
                    {
                        moveDirection.y += jumpHeight;

                    }
                    else  if (transform.position.y - floatDistance.y > jumpHeight) // when they reach the jump height
                    {
                        moveDirection.y = Mathf.Lerp(moveDirection.y, 0, Time.deltaTime);

                        if (moveDirection.y <= 0f)
                        {
                            isJumping = false;
                            isFalling = true;
                        }
                    }

                    transform.position += moveDirection * Time.deltaTime;

                }
                else if (isFalling)
                {
                    if (transform.position.y > tempPos.y)
                    {
                        moveDirection.y += (Physics.gravity.y * currentGravScale);
                        transform.position += moveDirection * Time.deltaTime;
                    }
                    else isFalling = false;
                }
                else// when not jumping
                {
                    isGrounded = true;

                    moveDirection *= Time.deltaTime;
                    transform.position = new Vector3(transform.position.x + moveDirection.x, tempPos.y, transform.position.z + moveDirection.z);
                }

                //transform.position += moveDirection * Time.deltaTime;
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

    public IEnumerator Jump(Vector3 floatDistance)
    {
        yield return new WaitUntil(()=> transform.position.y >= floatDistance.y + jumpHeight);

        isJumping = true;

        yield return new WaitUntil(()=> transform.position.y <= floatDistance.y);

        isJumping = false;
    }
}
