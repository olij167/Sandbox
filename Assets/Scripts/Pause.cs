using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public static Pause instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    private PlayerController player;
    private ThirdPersonCam cam;
    private PlayerInventory inventory;
    private WildernessGuide guide;

    [field: ReadOnlyField] public bool inventoryOpen;
    [field: ReadOnlyField] public bool chestOpen;
    [field: ReadOnlyField] public bool shopOpen;
    [field: ReadOnlyField] public bool guideOpen;

    public bool freezeMovement;
    public bool freezeCameraRotation;
    public bool freezeCameraOrbit;
    public bool unlockCursor;
    public bool freezeTime;

    private bool _freezeMovement;
    private bool _freezeCameraRotation;
    private bool _freezeCameraOrbit;
    private bool _unlockCursor;
    private bool _freezeTime;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        cam = FindObjectOfType<ThirdPersonCam>();
        inventory = FindObjectOfType<PlayerInventory>();
        guide = FindObjectOfType<WildernessGuide>();
    }

    void Update()
    {
        inventoryOpen = inventory.inventoryWindowOpen;
        chestOpen = inventory.chestPanel.activeSelf;
        shopOpen = inventory.shopParent.activeSelf;
        guideOpen = guide.journalPanel.activeSelf;

        if (inventoryOpen || guideOpen || chestOpen || shopOpen)
        {
            freezeCameraRotation = true;
            unlockCursor = true;
        }
        //else
        //{
        //    freezeCameraRotation = false;
        //    freezeCameraOrbit = false;
        //}


        if (_freezeMovement != freezeMovement)
            FreezeMovement();

         if (_freezeCameraRotation != freezeCameraRotation)
            FreezeCamRotation();
        
        if (_freezeCameraOrbit != freezeCameraOrbit)
            FreezeCamOrbit();

        if (_unlockCursor != unlockCursor)
            ToggleCursor();
        
        if (_freezeTime != freezeTime)
            FreezeTime();

    }

    private void FreezeMovement()
    {
        _freezeMovement = freezeMovement;
        if (_freezeMovement)
        {
            player.characterControllerMovement = false;
            player.GetComponent<CharacterController>().enabled = false;
            player.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            player.characterControllerMovement = true;
            player.GetComponent<CharacterController>().enabled = true;
            player.GetComponent<BoxCollider>().enabled = true;
        }
    }

    private void FreezeCamRotation()
    {
        _freezeCameraRotation = freezeCameraRotation;
        if (_freezeCameraRotation)
        {
            cam.freezeCameraRotation = true;
            //unlockCursor = true;
        }
        else
        {
            cam.freezeCameraRotation = false;
            //unlockCursor = false;
        }
    }
    private void FreezeCamOrbit()
    {
        _freezeCameraOrbit = freezeCameraOrbit;
        if (_freezeCameraOrbit)
        {
            cam.freezeOrientation = true;
            //unlockCursor = true;
        }
        else
        {
            cam.freezeOrientation = false;
            //unlockCursor = false;
        }
    }

    private void ToggleCursor()
    {
        _unlockCursor = unlockCursor;
        if (_unlockCursor)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            player.GetComponent<PlayerAttack>().enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            player.GetComponent<PlayerAttack>().enabled = true;
        }
    }

    private void FreezeTime()
    {
        _freezeTime = freezeTime;
        if (_freezeTime)
             Time.timeScale = 0f;
        else
             Time.timeScale = 1f;
    }
}
