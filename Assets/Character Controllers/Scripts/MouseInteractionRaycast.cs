using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseInteractionRaycast : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float reachDistance = 5f;
    [SerializeField] private float selectionSize = 1f;
    [SerializeField] private float distanceFromPlayer = 5f;

    public GameObject selectedObject;

    public Vector3 hitPoint;
    public Transform hitTransform;

    //public Image interactionAimIndicator;

    //public bool isDoor;
    [SerializeField] private bool isItem;
    [SerializeField] private bool isInteraction;
    [SerializeField] private bool isEmote;

    [HideInInspector] public bool isItemInteracted;
    [HideInInspector] public bool isEmoteInteracted;
    //[HideInInspector] public bool isConsumableInteracted;

    //[SerializeField] private TextPopUp popUpText;

    //private ToggleDoor door;

    public LayerMask mouseColliderLayerMask = new LayerMask();

    public TextMeshProUGUI interactPromptText;
    //[SerializeField] private TextMeshProUGUI consumePromptText;

    [SerializeField] private PlayerInventory inventorySystem;
    [SerializeField] private EmoteManager emoteManager;
    [SerializeField] private float delayTime = 1f;


    //public AudioSource audioSource;

    // public List<AudioClip> collectAudioList;
    //public List<AudioClip> consumeAudioList;

    private void Awake()
    {
        inventorySystem = FindObjectOfType<PlayerInventory>();
        emoteManager = FindObjectOfType<EmoteManager>();
        interactPromptText.text = "";

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Update()
    {
        //StartCoroutine(InteractionRaycast());

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.SphereCast(ray, selectionSize, out hit, reachDistance, mouseColliderLayerMask))
        {

            hitPoint = hit.point;
            //Debug.Log("Hit!");

            if (hit.transform.GetComponent<ItemInWorld>())
            {
                isItem = true;
                selectedObject = hit.transform.gameObject;

                interactPromptText.text = "Collect " + selectedObject.GetComponent<ItemInWorld>().item.itemName;
            }
            else
            {
                isItem = false;
            }

            if (hit.transform.GetComponent<EmoteInWorld>())
            {
                isEmote = true;
                selectedObject = hit.transform.gameObject;

                interactPromptText.text = "Collect " + selectedObject.GetComponent<EmoteInWorld>().emote.itemName + " Emote";
            }
            else
            {
                isEmote = false;
            }

            // Door Interactions
            #region
            //if (hit.transform.GetComponent<ToggleDoor>() && hit.transform.GetComponent<ToggleDoor>().enabled)
            //{
            //    isDoor = true;
            //    selectedObject = hit.transform.gameObject;

            //    if (!selectedObject.GetComponent<ToggleDoor>().isLocked)
            //    {
            //        if (selectedObject.GetComponent<ToggleDoor>().isOpen)
            //        {
            //            interactPromptText.text = "Close Door [" + selectInput + "]";
            //        }
            //        else
            //        {
            //            interactPromptText.text = "Open Door [" + selectInput + "]";
            //        }
            //    }
            //    else interactPromptText.text = "Locked";

            //}
            //else if (hit.transform.GetComponent<EnteranceDoor>() || hit.transform.GetComponentInChildren<EnteranceDoor>())
            //{
            //    isDoor = true;
            //    selectedObject = hit.transform.gameObject;

            //    for (int i = 0; i < DoorMaster.instance.connectedDoorsList.Count; i++)
            //    {
            //        if (hit.transform.GetComponent<EnteranceDoor>() == DoorMaster.instance.connectedDoorsList[i].exteriorDoor)
            //        {
            //            interactPromptText.text = "Enter [" + selectInput + "]";
            //            break;
            //        }
            //        else if (hit.transform.GetComponent<EnteranceDoor>() == DoorMaster.instance.connectedDoorsList[i].interiorDoor)
            //        {
            //            interactPromptText.text = "Exit [" + selectInput + "]";
            //            break;
            //        }
            //    }
            //}
            //else
            //{
            //    isDoor = false;
            //}
            #endregion
            if (selectedObject != null && Input.GetMouseButton(0))
            {

                if (isItem)
                {
                    isItemInteracted = true;

                    if (Vector3.Distance(player.transform.position, hit.transform.position) <= distanceFromPlayer)
                    {
                        PickUpItem();
                    }
                }

                if (isEmote)
                {
                    isEmoteInteracted = true;

                    if (Vector3.Distance(player.transform.position, hit.transform.position) <= distanceFromPlayer)
                    {
                        PickUpEmote();
                    }
                }

                //StartCoroutine(CheckInventoryIndicator());

                //if (isDoor)
                //{
                //    if (selectedObject.GetComponent<ToggleDoor>())
                //    {
                //        door = selectedObject.GetComponent<ToggleDoor>();
                //        if (!door.isLocked)
                //        {
                //            door.ToggleDoorState();
                //        }
                //        else
                //        {
                //            //communicate locked door to player

                //            interactPromptText.text = "Locked";
                //        }
                //    }
                //    else if (selectedObject.GetComponent<EnteranceDoor>())
                //    {
                //        selectedObject.GetComponent<EnteranceDoor>().WarpEnter(FindObjectOfType<PlayerController>().gameObject);
                //    }
                //}

                if (isInteraction)
                {
                    //get interaction from selected object & perform

                }
                else
                {
                    isInteraction = false;
                }



            }

            if (hit.point != null)
            {
                hitTransform = hit.transform;
                hitPoint = hit.point;
            }

            if (isItem || /*isDoor ||*/ isInteraction || isEmote)
            {
                //GUI.skin.settings.cursorColor = Color.red;
            }
            else
            {
                hitTransform = null;
                hitPoint = Vector3.zero;

                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                //Debug.Log("Did not Hit");
                selectedObject = null;
                interactPromptText.text = "";

                //GUI.skin.settings.cursorColor = Color.white;
            }

        }
        else
        {
            hitTransform = null;
            hitPoint = Vector3.zero;

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //Debug.Log("Did not Hit");
            selectedObject = null;
            interactPromptText.text = "";

            //GUI.skin.settings.cursorColor = Color.white;
        }
    }

  

    public void PickUpItem()
    {
        InventoryItem item = selectedObject.GetComponent<ItemInWorld>().item;

        inventorySystem.AddItemToInventory(item, selectedObject);

        StartCoroutine(DelaySettingFalseVariables());

    }
    public void PickUpEmote()
    {
        Debug.Log("Collecting emote");
        Emote emote = selectedObject.GetComponent<EmoteInWorld>().emote;

        emoteManager.AddEmoteToInventory(emote, selectedObject);

        StartCoroutine(DelaySettingFalseVariables());

    }

    public IEnumerator DelaySettingFalseVariables()
    {
        if (isItemInteracted)
        {
            yield return new WaitForSeconds(delayTime);

            isItemInteracted = false;
        }

         if (isEmoteInteracted)
        {
            yield return new WaitForSeconds(delayTime);

            isEmoteInteracted = false;
        }

        //if (isConsumableInteracted)
        //{
        //    yield return new WaitForSeconds(delayTime);

        //    isConsumableInteracted = false;
        //}

        //if (isBreakableInteracted)
        //{
        //    yield return new WaitForSeconds(delayTime);

        //    isBreakableInteracted = false;
        //}
    }

    //public IEnumerator InteractionRaycast()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
    //    RaycastHit hit;

    //    if (Physics.SphereCast(ray, selectionSize, out hit, reachDistance, ~uiLayer))
    //    {
    //        if (hit.transform.GetComponent<ItemInWorld>())
    //        {
    //            isItem = true;
    //            selectedObject = hit.transform.gameObject;
    //            //Debug.Log("hit = " + selectedObject);
    //            //interactionPromptText.gameObject.SetActive(true);
    //            GUI.skin.settings.cursorColor = Color.red;

    //            interactPromptText.gameObject.SetActive(true);

    //            interactPromptText.text = "Collect " + selectedObject.GetComponent<ItemInWorld>().item.itemName;
    //        }
    //        else
    //        {
    //            isItem = false;
    //        }

    //        //if (hit.transform.GetComponent<ToggleDoor>() && hit.transform.GetComponent<ToggleDoor>().enabled)
    //        //{
    //        //    isDoor = true;
    //        //    selectedObject = hit.transform.gameObject;
    //        //    //interactionPromptText.gameObject.SetActive(true);
    //        //    interactionAimIndicator.color = Color.red;

    //        //    interactPromptText.gameObject.SetActive(true);


    //        //    if (!selectedObject.GetComponent<ToggleDoor>().isLocked)
    //        //    {
    //        //        if (selectedObject.GetComponent<ToggleDoor>().isOpen)
    //        //        {
    //        //            interactPromptText.text = "Close Door";
    //        //        }
    //        //        else
    //        //        {
    //        //            interactPromptText.text = "Open Door";
    //        //        }
    //        //    }
    //        //    else interactPromptText.text = "Locked";

    //        //}
    //        //else
    //        //{
    //        //    isDoor = false;
    //        //}

    //        if (selectedObject != null && Input.GetMouseButton(0))
    //        {
    //            interactPromptText.gameObject.SetActive(false);

    //            if (isItem)
    //            {

    //                isItemInteracted = true;

    //                PickUpItem();
    //            }

    //            //StartCoroutine(CheckInventoryIndicator());

    //            //if (isDoor)
    //            //{
    //            //    door = selectedObject.GetComponent<ToggleDoor>();
    //            //    if (!door.isLocked)
    //            //    {
    //            //        if (door.GetComponent<Animator>().GetBool("isOpen"))
    //            //        {
    //            //            door.GetComponent<Animator>().SetBool("isOpen", !door.isOpen);
    //            //        }

    //            //        if (door.GetComponent<Animator>().GetBool("isOpen"))
    //            //        {
    //            //            transform.GetChild(0).GetComponent<Collider>().enabled = false;
    //            //        }
    //            //        else transform.GetChild(0).GetComponent<Collider>().enabled = true;
    //            //    }
    //            //    else
    //            //    {
    //            //        //communicate locked door to player

    //            //        interactPromptText.text = "Locked";
    //            //    }
    //            //}

    //            if (isInteraction)
    //            {
    //                //get interaction from selected object & perform

    //            }
    //        }
    //    }
    //    else
    //    {

    //        interactPromptText.gameObject.SetActive(false);

    //        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
    //        //Debug.Log("Did not Hit");
    //        selectedObject = null;
    //        //interactPromptIndicator.SetActive(false);

    //        GUI.skin.settings.cursorColor = Color.white;
    //    }


    //    yield return null;
    //}
}
