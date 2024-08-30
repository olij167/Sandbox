using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonCam : MonoBehaviour
{
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public CharacterController cc;

    public float rotationSpeed;

    private ToggleCursorLock toggleCursor;

    public bool rotateCameraOnInput;
    public bool freezeCameraRotation;

    public bool freezeOrientation;

    public CinemachineFreeLook thirdPersonVirtual;

    private void Start()
    {
       // toggleCursor.ToggleCursorMode();
    }

    private void Update()
    {
        if (!freezeOrientation)
        {
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir.normalized;


            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

        if (rotateCameraOnInput)
        {
            if (Input.GetMouseButton(1))
                thirdPersonVirtual.enabled = true;
            else thirdPersonVirtual.enabled = false;
        }

        if (!freezeCameraRotation)
            thirdPersonVirtual.enabled = true;
        else thirdPersonVirtual.enabled = false;
    }
}
