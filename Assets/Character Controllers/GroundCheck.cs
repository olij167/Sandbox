using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }
    private void OnTriggerEnter(Collider collision)
    {
        //if (collision.gameObject != player)
            player.isGrounded = true;
    }
    
    private void OnTriggerExit(Collider collision)
    {
        //if (collision.gameObject != player)
            player.isGrounded = false;
    }
}
