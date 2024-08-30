using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : Interactable
{
    public bool isSitting;
    public Vector3 sittingPositionOffset;

    public void SitDown(PlayerController playerController)
    {
        playerController.characterControllerMovement = false;
        playerController.GetComponent<CharacterController>().enabled = false;
        playerController.GetComponent<CapsuleCollider>().enabled = false;

        playerController.transform.position = transform.position + sittingPositionOffset;

        playerController.transform.parent = transform;

        playerController.animator.SetBool("isSitting", true);

        //playerController.model.SetActive( false);


        isSitting = true;
    }

    public void StandUp(PlayerController playerController)
    {
        playerController.characterControllerMovement = true;
        playerController.GetComponent<CharacterController>().enabled = true;
        playerController.GetComponent<CapsuleCollider>().enabled = true;

        playerController.transform.parent = null;
        //playerController.model.SetActive(true);
        playerController.animator.SetBool("isSitting", false);


        isSitting = false;
    }
}
