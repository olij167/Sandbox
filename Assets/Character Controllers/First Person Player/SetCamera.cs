using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SetCamera : MonoBehaviour
{
    public bool isFirstPerson = true;

    public Camera firstPersonCamera;
    public Transform firstPersonCamPos;

    public Camera thirdPersonCamera;
    public CinemachineFreeLook thirdPersonVirtual;

    public Transform lookTarget;
    public Transform firstPersonLookTargetParent;
    public Transform thirdPersonLookTargetParent;

    public void SetActiveCamera()
    {
        if (isFirstPerson)
        {
            if (!firstPersonCamera.gameObject.activeSelf)
            {
                firstPersonCamera.gameObject.SetActive(true);
                lookTarget.transform.parent = firstPersonLookTargetParent;
                firstPersonCamera.tag = "MainCamera";


            }

            if (firstPersonCamera.transform.position != firstPersonCamPos.position)
                firstPersonCamera.transform.position = firstPersonCamPos.position;

            if (thirdPersonCamera.gameObject.activeSelf)
            {
                thirdPersonVirtual.gameObject.SetActive(false);
                thirdPersonCamera.gameObject.SetActive(false);
                thirdPersonCamera.tag = "Default";

            }
        }
        else
        {
            if (firstPersonCamera.gameObject.activeSelf)
            {
                firstPersonCamera.gameObject.SetActive(false);
                firstPersonCamera.tag = "Default";

            }

            if (!thirdPersonCamera.gameObject.activeSelf)
            {
                thirdPersonVirtual.gameObject.SetActive(true);
                thirdPersonCamera.gameObject.SetActive(true);
                thirdPersonCamera.tag = "MainCamera";
                lookTarget.transform.parent = thirdPersonLookTargetParent;

            }

            //Control third person camera here if needed
        }
    }
}
