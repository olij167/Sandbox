using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInScene : MonoBehaviour
{
    public PlayerController player;
    public Transform startingLookPos;
    private Pause pause;
    void Awake()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();

        if (Pause.instance == null) pause = FindObjectOfType<Pause>();
        else pause = Pause.instance;

        LoadScene();
    }

    public void LoadScene()
    {
        //player.characterControllerMovement = false;
        pause.freezeMovement = true;
        player.transform.position = transform.position;
        player.transform.LookAt(startingLookPos);
        pause.freezeMovement = false;
        //player.characterControllerMovement = true;
    }
    void Update()
    {
        
    }
}
