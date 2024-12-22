using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ThirdPersonSelection : MonoBehaviour
{
    [SerializeField] private KeyCode input1 = KeyCode.F;
    [SerializeField] private KeyCode input2 = KeyCode.G;
    [SerializeField] private KeyCode input3 = KeyCode.H;

    public List<SelectedObjectType> interactions;

    public bool keyDown = false;
    public bool leftMouseDown = false;

    public float inputTimer = 0;

    public GameObject selectedObject;

    public List<GameObject> selectedObjects;

    public int firstInteractableIndex = 0;

    //public Transform hitTransform;

    //public Image interactionAimIndicator;

    [System.Serializable]
    public enum SelectedObjectType
    {
        Item, Plant, Emote, Ability, Chest, Car, Chair, FloatingVehicle, SceneTransition, Door, Shop, Sleep, Rope, Entity
    }

    //[SerializeField] private bool isItem;
    //[SerializeField] private bool isPlant;
    //// [SerializeField] private bool isInteraction;
    //[SerializeField] private bool isEmote;
    //[SerializeField] private bool isAbility;
    //[SerializeField] private bool isChest;
    //[SerializeField] private bool isCar;
    //[SerializeField] private bool isChair;
    //[SerializeField] private bool isFloatingVehicle;
    //[SerializeField] private bool isSceneTransition;
    //[SerializeField] private bool isDoor;
    //[SerializeField] private bool isShop;
    //[SerializeField] private bool isSleep;
    ////public bool isClimbable;

    [HideInInspector] public bool isItemInteracted;
    [HideInInspector] public bool isPlantInteracted;
    [HideInInspector] public bool isEmoteInteracted;
    [HideInInspector] public bool isAbilityInteracted;
    [HideInInspector] public bool isChestInteracted;
    [HideInInspector] public bool isCarInteracted;
    [HideInInspector] public bool isChairInteracted;
    [HideInInspector] public bool isFloatingVehicleInteracted;
    [HideInInspector] public bool isSceneTransitionInteracted;
    [HideInInspector] public bool isDoorInteracted;
    [HideInInspector] public bool isShopInteracted;
    [HideInInspector] public bool isSleepInteracted;
    [HideInInspector] public bool isRopeInteracted;
    [HideInInspector] public bool isEntityInteracted;


    public TextMeshProUGUI interactPrompt1Text;
    public TextMeshProUGUI interactPrompt2Text;
    public TextMeshProUGUI interactPrompt3Text;

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

        interactPrompt1Text.text = "";
        interactPrompt2Text.text = "";
        interactPrompt3Text.text = "";
    }

    private void Update()
    {
        if (selectedObjects.Count > 0)
        {
            if (selectedObjects[0] != null)
            {

                if (selectedObjects[0].CompareTag("Climbable"))
                {
                    playerController.canClimb = true;

                    for (int i = 0; i < selectedObjects.Count; i++)
                    {
                        if (selectedObjects[i] == null)
                            selectedObjects.RemoveAt(i);
                        else if (!selectedObjects[i].CompareTag("Climbable"))
                            firstInteractableIndex = i;
                    }
                }
                else firstInteractableIndex = 0;

                if (selectedObjects.Count <= firstInteractableIndex) firstInteractableIndex = 0;


                if (selectedObjects[firstInteractableIndex].GetComponent<Interactable>() || selectedObjects[firstInteractableIndex].transform.CompareTag("Climbable"))
                {

                    if (selectedObjects[firstInteractableIndex].GetComponent<ItemInWorld>() && (inventorySystem.selectedInventoryItem == null || selectedObjects[firstInteractableIndex] != inventorySystem.selectedInventoryItem.physicalItem))
                    {
                        //isItem = true;
                        if (selectedObjects[firstInteractableIndex].GetComponent<ProduceController>() && selectedObjects[firstInteractableIndex].GetComponent<ProduceController>().stillPlanted)
                        {
                            if (!interactions.Contains(SelectedObjectType.Plant))
                                interactions.Add(SelectedObjectType.Plant);

                            //isPlant = true;
                        }
                        else
                        { /*isPlant = false;*/
                            if (interactions.Contains(SelectedObjectType.Plant)) interactions.Remove(SelectedObjectType.Plant);

                            if (!interactions.Contains(SelectedObjectType.Item))
                                interactions.Add(SelectedObjectType.Item);

                        }

                        if (interactions.Contains(SelectedObjectType.Plant))
                        {
                            if (selectedObjects[firstInteractableIndex].GetComponent<ProduceController>().produceQuality != ProduceQuality.Growing)
                            {
                                for (int i = 0; i < interactions.Count; i++)
                                {
                                    if (interactions[i] == SelectedObjectType.Plant)
                                        switch (i + 1)
                                        {
                                            case 1:
                                                interactPrompt1Text.text = "[" + input1 + "] Harvest " + selectedObjects[firstInteractableIndex].GetComponent<ItemInWorld>().item.itemName;
                                                break;
                                            case 2:
                                                interactPrompt2Text.text = "[" + input2 + "] Harvest " + selectedObjects[firstInteractableIndex].GetComponent<ItemInWorld>().item.itemName;
                                                break;
                                            case 3:
                                                interactPrompt3Text.text = "[" + input3 + "] Harvest " + selectedObjects[firstInteractableIndex].GetComponent<ItemInWorld>().item.itemName;
                                                break;
                                        }
                                }
                            }
                            else
                            {
                                //interactPrompt1Text.text = selectedObjects[firstInteractableIndex].GetComponent<ItemInWorld>().item.itemName + " isn't ready to harvest yet";
                                for (int i = 0; i < interactions.Count; i++)
                                {
                                    if (interactions[i] == SelectedObjectType.Plant)
                                        switch (i + 1)
                                        {
                                            case 1:
                                                interactPrompt1Text.text = selectedObjects[firstInteractableIndex].GetComponent<ItemInWorld>().item.itemName + " isn't ready to harvest yet";
                                                break;
                                            case 2:
                                                interactPrompt2Text.text = selectedObjects[firstInteractableIndex].GetComponent<ItemInWorld>().item.itemName + " isn't ready to harvest yet";
                                                break;
                                            case 3:
                                                interactPrompt3Text.text = selectedObjects[firstInteractableIndex].GetComponent<ItemInWorld>().item.itemName + " isn't ready to harvest yet";
                                                break;
                                        }
                                }

                                if (interactions.Contains(SelectedObjectType.Item)) interactions.Remove(SelectedObjectType.Item);

                                if (interactions.Contains(SelectedObjectType.Plant)) interactions.Remove(SelectedObjectType.Plant);

                            }
                        }
                        else
                        {
                            for (int i = 0; i < interactions.Count; i++)
                            {
                                if (interactions[i] == SelectedObjectType.Item)
                                    switch (i + 1)
                                    {
                                        case 1:
                                            interactPrompt1Text.text = "[" + input1 + "] Collect " + selectedObjects[firstInteractableIndex].GetComponent<ItemInWorld>().item.itemName;
                                            break;
                                        case 2:
                                            interactPrompt2Text.text = "[" + input2 + "] Collect " + selectedObjects[firstInteractableIndex].GetComponent<ItemInWorld>().item.itemName;
                                            break;
                                        case 3:
                                            interactPrompt3Text.text = "[" + input3 + "] Collect " + selectedObjects[firstInteractableIndex].GetComponent<ItemInWorld>().item.itemName;
                                            break;
                                    }
                            }
                        }
                    }
                    else if (interactions.Contains(SelectedObjectType.Item)) interactions.Remove(SelectedObjectType.Item);


                    if (selectedObjects[firstInteractableIndex].GetComponent<EmoteInWorld>())
                    {
                        //isEmote = true;
                        if (!interactions.Contains(SelectedObjectType.Emote))
                            interactions.Add(SelectedObjectType.Emote);

                        for (int i = 0; i < interactions.Count; i++)
                        {
                            if (interactions[i] == SelectedObjectType.Emote)
                                switch (i + 1)
                                {
                                    case 1:
                                        interactPrompt1Text.text = "[" + input1 + "] Collect " + selectedObjects[firstInteractableIndex].GetComponent<EmoteInWorld>().emote.itemName + " Emote";
                                        break;
                                    case 2:
                                        interactPrompt2Text.text = "[" + input2 + "] Collect " + selectedObjects[firstInteractableIndex].GetComponent<EmoteInWorld>().emote.itemName + " Emote";
                                        break;
                                    case 3:
                                        interactPrompt3Text.text = "[" + input3 + "] Collect " + selectedObjects[firstInteractableIndex].GetComponent<EmoteInWorld>().emote.itemName + " Emote";
                                        break;
                                }
                        }
                    }
                    else if (interactions.Contains(SelectedObjectType.Emote)) interactions.Remove(SelectedObjectType.Emote);

                    if (selectedObjects[firstInteractableIndex].GetComponent<AbilityInWorld>())
                    {
                        //isAbility = true;
                        if (!interactions.Contains(SelectedObjectType.Ability))
                            interactions.Add(SelectedObjectType.Ability);

                        for (int i = 0; i < interactions.Count; i++)
                        {
                            if (interactions[i] == SelectedObjectType.Ability)
                                switch (i + 1)
                                {
                                    case 1:
                                        interactPrompt1Text.text = "[" + input1 + "] Collect " + selectedObjects[firstInteractableIndex].GetComponent<AbilityInWorld>().ability.itemName + " Ability";
                                        break;
                                    case 2:
                                        interactPrompt2Text.text = "[" + input2 + "] Collect " + selectedObjects[firstInteractableIndex].GetComponent<AbilityInWorld>().ability.itemName + " Ability";
                                        break;
                                    case 3:
                                        interactPrompt3Text.text = "[" + input3 + "] Collect " + selectedObjects[firstInteractableIndex].GetComponent<AbilityInWorld>().ability.itemName + " Ability";
                                        break;
                                }

                        }
                    }
                    else if (interactions.Contains(SelectedObjectType.Ability)) interactions.Remove(SelectedObjectType.Ability);


                    if (selectedObjects[firstInteractableIndex].GetComponent<ChestInventory>())
                    {
                        //isChest = true;
                        if (!interactions.Contains(SelectedObjectType.Chest))
                            interactions.Add(SelectedObjectType.Chest);

                        for (int i = 0; i < interactions.Count; i++)
                        {
                            if (interactions[i] == SelectedObjectType.Chest)
                                switch (i + 1)
                                {
                                    case 1:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<ChestInventory>().chestPanel.activeSelf)
                                            interactPrompt1Text.text = "[" + input1 + "] Open Chest";
                                        else
                                            interactPrompt1Text.text = "[" + input1 + "] Close Chest";
                                        break;
                                    case 2:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<ChestInventory>().chestPanel.activeSelf)
                                            interactPrompt2Text.text = "[" + input2 + "] Open Chest";
                                        else
                                            interactPrompt2Text.text = "[" + input2 + "] Close Chest";
                                        break;
                                    case 3:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<ChestInventory>().chestPanel.activeSelf)
                                            interactPrompt3Text.text = "[" + input3 + "] Open Chest";
                                        else
                                            interactPrompt3Text.text = "[" + input3 + "] Close Chest";
                                        break;
                                }
                        }
                    }
                    else if (interactions.Contains(SelectedObjectType.Chest)) interactions.Remove(SelectedObjectType.Chest);

                    if (selectedObjects[firstInteractableIndex].GetComponent<WheelDrive>())
                    {
                        //isCar = true;
                        if (!interactions.Contains(SelectedObjectType.Car))
                            interactions.Add(SelectedObjectType.Car);

                        for (int i = 0; i < interactions.Count; i++)
                        {
                            if (interactions[i] == SelectedObjectType.Car)
                                switch (i + 1)
                                {
                                    case 1:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<WheelDrive>().beingDriven)
                                            interactPrompt1Text.text = "[" + input1 + "] Enter Vehicle";
                                        else
                                            interactPrompt1Text.text = "[" + input1 + "] Exit Vehicle";
                                        break;
                                    case 2:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<WheelDrive>().beingDriven)
                                            interactPrompt2Text.text = "[" + input2 + "] Enter Vehicle";
                                        else
                                            interactPrompt2Text.text = "[" + input2 + "] Exit Vehicle";
                                        break;
                                    case 3:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<WheelDrive>().beingDriven)
                                            interactPrompt3Text.text = "[" + input3 + "] Enter Vehicle";
                                        else
                                            interactPrompt3Text.text = "[" + input3 + "] Exit Vehicle";
                                        break;
                                }
                        }
                    }
                    else if (interactions.Contains(SelectedObjectType.Car)) interactions.Remove(SelectedObjectType.Car);

                    if (selectedObjects[firstInteractableIndex].GetComponent<Chair>())
                    {
                        //isChair = true;
                        if (!interactions.Contains(SelectedObjectType.Chair))
                            interactions.Add(SelectedObjectType.Chair);

                        for (int i = 0; i < interactions.Count; i++)
                        {
                            if (interactions[i] == SelectedObjectType.Chair)
                                switch (i + 1)
                                {
                                    case 1:
                                        if (!selectedObjects[0].GetComponent<Chair>().isSitting)
                                            interactPrompt1Text.text = "[" + input1 + "] Sit Down";
                                        else
                                            interactPrompt1Text.text = "[" + input1 + "] Stand Up";
                                        break;
                                    case 2:
                                        if (!selectedObjects[0].GetComponent<Chair>().isSitting)
                                            interactPrompt2Text.text = "[" + input2 + "] Sit Down";
                                        else
                                            interactPrompt2Text.text = "[" + input2 + "] Stand Up";
                                        break;
                                    case 3:
                                        if (!selectedObjects[0].GetComponent<Chair>().isSitting)
                                            interactPrompt3Text.text = "[" + input3 + "] Sit Down";
                                        else
                                            interactPrompt3Text.text = "[" + input3 + "] Stand Up";
                                        break;
                                }
                        }
                    }
                    else if (interactions.Contains(SelectedObjectType.Chair)) interactions.Remove(SelectedObjectType.Chair);

                    if (selectedObjects[0].GetComponent<FloatingVehicle>())
                    {
                        //isFloatingVehicle = true;
                        if (!interactions.Contains(SelectedObjectType.FloatingVehicle))
                            interactions.Add(SelectedObjectType.FloatingVehicle);

                        for (int i = 0; i < interactions.Count; i++)
                        {
                            if (interactions[i] == SelectedObjectType.FloatingVehicle)
                                switch (i + 1)
                                {
                                    case 1:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<FloatingVehicle>().beingDriven)
                                            interactPrompt1Text.text = "[" + input1 + "] Enter Vehicle";
                                        else
                                            interactPrompt1Text.text = "[" + input1 + "] Exit Vehicle";
                                        break;
                                    case 2:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<FloatingVehicle>().beingDriven)
                                            interactPrompt2Text.text = "[" + input2 + "] Enter Vehicle";
                                        else
                                            interactPrompt2Text.text = "[" + input2 + "] Exit Vehicle";
                                        break;
                                    case 3:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<FloatingVehicle>().beingDriven)
                                            interactPrompt3Text.text = "[" + input3 + "] Enter Vehicle";
                                        else
                                            interactPrompt3Text.text = "[" + input3 + "] Exit Vehicle";
                                        break;
                                }
                        }
                    }
                    else if (interactions.Contains(SelectedObjectType.FloatingVehicle)) interactions.Remove(SelectedObjectType.FloatingVehicle);

                    if (selectedObjects[0].GetComponent<SceneLoader>())
                    {
                        //isSceneTransition = true;
                        if (!interactions.Contains(SelectedObjectType.SceneTransition))
                            interactions.Add(SelectedObjectType.SceneTransition);

                        for (int i = 0; i < interactions.Count; i++)
                        {
                            if (interactions[i] == SelectedObjectType.SceneTransition)
                                switch (i + 1)
                                {
                                    case 1:
                                        interactPrompt1Text.text = "[" + input1 + "] Enter " + selectedObjects[0].GetComponent<SceneLoader>().sceneToLoad;
                                        break;
                                    case 2:
                                        interactPrompt2Text.text = "[" + input2 + "] Enter " + selectedObjects[0].GetComponent<SceneLoader>().sceneToLoad;
                                        break;
                                    case 3:
                                        interactPrompt3Text.text = "[" + input3 + "] Enter " + selectedObjects[0].GetComponent<SceneLoader>().sceneToLoad;
                                        break;
                                }
                        }
                    }
                    else if (interactions.Contains(SelectedObjectType.SceneTransition)) interactions.Remove(SelectedObjectType.SceneTransition);

                    if (selectedObjects[0].GetComponent<ToggleDoor>())
                    {
                        //isDoor = true;
                        if (!interactions.Contains(SelectedObjectType.Door))
                            interactions.Add(SelectedObjectType.Door);

                        for (int i = 0; i < interactions.Count; i++)
                        {
                            if (interactions[i] == SelectedObjectType.Door)
                                switch (i + 1)
                                {
                                    case 1:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<ToggleDoor>().isOpen)
                                            interactPrompt1Text.text = "[" + input1 + "] Open Door";
                                        else
                                            interactPrompt1Text.text = "[" + input1 + "] Close Door";
                                        break;
                                    case 2:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<ToggleDoor>().isOpen)
                                            interactPrompt2Text.text = "[" + input2 + "] Open Door";
                                        else
                                            interactPrompt2Text.text = "[" + input2 + "] Close Door";
                                        break;
                                    case 3:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<ToggleDoor>().isOpen)
                                            interactPrompt3Text.text = "[" + input3 + "] Open Door";
                                        else
                                            interactPrompt3Text.text = "[" + input3 + "] Close Door";
                                        break;
                                }
                        }

                    }
                    else if (interactions.Contains(SelectedObjectType.Door)) interactions.Remove(SelectedObjectType.Door);

                    if (selectedObjects[0].GetComponent<ShopInventory>())
                    {
                        //isShop = true;
                        if (!interactions.Contains(SelectedObjectType.Shop))
                            interactions.Add(SelectedObjectType.Shop);

                        for (int i = 0; i < interactions.Count; i++)
                        {
                            if (interactions[i] == SelectedObjectType.Shop)
                                switch (i + 1)
                                {
                                    case 1:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<ShopInventory>().shopPanel.activeSelf || !selectedObjects[firstInteractableIndex].GetComponent<ShopInventory>().shopPanel.transform.parent.gameObject.activeSelf)
                                            interactPrompt1Text.text = "[" + input1 + "] Open Shop";
                                        else
                                            interactPrompt1Text.text = "[" + input1 + "] Close Shop";
                                        break;
                                    case 2:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<ShopInventory>().shopPanel.activeSelf || !selectedObjects[firstInteractableIndex].GetComponent<ShopInventory>().shopPanel.transform.parent.gameObject.activeSelf)
                                            interactPrompt2Text.text = "[" + input2 + "] Open Shop";
                                        else
                                            interactPrompt2Text.text = "[" + input2 + "] Close Shop";
                                        break;
                                    case 3:
                                        if (!selectedObjects[firstInteractableIndex].GetComponent<ShopInventory>().shopPanel.activeSelf || !selectedObjects[firstInteractableIndex].GetComponent<ShopInventory>().shopPanel.transform.parent.gameObject.activeSelf)
                                            interactPrompt3Text.text = "[" + input3 + "] Open Shop";
                                        else
                                            interactPrompt3Text.text = "[" + input3 + "] Close Shop";
                                        break;
                                }
                        }
                    }
                    else if (interactions.Contains(SelectedObjectType.Shop)) interactions.Remove(SelectedObjectType.Shop);

                    if (selectedObjects[0].GetComponent<SleepObject>())
                    {
                        //isSleep = true;
                        if (!interactions.Contains(SelectedObjectType.Sleep))
                            interactions.Add(SelectedObjectType.Sleep);

                        for (int i = 0; i < interactions.Count; i++)
                        {
                            if (interactions[i] == SelectedObjectType.Sleep)
                                switch (i + 1)
                                {
                                    case 1:
                                        interactPrompt1Text.text = "[" + input1 + "] Sleep in " + selectedObjects[0].name;
                                        break;
                                    case 2:
                                        interactPrompt2Text.text = "[" + input2 + "] Sleep in " + selectedObjects[0].name;
                                        break;
                                    case 3:
                                        interactPrompt3Text.text = "[" + input3 + "] Sleep in " + selectedObjects[0].name;
                                        break;
                                }
                        }
                    }
                    else if (interactions.Contains(SelectedObjectType.Sleep)) interactions.Remove(SelectedObjectType.Sleep);

                    if (selectedObjects[0].GetComponent<EntityController>())
                    {
                        if (!interactions.Contains(SelectedObjectType.Entity))
                            interactions.Add(SelectedObjectType.Entity);


                    }
                    else if (interactions.Contains(SelectedObjectType.Entity)) interactions.Remove(SelectedObjectType.Entity);

                    if (selectedObjects[0].GetComponent<RopeEnd>() || selectedObjects[0].GetComponentInChildren<RopeEnd>())
                    {
                        //isSleep = true;
                        if (!interactions.Contains(SelectedObjectType.Rope))
                            interactions.Add(SelectedObjectType.Rope);

                        for (int i = 0; i < interactions.Count; i++)
                        {
                            if (interactions[i] == SelectedObjectType.Rope)
                                switch (i + 1)
                                {
                                    case 1:
                                        interactPrompt1Text.text = "[LMB] Collect Rope";
                                        break;
                                    case 2:
                                        interactPrompt2Text.text = "[LMB] Collect Rope";
                                        break;
                                    case 3:
                                        interactPrompt3Text.text = "[LMB] Collect Rope";
                                        break;
                                }
                        }
                    }
                    //else if (interactions.Contains(SelectedObjectType.Rope)) interactions.Remove(SelectedObjectType.Rope);


                    else if ( inventorySystem.selectedInventoryItem != null && inventorySystem.selectedInventoryItem.item.itemName.Contains("Rope")) // !selectedObjects[0].GetComponent<RopeEnd>() && !selectedObjects[0].GetComponentInChildren<RopeEnd>() &&
                    {
                        if (interactions.Contains(SelectedObjectType.Rope)) interactions.Remove(SelectedObjectType.Rope);
                        //Debug.Log("Can tie rope here, interaction text num = " + (interactions.Count + 1).ToString());
                        switch (interactions.Count + 1)
                        {
                            case 1:
                                interactPrompt1Text.text = "[LMB] Tie Rope";
                                break;
                            case 2:
                                interactPrompt2Text.text = "[LMB] Tie Rope";
                                break;
                            case 3:
                                interactPrompt3Text.text = "[LMB] Tie Rope";
                                break;
                        }
                    }
                }

                //BELOW IS THE SCRIPT FOR HELD BUTTON INPUTS - TO BE IMPLEMENTED FULLY

                //if (Input.GetKeyDown(input1))
                //    keyDown = true;

                //if (keyDown)
                //{
                //    inputTimer += Time.deltaTime;
                //    if (inputTimer > 1f)
                //    {
                //        //put in here for held inputs

                //        inputTimer = 0f;
                //        keyDown = false;
                //    }
                //}
                //else if (inputTimer > 0)
                //{

                //    inputTimer = 0f;
                //}


                if (Input.GetKeyDown(input1) && interactions.Count > 0)
                {
                    TriggerInteraction(0);

                }
                if (Input.GetKeyDown(input2) && interactions.Count >= 1)
                {
                    TriggerInteraction(1);

                }
                if (Input.GetKeyDown(input3) && interactions.Count >= 2)
                {
                    TriggerInteraction(2);

                }

                if (Input.GetMouseButtonDown(0) && interactions.Contains(SelectedObjectType.Rope))
                {
                    for (int i = 0; i < interactions.Count; i++)
                    {
                        if (interactions[i] == SelectedObjectType.Rope)
                        {
                            TriggerInteraction(i);
                            break;
                        }
                    }
                }

                if (selectedObjects[firstInteractableIndex] != null)
                    selectedObject = selectedObjects[firstInteractableIndex];
                else
                {
                    selectedObjects.RemoveAt(firstInteractableIndex);
                }

            }
            else
            {
                selectedObjects.RemoveAt(0);

                firstInteractableIndex = 0;

                playerController.canClimb = false;

            }
        }
        else
        {
            selectedObject = null;

            playerController.canClimb = false;

        }

        if ((selectedObject == null || selectedObject.CompareTag("Climbable"))) //&& (inventorySystem.selectedInventoryItem == null || !inventorySystem.selectedInventoryItem.item.itemName.Contains("Rope"))
        {
            interactPrompt1Text.text = "";
            interactPrompt2Text.text = "";
            interactPrompt3Text.text = "";

            interactions.Clear();

        }

        if (inventorySystem.selectedInventoryItem == null || !inventorySystem.selectedInventoryItem.item.itemName.Contains("Rope"))
        {
            if (interactions.Count < 3)
                interactPrompt3Text.text = "";
            if (interactions.Count < 2)
                interactPrompt2Text.text = "";
            if (interactions.Count < 1)
                interactPrompt1Text.text = "";
        }
        else if (inventorySystem.selectedInventoryItem != null && inventorySystem.selectedInventoryItem.item.itemName.Contains("Rope"))
        {
            if (interactions.Count == 1)
            {
                interactPrompt3Text.text = "";
            }
            else if (interactions.Count < 1)
            {
                interactPrompt3Text.text = "";
                interactPrompt2Text.text = "";
                interactPrompt1Text.text = "";
            }
        }
    }

    public void TriggerInteraction(int i)
    {
        if (interactions[i] == SelectedObjectType.Plant && selectedObject.GetComponent<ProduceController>() && selectedObject.GetComponent<ProduceController>().stillPlanted)
        {
            isPlantInteracted = true;

            HarvestPlant();
        }
        else if (interactions[i] == SelectedObjectType.Item && selectedObject.GetComponent<ItemInWorld>())
        {
            isItemInteracted = true;

            PickUpItem();
        }

        if (interactions[i] == SelectedObjectType.Emote && selectedObject.GetComponent<EmoteInWorld>())
        {
            isEmoteInteracted = true;


            PickUpEmote();
        }

        if (interactions[i] == SelectedObjectType.Ability && selectedObject.GetComponent<AbilityInWorld>())
        {
            isAbilityInteracted = true;

            PickUpAbility();
        }

        if (interactions[i] == SelectedObjectType.Chest && selectedObject.GetComponent<ChestInventory>())
        {
            isChestInteracted = true;

            if (!selectedObject.GetComponent<ChestInventory>().chestPanel.activeSelf)
                OpenChest();
            else selectedObject.GetComponent<ChestInventory>().CloseChest();
        }

        if (interactions[i] == SelectedObjectType.Car && selectedObject.GetComponent<WheelDrive>())
        {
            isCarInteracted = true;

            if (!selectedObject.GetComponent<WheelDrive>().beingDriven)
                EnterCar();
            else selectedObject.GetComponent<WheelDrive>().ExitCar();
        }

        if (interactions[i] == SelectedObjectType.Chair && selectedObject.GetComponent<Chair>())
        {
            isChairInteracted = true;

            if (!selectedObject.GetComponent<Chair>().isSitting)
                SitDown();
            else selectedObject.GetComponent<Chair>().StandUp(playerController);
        }
        if (interactions[i] == SelectedObjectType.FloatingVehicle && selectedObject.GetComponent<FloatingVehicle>())
        {
            isFloatingVehicleInteracted = true;

            if (!selectedObject.GetComponent<FloatingVehicle>().beingDriven)
                EnterFloatingVehicle();
            else selectedObject.GetComponent<FloatingVehicle>().StandUp(playerController);
        }

        if (interactions[i] == SelectedObjectType.SceneTransition && selectedObject.GetComponent<SceneLoader>())
        {
            isSceneTransitionInteracted = true;

            if (!selectedObject.GetComponent<SceneLoader>().isLoaded)
                selectedObject.GetComponent<SceneLoader>().LoadScene();
            else if (selectedObject.GetComponent<SceneLoader>().rootScene != selectedObject.GetComponent<SceneLoader>().mainScene)
            {
                selectedObject.GetComponent<SceneLoader>().UnloadScene();
            }
        }

        if (interactions[i] == SelectedObjectType.Door && selectedObject.GetComponent<ToggleDoor>())
        {
            isDoorInteracted = true;

            selectedObject.GetComponent<ToggleDoor>().DoorInteraction();
        }

        if (interactions[i] == SelectedObjectType.Shop && selectedObject.GetComponent<ShopInventory>())
        {
            isShopInteracted = true;

            if ((!selectedObject.GetComponent<ShopInventory>().shopPanel.activeSelf || !selectedObject.GetComponent<ShopInventory>().shopPanel.transform.parent.gameObject.activeSelf) && !selectedObject.GetComponent<ShopInventory>().buyBackPanelOpen)
                OpenShop();
            else selectedObject.GetComponent<ShopInventory>().CloseShop();
        }

        if (interactions[i] == SelectedObjectType.Sleep && selectedObject.GetComponent<SleepObject>())
        {
            isSleepInteracted = true;

            selectedObject.GetComponent<SleepObject>().sleep.ToggleSleepPanel();
        }



        if (interactions[i] == SelectedObjectType.Rope && (selectedObject.GetComponent<RopeEnd>() || selectedObject.GetComponentInChildren<RopeEnd>()))
        {

            if (selectedObject.GetComponent<RopeEnd>())
            {

                selectedObject.GetComponent<RopeEnd>().ropeItem.CollectRopeEnd(selectedObject.GetComponent<RopeEnd>());
                isRopeInteracted = true;

            }
            else if (selectedObject.GetComponentInChildren<RopeEnd>())
            {


                selectedObject.GetComponentInChildren<RopeEnd>().ropeItem.CollectRopeEnd(selectedObject.GetComponentInChildren<RopeEnd>());
                isRopeInteracted = true;


            }
        }

        if (interactions[i] == SelectedObjectType.Entity && selectedObject.GetComponent<EntityController>())
        {

            //Add entity interaction logic here

            if (selectedObject.GetComponent<EntityController>().playerCanRide)
            {
                //If large animal add ride input
            }

            //if ()
            //Check held inventory item
            // - if none, add pet input
            // - if item is held, check whether it has an entity-specific interaction and add it's input; i.e Feed animal


            isEntityInteracted = true;

        }

        StartCoroutine(DelaySettingFalseVariables());
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.GetComponent<Interactable>() ||
            (other.transform.CompareTag("Climbable") && !other.GetComponent<Collider>().isTrigger))
        {
            selectedObjects.Add(other.transform.gameObject);

            //interactPromptText.text = "Collect " + selectedObjects[selectedObjects.Count].GetComponent<ItemInWorld>().item.itemName;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<Interactable>() ||
            other.transform.CompareTag("Climbable"))
        {
            if (other.gameObject == selectedObject) selectedObject = null;

            selectedObjects.Remove(other.transform.gameObject);

            //interactPromptText.text = "Collect " + selectedObjects[selectedObjects.Count].GetComponent<ItemInWorld>().item.itemName;
        }

    }

    public void PickUpItem()
    {
        InventoryItem item = selectedObject.GetComponent<ItemInWorld>().item;

        if (selectedObject.GetComponent<PlacedObject>())
        {
            GridBuildingSystem gBS = FindObjectOfType<GridBuildingSystem>();
            gBS.grid.GetGridObject(gBS.x, gBS.z).ClearPlacedObject();
        }
        ProduceController produceController = null;

        if (selectedObject.GetComponent<ProduceController>()) produceController = selectedObject.GetComponent<ProduceController>();

        inventorySystem.AddItemToInventory(item, inventorySystem.inventory, inventorySystem.inventorySlots, selectedObject, produceController);
    }

    public void HarvestPlant()
    {
        ProduceController produce = selectedObject.GetComponent<ProduceController>();

        produce.Harvest();
    }
    public void PickUpEmote()
    {
        Debug.Log("Collecting emote");
        Emote emote = selectedObject.GetComponent<EmoteInWorld>().emote;

        emoteManager.AddEmoteToInventory(emote, selectedObject);

    }
    public void PickUpAbility()
    {
        Debug.Log("Collecting ability");
        Ability ability = selectedObject.GetComponent<AbilityInWorld>().ability;

        playerAbilities.AddAbilityToInventory(ability, selectedObject);

    }

    public void OpenChest()
    {
        Debug.Log("Opening Chest");
        ChestInventory chest = selectedObject.GetComponent<ChestInventory>();
        chest.InitialiseChestPanel();

    }
    public void OpenShop()
    {
        Debug.Log("Opening Shop");
        ShopInventory shop = selectedObject.GetComponent<ShopInventory>();
        shop.InitialiseShopPanel();

    }

    public void EnterCar()
    {
        Debug.Log("Entering Vehicle");
        selectedObject.GetComponent<WheelDrive>().EnterCar();

    }
    public void SitDown()
    {
        Debug.Log("Entering Vehicle");
        selectedObject.GetComponent<Chair>().SitDown(playerController);


    }

    public void EnterFloatingVehicle()
    {
        Debug.Log("Entering Vehicle");
        selectedObject.GetComponent<FloatingVehicle>().SitDown(playerController);


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

            isChairInteracted = false;
        }

        if (isFloatingVehicleInteracted)
        {
            yield return new WaitForSeconds(delayTime);

            isFloatingVehicleInteracted = false;
        }

        if (isSceneTransitionInteracted)
        {
            yield return new WaitForSeconds(delayTime);

            isSceneTransitionInteracted = false;
        }

        if (isDoorInteracted)
        {
            yield return new WaitForSeconds(delayTime);

            isDoorInteracted = false;
        }

        if (isShopInteracted)
        {
            yield return new WaitForSeconds(delayTime);

            isShopInteracted = false;
        }
        if (isSleepInteracted)
        {
            yield return new WaitForSeconds(delayTime);

            isSleepInteracted = false;
        }
        if (isRopeInteracted)
        {
            yield return new WaitForSeconds(delayTime);

            isRopeInteracted = false;
        } 
        if (isEntityInteracted)
        {
            yield return new WaitForSeconds(delayTime);

            isEntityInteracted = false;
        }
    }
}
