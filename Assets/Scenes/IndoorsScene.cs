using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndoorsScene : MonoBehaviour
{
    public PlayerController player;
    public Transform startingLookPos;
    void Awake()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();

        LoadScene();
    }

    public void LoadScene()
    {
        //player.characterControllerMovement = false;
        Pause.instance.freezeMovement = true;
        player.transform.position = transform.position;
        player.transform.LookAt(startingLookPos);
        Pause.instance.freezeMovement = false;
        //player.characterControllerMovement = true;
    }
    void Update()
    {
        
    }
}
