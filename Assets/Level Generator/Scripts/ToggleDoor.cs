using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDoor : Interactable
{
    Animator animator;
    bool isInTrigger;
    public bool isOpen;

    private void Start()
    {
        animator = GetComponent<Animator>();
        isOpen = animator.GetBool("isOpen");

    }

    //private void Update()
    //{
    //    if (isInTrigger && Input.GetKeyDown(KeyCode.E))
    //    {
    //       bool isOpen = animator.GetBool("isOpen");

    //        animator.SetBool("isOpen", !isOpen);
    //    }

    //    if (animator.GetBool("isOpen"))
    //    {
    //        transform.GetChild(0).GetComponent<Collider>().enabled = false;
    //    }
    //    else transform.GetChild(0).GetComponent<Collider>().enabled = true;
    //}

    public void DoorInteraction()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);

        if (animator.GetBool("isOpen"))
        {
            transform.GetChild(0).GetComponent<Collider>().enabled = false;
        }
        else transform.GetChild(0).GetComponent<Collider>().enabled = true;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        isInTrigger = true;
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isOpen)
        {
            DoorInteraction();
            //isInTrigger = false;
        }
    }
}
