using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThirdPersonSelection : MonoBehaviour
{
    [SerializeField] private KeyCode selectInput = KeyCode.F;

    public GameObject selectedObject;

    public List<GameObject> selectedObjects;

    //public Transform hitTransform;

    //public Image interactionAimIndicator;

    [SerializeField] private bool isItem;
   // [SerializeField] private bool isInteraction;
    [SerializeField] private bool isEmote;
    [SerializeField] private bool isAbility;
    [SerializeField] private bool isChest;
    [SerializeField] private bool isCar;
    [SerializeField] private bool isChair;

    [HideInInspector] public bool isItemInteracted;
    [HideInInspector] public bool isEmoteInteracted;
    [HideInInspector] public bool isAbilityInteracted;
    [HideInInspector] public bool isChestInteracted;
    [HideInInspector] public bool isCarInteracted;
    [HideInInspector] public bool isChairInteracted;

    public TextMeshProUGUI interactPromptText;

    [SerializeField] private PlayerInventory inventorySystem;
    [SerializeField] private EmoteManager emoteManager;
    [SerializeField] private PlayerAbilities playerAbilities;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float delayTime = 1f;

    private void Awake()
    {
        inventorySystem = FindObjectOfType<PlayerInventory>();
        emoteManager = FindObjectOfType<EmoteManager>();
        playerAbilities = FindObjectOfType<PlayerAbilities>();
        playerController = FindObjectOfType<PlayerController>();

        interactPromptText.text = "";
    }

    private void Update()
    {
        if (selectedObjects.Count > 0)
        {
            if (selectedObjects[0] != null && selectedObjects[0].GetComponent<Interactable>())
            {
                if (selectedObjects[0].GetComponent<ItemInWorld>())
                {
                    isItem = true;
                    interactPromptText.text = "Collect " + selectedObjects[0].GetComponent<ItemInWorld>().item.itemName;

                    isEmote = false;
                    isAbility = false;
                    isChest = false;
                    isCar = false;
                    isChair = false;

                    //isInteraction = false;
                }
                else if (selectedObjects[0].GetComponent<EmoteInWorld>())
                {
                    isEmote = true;
                    interactPromptText.text = "Collect " + selectedObjects[0].GetComponent<EmoteInWorld>().emote.itemName + " Emote";

                    isItem = false;
                    isAbility = false;
                    isChest = false;
                    isCar = false;
                    isChair = false;

                    //isInteraction = false;
                }
                else if (selectedObjects[0].GetComponent<AbilityInWorld>())
                {
                    isAbility = true;
                    interactPromptText.text = "Collect " + selectedObjects[0].GetComponent<AbilityInWorld>().ability.itemName + " Ability";

                    isItem = false;
                    isEmote = false;
                    isChest = false;
                    isCar = false;
                    isChair = false;

                    //isInteraction = false;
                }
                else if (selectedObjects[0].GetComponent<ChestInventory>())
                {
                    isChest = true;

                    if (!selectedObjects[0].GetComponent<ChestInventory>().chestPanel.activeSelf)
                        interactPromptText.text = "Open Chest";
                    else
                        interactPromptText.text = "Close Chest";

                    isItem = false;
                    isEmote = false;
                    isAbility = false;
                    isCar = false;
                    isChair = false;

                    //isInteraction = false;
                }
                else if (selectedObjects[0].GetComponent<WheelDrive>())
                {
                    isCar = true;

                    if (!selectedObjects[0].GetComponent<WheelDrive>().beingDriven)
                        interactPromptText.text = "Enter Vehicle";
                    else
                        interactPromptText.text = "Exit Vehicle";

                    isItem = false;
                    isEmote = false;
                    isAbility = false;
                    isChest = false;
                    isChair = false;

                    //isInteraction = false;
                }
                else if (selectedObjects[0].GetComponent<Chair>())
                {
                    isChair = true;

                    if (!selectedObjects[0].GetComponent<Chair>().isSitting)
                        interactPromptText.text = "Sit Down";
                    else
                        interactPromptText.text = "Stand Up";

                    isItem = false;
                    isEmote = false;
                    isAbility = false;
                    isChest = false;
                    isCar = false;

                    //isInteraction = false;
                }
                else
                {
                    isItem = false;
                    isEmote = false;
                    isAbility = false;
                    isChest = false;
                    isCar = false;
                    isChair = false;

                    //isInteraction = false;
                }


                if (Input.GetKeyDown(selectInput))
                {

                    if (isItem)
                    {
                        isItemInteracted = true;

                        PickUpItem();
                    }

                    if (isEmote)
                    {
                        isEmoteInteracted = true;


                        PickUpEmote();
                    }

                    if (isAbility)
                    {
                        isAbilityInteracted = true;

                        PickUpAbility();
                    } 
                    
                    if (isChest)
                    {
                        isChestInteracted = true;

                        if (!selectedObject.GetComponent<ChestInventory>().chestPanel.activeSelf)
                            OpenChest();
                        else selectedObject.GetComponent<ChestInventory>().CloseChest();
                    }

                    if (isCar)
                    {
                        isCarInteracted = true;

                        if (!selectedObject.GetComponent<WheelDrive>().beingDriven)
                            EnterCar();
                        else selectedObject.GetComponent<WheelDrive>().ExitCar();
                    }

                    if (isChair)
                    {
                        isChairInteracted = true;

                        if (!selectedObject.GetComponent<Chair>().isSitting)
                            SitDown();
                        else selectedObject.GetComponent<Chair>().StandUp(playerController);
                    }

                    //if (isInteraction)
                    //{
                    //    //get interaction from selected object & perform

                    //}
                    //else
                    //{
                    //    isInteraction = false;
                    //}

                }

                selectedObject = selectedObjects[0];
            }
            else
            {
                selectedObjects.RemoveAt(0);
            }
        }
        else
        {
            selectedObject = null;
            interactPromptText.text = "";
        }
        //if (isItem || isEmote /*|| isInteraction*/)
        //{
        //    interactionAimIndicator.color = Color.red;
        //}
        //else
        //{
        //    interactionAimIndicator.color = Color.white;
        //    interactPromptText.text = "";
        //}
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.GetComponent<Chair>() ||
            other.transform.GetComponent<ItemInWorld>() ||
            other.transform.GetComponent<EmoteInWorld>() ||
            other.transform.GetComponent<AbilityInWorld>() ||
            other.transform.GetComponent<ChestInventory>() ||
            other.transform.GetComponent<WheelDrive>())
        {
            selectedObjects.Add(other.transform.gameObject);

            //interactPromptText.text = "Collect " + selectedObjects[selectedObjects.Count].GetComponent<ItemInWorld>().item.itemName;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<ItemInWorld>() ||
            other.transform.GetComponent<EmoteInWorld>() ||
            other.transform.GetComponent<AbilityInWorld>() ||
            other.transform.GetComponent<ChestInventory>() ||
            other.transform.GetComponent<WheelDrive>() ||
            other.transform.GetComponent<Chair>())
        {
            selectedObjects.Remove(other.transform.gameObject);

            //interactPromptText.text = "Collect " + selectedObjects[selectedObjects.Count].GetComponent<ItemInWorld>().item.itemName;
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
    public void PickUpAbility()
    {
        Debug.Log("Collecting ability");
        Ability ability = selectedObject.GetComponent<AbilityInWorld>().ability;

        playerAbilities.AddAbilityToInventory(ability, selectedObject);

        StartCoroutine(DelaySettingFalseVariables());

    }

    public void OpenChest()
    {
        Debug.Log("Opening Chest");
        ChestInventory chest = selectedObject.GetComponent<ChestInventory>();
        chest.InitialiseChestPanel();

        StartCoroutine(DelaySettingFalseVariables());

    }

    public void EnterCar()
    {
        Debug.Log("Entering Vehicle");
         selectedObject.GetComponent<WheelDrive>().EnterCar();

        StartCoroutine(DelaySettingFalseVariables());

    }
     public void SitDown()
    {
        Debug.Log("Entering Vehicle");
        selectedObject.GetComponent<Chair>().SitDown(playerController);


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

        if (isAbilityInteracted)
        {
            yield return new WaitForSeconds(delayTime);

            isAbilityInteracted = false;
        } 
        
        if (isChestInteracted)
        {
            yield return new WaitForSeconds(delayTime);

            isChestInteracted = false;
        }

        if (isCarInteracted)
        {
            yield return new WaitForSeconds(delayTime);

            isCarInteracted = false;
        }

        if (isChairInteracted)
        {
            yield return new WaitForSeconds(delayTime);

            isChestInteracted = false;
        }
    }
}
