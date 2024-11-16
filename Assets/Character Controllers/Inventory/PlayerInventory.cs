using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlayerInventory : MonoBehaviour
{
    [Header("Item Selection")]
    public InventoryUIItem selectedInventoryItem;
    public GameObject selectedPhysicalItem;
    public GameObject offHandItem;
    public int selectedItemSlot = 1;
    public Transform rightHandPos;
    public Transform leftHandPos;
    [SerializeField] private Color selectedColour;
    private Color originalColour;
    public PlayerInteractionRaycast firstPersonRaycast;
    public ThirdPersonSelection thirdPersonRaycast;
    private SetCamera setCamera;

    [Header("UI Elements")]
    public GameObject inventoryUI;
    public GameObject inventoryItemPrefab;
    public GameObject inventorySlotPrefab;
    public GameObject inventoryBarPanel;
    public int inventorySlotNum = 12;
    [SerializeField] private GameObject inventoryWindowPanel;
    [SerializeField] private GameObject inventoryEquipmentPanel;
    public bool inventoryWindowOpen;
    [SerializeField] private TextMeshProUGUI rightHandTextPrompts;
    [SerializeField] private TextMeshProUGUI leftHandTextPrompts;
    [SerializeField] private TextPopUp textPopUp;

    public GameObject chestPanel;
    public ChestInventory currentChest;
    public GameObject shopPanel;
    public GameObject shopParent;
    public GameObject buyBackPanel;
    public ShopInventory currentShop;

    //public GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI selectedItemNameText;
    [SerializeField] private TextMeshProUGUI selectedItemDescText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private TextMeshProUGUI valueText;

    public ItemSlot[] inventorySlots;
    public EquipmentSlot[] equipmentSlots;

    private EmoteManager emoteManager;
    private PlayerAbilities playerAbilities;

    [Header("Inputs")]
    [SerializeField] private KeyCode inventoryInput = KeyCode.I;
    [SerializeField] private KeyCode rightDropItemInput = KeyCode.Q;
    [SerializeField] private KeyCode leftDropItemInput = KeyCode.X;
    [SerializeField] private KeyCode rightThrowItemInput = KeyCode.T;
    [SerializeField] private KeyCode lefttThrowItemInput = KeyCode.G;
    //[SerializeField] private KeyCode useItemInput = KeyCode.C;
    //Item use input in the "UseItem" script

    [Header("Inventory Contents")]
    public float inventoryWeight;
    public float inventoryValue;
    public float money;

    public List<InventoryUIItem> inventory;
    public List<ProduceInInventory> produceItems;
    [SerializeField]private PlayerController player;
    [HideInInspector] public ThirdPersonCam cam;


    [Header("Holding Items")]
    public TwoBoneIKConstraint leftArm;
    public TwoBoneIKConstraint rightArm;

    public Transform leftTarget;
    public Transform rightTarget; 
    
    public Transform leftWeaponTarget;
    public Transform rightWeaponTarget;

    public EquipmentSlot offHandSlot;
    public InventoryUIItem offHandPaused;
    public Transform swordTarget;
    public Transform projectileTarget;

    [Range(0f, 1f)] public float holdingItemRigWeight = 0.6f;

    public float throwCooldown;
    public float throwForce;
    public float throwUpwardForce;
    public bool canThrow;

    private Quaternion lastParentRotation;

    private void Awake()
    {
        //playerInteractionRaycast = FindObjectOfType<PlayerInteractionRaycast>();
        setCamera = FindObjectOfType<SetCamera>();
        player = FindObjectOfType<PlayerController>();
        cam = FindObjectOfType<ThirdPersonCam>();

        emoteManager = FindObjectOfType<EmoteManager>();
        playerAbilities = FindObjectOfType<PlayerAbilities>();

        inventorySlots = new ItemSlot[inventoryBarPanel.transform.childCount + inventorySlotNum];
        equipmentSlots = new EquipmentSlot[inventoryEquipmentPanel.transform.childCount];

        lastParentRotation = rightHandPos.localRotation;

        foreach (Transform child in inventoryWindowPanel.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventory.Add(null);
            produceItems.Add(null);
        }

        for (int i = 0; i < inventoryBarPanel.transform.childCount + inventorySlotNum; i++)
        {
            if (i < inventoryBarPanel.transform.childCount)
            {
                inventorySlots[i] = inventoryBarPanel.transform.GetChild(i).GetComponent<ItemSlot>();
                inventorySlots[i].slot = i;

            }
            else
            {
                //inventorySlots[i] = inventoryWindowPanel.transform.GetChild(i - inventoryBarPanel.transform.childCount).GetComponent<ItemSlot>();
                inventorySlots[i] = Instantiate(inventorySlotPrefab, inventoryWindowPanel.transform).GetComponent<ItemSlot>();
                inventorySlots[i].slot = i;
            }
        }
        originalColour = inventorySlots[0].GetComponent<Image>().color;
        for (int i = 0; i < inventoryEquipmentPanel.transform.childCount; i++)
        {

            equipmentSlots[i] = inventoryEquipmentPanel.transform.GetChild(i).GetComponent<EquipmentSlot>();
            equipmentSlots[i].slot = i;


        }
        GetInventoryWeight();
        //GetInventoryValue();

        //infoPanel.SetActive(false);

        rightArm.weight = 0f;
        leftArm.weight = 0f;
    }

    private void Update()
    {
        valueText.text = money.ToString("$" + "0.00");

        if (inventoryUI.activeSelf)
        {
            SelectInventoryItemWithNumbers();
            SelectInventoryItemWithScroll();
        }

        if (inventory[selectedItemSlot] != null && selectedInventoryItem == null)
        {
            selectedInventoryItem = inventory[selectedItemSlot];
            //EndItemInspection();
            HoldItemMainHand();
        }

        if (offHandSlot.inventoryItem == null && offHandItem != null)
            EndOffHandInspection();
        else if (offHandSlot.inventoryItem != null && offHandSlot.inventoryItem.physicalItem == null && offHandItem != null)
            offHandSlot.inventoryItem.physicalItem = offHandItem;

        if (!player.isEmoting)
        {
            if (selectedInventoryItem != null)
            {
                rightArm.weight = selectedInventoryItem.item.heldRigWeight;

                if (player.isUsingBoth)
                {
                    leftArm.weight = rightArm.weight;

                    if (selectedInventoryItem.physicalItem.GetComponent<WeaponItem>() )
                    {
                        if (selectedInventoryItem.physicalItem.GetComponent<WeaponItem>().isProjectile)
                        rightTarget.position = rightWeaponTarget.position;

                        leftTarget.position = selectedInventoryItem.physicalItem.GetComponent<WeaponItem>().leftHandPos.position;
                    }
                }
            }
            else
                rightArm.weight = 0f;

            if (offHandSlot.inventoryItem != null)
            {
                leftArm.weight = offHandSlot.inventoryItem.item.heldRigWeight;
            }
            else if (!player.isUsingBoth)
            {
                leftArm.weight = 0f;
            }
        }
        else
        {
            rightArm.weight = 0f;
            leftArm.weight = 0f;
        }

        if (!player.isUsingBoth)
        {
           

            if (!player.isUsingRight)
            {
                rightHandPos.GetComponent<Collider>().enabled = false;
            }

            if (!player.isUsingLeft)
            {
                leftHandPos.GetComponent<Collider>().enabled = false;
            }
        }
        //else
        //    if (offHandItem != null)
        //    PauseOffHandInspection();

        if (inventoryBarPanel.activeSelf)
        {
            if (Input.GetKeyDown(inventoryInput))
            {

                OpenInventoryBar();
            }

            if (selectedInventoryItem != null)
            {
                if (selectedInventoryItem.item.prefab.GetComponent<WeaponItem>() && selectedInventoryItem.item.prefab.GetComponent<WeaponItem>().isTwoHanded)
                {
                    if (offHandItem != null)
                        PauseOffHandInspection();
                }

                SetSelectedItemColour();
                HoldItemMainHand();

                if (selectedPhysicalItem.GetComponent<WeaponItem>() && selectedPhysicalItem.GetComponent<WeaponItem>().isProjectile)
                {
                    //selectedPhysicalItem.transform.eulerAngles = player.orientation.forward ;
                    selectedPhysicalItem.transform.LookAt(projectileTarget);
                }

                if (!inventoryWindowOpen)
                {
                    rightHandTextPrompts.gameObject.SetActive(true);
                    if (selectedInventoryItem.item.canConsume)
                    {
                        rightHandTextPrompts.text = "Drop [" + rightDropItemInput.ToString() + "]" + "\nConsume [LMB] \nThrow [" + rightThrowItemInput + "]"/*+ selectedInventoryItem.item.itemName*/;

                        if (Input.GetButtonDown("Fire1"))
                        {
                            ConsumeFood(true);
                            player.animator.SetBool("isUsingRight", true);
                            player.animator.SetInteger("RightIndex", -2);
                        }
                    }
                    else rightHandTextPrompts.text = "Drop [" + rightDropItemInput.ToString() + "] \nThrow [" + rightThrowItemInput + "]" /*+ selectedInventoryItem.item.itemName*/;

                    if (Input.GetKeyDown(rightDropItemInput))
                    {

                        DropItem(selectedInventoryItem, true);

                    }

                    if (Input.GetKeyDown(rightThrowItemInput) && canThrow)
                    {
                        ThrowItem(selectedInventoryItem, true);
                        player.animator.SetBool("isUsingRight", true);
                        player.animator.SetInteger("RightIndex", -1);
                    }
                }
            }
            else
            {
                rightHandTextPrompts.text = "";
                rightHandTextPrompts.gameObject.SetActive(false);


                if (selectedPhysicalItem != null)
                {
                    EndItemInspection();
                }
                selectedInventoryItem = null;
                SetSelectedItemColour();

            }
        }
        else
        {
            rightHandTextPrompts.text = "";
            rightHandTextPrompts.gameObject.SetActive(false);

            if (selectedPhysicalItem != null)
            {
                EndItemInspection();
            }
            selectedInventoryItem = null;
            SetSelectedItemColour();
        }

        if (offHandPaused != null && (selectedInventoryItem == null || !selectedInventoryItem.physicalItem.GetComponent<WeaponItem>() || !selectedInventoryItem.physicalItem.GetComponent<WeaponItem>().isTwoHanded))
        {
            HoldItemOffHand(offHandPaused);
            offHandPaused = null;
        }

        if (offHandSlot.inventoryItem != null)
        {
            //SetSelectedItemColour();
            //HoldItemMainHand();
            leftHandTextPrompts.gameObject.SetActive(true);

            if (!inventoryWindowOpen)
            {
                if (offHandSlot.inventoryItem.item.canConsume)
                {
                    leftHandTextPrompts.text = "Drop [" + rightDropItemInput.ToString() + "]" + "\nConsume [RMB] \nThrow [" + lefttThrowItemInput + "]";/*+ selectedInventoryItem.item.itemName*/;

                    if (Input.GetButtonDown("Fire2"))
                    {
                        ConsumeFood(false);
                        player.animator.SetBool("isUsingLeft", true);
                        player.animator.SetInteger("LeftIndex", -2);
                    }
                }
                else leftHandTextPrompts.text = "Drop [" + leftDropItemInput.ToString() + "] \nThrow [" + lefttThrowItemInput + "]" /*+ selectedInventoryItem.item.itemName*/;

                if (Input.GetKeyDown(leftDropItemInput))
                {

                    DropItem(offHandSlot.inventoryItem, false);

                }

                if (Input.GetKeyDown(lefttThrowItemInput) && canThrow)
                {
                    ThrowItem(offHandSlot.inventoryItem, false);
                    player.animator.SetBool("isUsingLeft", true);
                    player.animator.SetInteger("LeftIndex", -1);
                }
            }
        }
        else
        {
            leftHandTextPrompts.text = "";
            leftHandTextPrompts.gameObject.SetActive(false);


            if (offHandItem != null)
            {
                EndItemInspection();
            }
            offHandItem = null;
            //SetSelectedItemColour();

        }

        if (inventoryWindowOpen && !inventoryWindowPanel.activeSelf)
        {
            OpenInventoryWindow();
        }
        else if (!inventoryWindowOpen && inventoryWindowPanel.activeSelf)
        {
            CloseInventoryWindow();
        }

        //if (infoPanel.activeSelf)
        //{
        //    if ((selectedInventoryItem != null && selectedInventoryItem.item.itemName != selectedItemNameText.text) || (selectedInventoryItem == null && selectedItemNameText.text != null))
        //    {
        //        SetSelectedItemInfoUI();
        //        Debug.Log("Item info set");
        //    }
        //}
    }

    public void OpenInventoryBar()
    {
        if (emoteManager.emoteUI.activeSelf || playerAbilities.abilityUI.activeSelf)
        {
            emoteManager.emoteUI.SetActive(false);
            playerAbilities.abilityUI.SetActive(false);

            inventoryUI.SetActive(true);
        }
        else
        {
            inventoryWindowOpen = !inventoryWindowOpen;
        }
    }

    public void OpenInventoryWindow()
    {
        inventoryWindowPanel.SetActive(true);
        inventoryEquipmentPanel.SetActive(true);

        //cam.freezeCameraRotation = true;
        //player.GetComponent<PlayerAttack>().enabled = false;
            player.GetComponent<PlayerAttack>().enabled = false;
        Pause.instance.freezeCameraRotation = true;
        Pause.instance.unlockCursor = true;


        //infoPanel.SetActive(true);


        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;
    }

    public void CloseInventoryWindow()
    {
        inventoryWindowPanel.SetActive(false);
        inventoryEquipmentPanel.SetActive(false);
        //cam.freezeCameraRotation = false;
        //player.GetComponent<PlayerAttack>().enabled = true;
        Pause.instance.freezeCameraRotation = false;
        Pause.instance.unlockCursor = false;

        //infoPanel.SetActive(false);
        //SetSelectedItemInfoUI();


        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    public void SetSelectedItemInfoUI()
    {

        if (selectedInventoryItem != null)
        {
            selectedItemNameText.text = selectedInventoryItem.item.itemName;
            selectedItemDescText.text = selectedInventoryItem.item.itemDescription;

            statsText.text = null;

            if (selectedInventoryItem.healthEffect != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Health Effect: " + selectedInventoryItem.item.healthEffect;
            }

            if (selectedInventoryItem.staminaEffect != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Stamina Effect: " + selectedInventoryItem.item.staminaEffect;
            }

            if (selectedInventoryItem.healthModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Health: " + selectedInventoryItem.item.healthModifier;
            }

            if (selectedInventoryItem.staminaModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Stamina: " + selectedInventoryItem.item.staminaModifier;
            }

            if (selectedInventoryItem.oxygenModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Oxygen: " + selectedInventoryItem.item.oxygenModifier;
            }

            if (selectedInventoryItem.speedModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Speed: " + selectedInventoryItem.item.speedModifier;
            }

            if (selectedInventoryItem.jumpModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Jump: " + selectedInventoryItem.item.jumpModifier;
            }

            if (selectedInventoryItem.armourModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Armour: " + selectedInventoryItem.item.armourModifier;
            }

            if (selectedInventoryItem.attackDamageModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Attack Damage: " + selectedInventoryItem.item.attackDamageModifier;
            }  
            
            if (selectedInventoryItem.passiveDamageModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Passive Damage: " + selectedInventoryItem.item.passiveDamageModifier;
            }

            if (selectedInventoryItem.attackSpeedModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Attack Speed: " + selectedInventoryItem.item.attackSpeedModifier;
            }

            if (selectedInventoryItem.knockBackModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Knockback: " + selectedInventoryItem.item.knockBackModifier;
            }
        }
        else
        {
            selectedItemNameText.text = null;
            selectedItemDescText.text = null;

            statsText.text = null;
        }
    }

    public void AddItemToInventory(InventoryItem item, List<InventoryUIItem> inventory, ItemSlot[] inventorySlots, GameObject itemInWorld = null, ProduceController produceController = null)
    {
        Debug.Log("Adding " + item.itemName + " to inventory");
        if (player.stats.weight + item.weight <= player.stats.maxWeight.GetValue())
        {
            if (itemInWorld != null)
                ParkStats.instance.StopTrackingObject(itemInWorld);
            //if item is already in inventory increase num carried (in 'InventoryItem' scriptable object)
            if (item.isStackable)
            {
                Debug.Log(item.itemName + " is stackable");
                List<InventoryUIItem> slotsWithItem = new List<InventoryUIItem>();

                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i] != null && inventory[i].item == item)
                    {
                        slotsWithItem.Add(inventory[i]);
                    }
                }

                if (slotsWithItem.Count > 0)
                {
                    for (int i = 0; i < slotsWithItem.Count; i++)
                    {
                        if (slotsWithItem[i].numCarried < item.maxNumCarried)
                        {

                            slotsWithItem[i].numCarried += 1;
                            slotsWithItem[i].stackCountText.text = slotsWithItem[i].numCarried.ToString();

                            if (slotsWithItem[i].item.canSpoil) // if the item can spoil
                            {
                                Debug.Log(slotsWithItem[i].item.itemName + " is produce - initialising item in stack");

                                ProduceInInventory produce = null;

                                if (!slotsWithItem[i].GetComponent<ProduceInInventory>())
                                {
                                    produce = slotsWithItem[i].AddComponent<ProduceInInventory>();

                                    //produce.produceItem = slotsWithItem[i];

                                    if (itemInWorld != null && itemInWorld.GetComponent<ProduceController>())
                                    {
                                        ProduceController currentProduce = itemInWorld.GetComponent<ProduceController>();
                                        //produce.InitaliseInventoryProduce(slotsWithItem[i], currentProduce);

                                        produce.produceAgesInStack.Add(currentProduce.age);
                                    }
                                    else if (slotsWithItem[i].physicalItem != null && slotsWithItem[i].physicalItem.GetComponent<ProduceController>() != null)
                                    {
                                        ProduceController currentProduce = slotsWithItem[i].physicalItem.GetComponent<ProduceController>();
                                        //produce.InitaliseInventoryProduce(slotsWithItem[i], currentProduce);
                                        produce.produceAgesInStack.Add(currentProduce.age);

                                    }
                                    else if (inventory[i].item.prefab != null && inventory[i].item.prefab.GetComponent<ProduceController>() != null)
                                    {
                                        ProduceController currentProduce = inventory[i].item.prefab.GetComponent<ProduceController>();
                                        //currentProduce.age = currentProduce.ageOfMaturity;

                                        produce.produceAgesInStack.Add(currentProduce.ageOfMaturity);

                                    }
                                }
                                else
                                {
                                    produce = slotsWithItem[i].GetComponent<ProduceInInventory>();

                                    if (produceController == null)
                                    {
                                        if (itemInWorld != null && itemInWorld.GetComponent<ProduceController>())
                                        {
                                            ProduceController currentProduce = itemInWorld.GetComponent<ProduceController>();
                                            produce.produceAgesInStack.Add(currentProduce.age);

                                        }
                                        else if (slotsWithItem[i].physicalItem != null && slotsWithItem[i].physicalItem.GetComponent<ProduceController>() != null)
                                        {
                                            ProduceController currentProduce = slotsWithItem[i].physicalItem.GetComponent<ProduceController>();
                                            produce.produceAgesInStack.Add(currentProduce.age);

                                        }
                                        else if (item.prefab != null && item.prefab.GetComponent<ProduceController>() != null)
                                        {
                                            ProduceController currentProduce = item.prefab.GetComponent<ProduceController>();
                                            //currentProduce.age = currentProduce.ageOfMaturity;

                                            produce.produceAgesInStack.Add(currentProduce.ageOfMaturity);

                                        }
                                    }
                                    else
                                    {
                                        produce.produceAgesInStack.Add(produceController.age);

                                    }

                                    //produce.InitaliseInventoryProduce(produce, );

                                }
                            }
                            else Debug.Log(slotsWithItem[i].item.itemName + " isn't produce");


                            if (itemInWorld != null && itemInWorld.GetComponent<ItemInWorld>())
                            {
                                Destroy(itemInWorld);

                                if (setCamera.isFirstPerson)
                                    firstPersonRaycast.selectedObject = null;
                                else thirdPersonRaycast.selectedObject = null;
                                //playerInteractionRaycast.interactPromptIndicator.SetActive(false);
                            }
                            GetInventoryWeight();
                            // GetInventoryValue();
                            break;
                        }
                        else if (i == slotsWithItem.Count - 1)// if the capacity is full check if it can take a new slot
                        {
                            if (!slotsWithItem[i].item.canSpoil) // if the item cant spoil
                            {
                                AddItemToNewInventorySlot(item, inventory, inventorySlots, itemInWorld);
                                break;
                            }
                            else
                            {
                                if (itemInWorld != null && itemInWorld.GetComponent<ProduceController>())
                                {
                                    ProduceController currentProduce = itemInWorld.GetComponent<ProduceController>();
                                    AddItemToNewInventorySlot(item, inventory, inventorySlots, itemInWorld, 1, currentProduce, null, currentProduce.age);

                                    break;

                                }
                                else if (slotsWithItem[i].physicalItem != null && slotsWithItem[i].physicalItem.GetComponent<ProduceController>() != null)
                                {
                                    ProduceController currentProduce = slotsWithItem[i].physicalItem.GetComponent<ProduceController>();
                                    AddItemToNewInventorySlot(item, inventory, inventorySlots, itemInWorld, 1, currentProduce, null, currentProduce.age);
                                    break;
                                }
                                else if (slotsWithItem[i].item.prefab != null && slotsWithItem[i].item.prefab.GetComponent<ProduceController>() != null)
                                {
                                    ProduceController currentProduce = slotsWithItem[i].item.prefab.GetComponent<ProduceController>();

                                    AddItemToNewInventorySlot(item, inventory, inventorySlots, itemInWorld, 1, currentProduce, null, currentProduce.ageOfMaturity);

                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Debug.Log(item.itemName + " isn't produce"); 
                    AddItemToNewInventorySlot(item, inventory, inventorySlots, itemInWorld, 1, produceController);
                }
            }
            else // otherwise add a new item to the inventory
            {
                Debug.Log(item.itemName + " isn't stackable");

                // AddItemToNewInventorySlot(item, inventory, inventorySlots, itemInWorld);

                if (!item.canSpoil) // if the item can spoil
                {
                    AddItemToNewInventorySlot(item, inventory, inventorySlots, itemInWorld);
                }
                else
                {
                    if (itemInWorld != null && itemInWorld.GetComponent<ProduceController>())
                    {
                        ProduceController currentProduce = itemInWorld.GetComponent<ProduceController>();
                        AddItemToNewInventorySlot(item, inventory, inventorySlots, itemInWorld, 1, currentProduce, null, currentProduce.age);


                    }
                    else if (item.prefab != null && item.prefab.GetComponent<ProduceController>() != null)
                    {
                        ProduceController currentProduce = item.prefab.GetComponent<ProduceController>();

                        AddItemToNewInventorySlot(item, inventory, inventorySlots, itemInWorld, 1, currentProduce, null, currentProduce.ageOfMaturity);
                    }

                }
            }
        }
        else
        {
            //display a pop up if the player can't pick up an item
            textPopUp.SetAndDisplayPopUp("Weight Limit Reached");
        }

    }

    public void SplitStack(InventoryUIItem item, int numToTake, List<InventoryUIItem> inventory, ItemSlot[] inventorySlots, ProduceInInventory produceInInventory = null)
    {
        if (item.numCarried >= numToTake)
        {
            int next = GetFirstEmptySlot(inventorySlots);


            //
            for (int i = 0; i < numToTake; i++)
            {
                if (produceInInventory != null)
                {
                    if (i == 0)
                    {
                        AddItemToNewInventorySlot(item.item, inventory, inventorySlots, null, numToTake, null, produceInInventory, item.GetComponent<ProduceInInventory>().produceAgesInStack[i]);

                    }

                    ProduceInInventory nextProduce = inventory[next].GetComponent<ProduceInInventory>();
                    nextProduce.InitaliseInventoryProduce(inventory[next], nextProduce, i);
                    Debug.Log("Initialising produce for split");

                    UpdateStackText(inventory[next]);

                    //RemoveItemFromInventory(item, inventory, numToTake, produceInInventory);
                    item.GetComponent<ProduceInInventory>().produceAgesInStack.RemoveAt(i);
                   
                }
                else
                {
                    if (i == 0)
                    {
                        AddItemToNewInventorySlot(item.item, inventory, inventorySlots, null, numToTake);
                        UpdateStackText(inventory[next]);
                        break;
                    }
                }
                item.numCarried--;

                UpdateStackText(item);
            }

        }
        else
        {
            AddItemToNewInventorySlot(item.item, inventory, inventorySlots, null, item.numCarried, null, produceInInventory);
            RemoveItemFromInventory(item, inventory, item.numCarried);
        }

        UpdateStackText(item);
    }
     public void CombineStack(InventoryUIItem staticItem, InventoryUIItem movedItem, List<InventoryUIItem> inventory, ItemSlot[] inventorySlots)
    {
        if (staticItem.item == movedItem.item && staticItem != movedItem)
        {
            if (staticItem.numCarried + movedItem.numCarried <= staticItem.item.maxNumCarried)
            {

                if (staticItem.GetComponent<ProduceInInventory>() != null)
                {
                    ProduceInInventory produceInInventory = staticItem.GetComponent<ProduceInInventory>();
                    for (int i = 0; i < movedItem.GetComponent<ProduceInInventory>().produceAgesInStack.Count; i++)
                    {
                        if (produceInInventory.AssessRipeness(movedItem.GetComponent<ProduceInInventory>().produceAgesInStack[i]) == produceInInventory.stackQuality)
                        {
                            staticItem.GetComponent<ProduceInInventory>().produceAgesInStack.Add(movedItem.GetComponent<ProduceInInventory>().produceAgesInStack[i]);
                            movedItem.GetComponent<ProduceInInventory>().produceAgesInStack.RemoveAt(i);

                            staticItem.numCarried += 1;


                            if (movedItem.numCarried - 1 >= 0)
                            {
                                RemoveItemFromInventory(movedItem, inventory);
                                //break;
                            }
                            else
                                movedItem.numCarried -= 1;


                        }
                        else
                        {
                            Debug.Log("Can't combine produce stacks of different qualities");
                            textPopUp.SetAndDisplayPopUp("Can't combine produce stacks of different qualities");
                            break;
                        }
                    }
                }
                else
                {
                    staticItem.numCarried += movedItem.numCarried;
                    movedItem.numCarried = 0;

                    RemoveItemFromInventory(movedItem, inventory);
                }

            }
            else
            {
                int extra = (staticItem.numCarried + movedItem.numCarried) - staticItem.item.maxNumCarried;
                Debug.Log("Extra items = " + extra);

                if (staticItem.GetComponent<ProduceInInventory>() != null)
                {
                    ProduceInInventory produceInInventory = staticItem.GetComponent<ProduceInInventory>();
                    for (int i = 0; i < staticItem.item.maxNumCarried - staticItem.numCarried; i++)
                    {
                        if (produceInInventory.AssessRipeness(movedItem.GetComponent<ProduceInInventory>().produceAgesInStack[i]) == produceInInventory.stackQuality)
                        {
                            staticItem.GetComponent<ProduceInInventory>().produceAgesInStack.Add(movedItem.GetComponent<ProduceInInventory>().produceAgesInStack[i]);
                            //RemoveItemFromInventory(movedItem, inventory, );
                            movedItem.GetComponent<ProduceInInventory>().produceAgesInStack.RemoveAt(i);

                            staticItem.numCarried += 1;

                            if (movedItem.numCarried - 1 >= 0)
                            {
                                RemoveItemFromInventory(movedItem, inventory);
                                //break;
                            }
                            else
                                movedItem.numCarried -= 1;

                        }
                        else
                        {
                            Debug.Log("Can't combine produce stacks of different qualities");
                            textPopUp.SetAndDisplayPopUp("Can't combine produce stacks of different qualities");
                            break;
                        }
                    }
                }
                else
                {
                    //movedItem.numCarried -= staticItem.item.maxNumCarried - staticItem.numCarried;

                    RemoveItemFromInventory(movedItem, inventory, staticItem.item.maxNumCarried - staticItem.numCarried);

                    staticItem.numCarried = staticItem.item.maxNumCarried;

                }
            }
        }
        UpdateStackText(staticItem);

        if (movedItem != null) UpdateStackText(movedItem);


    }

    public void CombineAllStacksOfType(InventoryUIItem staticItem, List<InventoryUIItem> inventory, ItemSlot[] inventorySlots)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null && inventory[i].item == staticItem.item && inventory[i] != staticItem)
            {
                InventoryUIItem otherStack = inventory[i];
                CombineStack(staticItem, otherStack, inventory, inventorySlots);
            }
        }
    }

    public int CountStacksOfType(InventoryUIItem staticItem, List<InventoryUIItem> inventory)
    {
        int count = 0;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null && inventory[i].item == staticItem.item)
            {
                count += 1;
            }
        }

        return count;
    }
    public int CheckEmptySlots(ItemSlot[] slots)
    {
        int emptySlots = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].transform.childCount == 0)
            {
                emptySlots++;
            }
        }

        return emptySlots;
    }

    public int GetFirstEmptySlot(ItemSlot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].transform.childCount == 0) // for the first empty slot
            {
                return i;
            }
        }
        return -1;
    }

    public void UpdateStackText(InventoryUIItem invItem)
    {
        if (invItem.item.isStackable && invItem.numCarried > 1)
        {
            invItem.stackCountText.text = invItem.numCarried.ToString();
        }
        else invItem.stackCountText.text = null;
    }

    public void AddItemToNewInventorySlot(InventoryItem item, List<InventoryUIItem> inventory, ItemSlot[] inventorySlots, GameObject itemInWorld = null, int stackCount = 1, ProduceController produceController = null, ProduceInInventory produceInInventory = null, float produceAge = 0f)
    {
        Debug.Log("adding new item");
        if (CheckEmptySlots(inventorySlots) > 0)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (GetFirstEmptySlot(inventorySlots) == i) // for the first empty slot
                {
                    if (setCamera.isFirstPerson)
                    {
                        if (firstPersonRaycast.selectedObject.GetComponent<InventoryUIItem>()) // check whether it has used variables
                        {
                            SpawnUsedItem(firstPersonRaycast.selectedObject.GetComponent<InventoryUIItem>(), i, inventory, inventorySlots); // add a new inventory item with old variables

                            firstPersonRaycast.selectedObject = null;

                        }
                        else
                        {
                            SpawnNewItem(item, i, inventory, inventorySlots);

                        }

                        inventory[i].numCarried = stackCount;


                        UpdateStackText(inventory[i]);

                        if (itemInWorld != null && itemInWorld.GetComponent<ItemInWorld>())
                        {
                            Destroy(itemInWorld);
                            firstPersonRaycast.selectedObject = null;
                        }
                    }
                    else if (thirdPersonRaycast.selectedObject.GetComponent<InventoryUIItem>()) // check whether it has used variables
                    {
                        SpawnUsedItem(thirdPersonRaycast.selectedObject.GetComponent<InventoryUIItem>(), i, inventory, inventorySlots); // add a new inventory item with old variables

                        thirdPersonRaycast.selectedObject = null;

                    }
                    else
                    {
                        SpawnNewItem(item, i, inventory, inventorySlots);
                    }
                    inventory[i].numCarried = stackCount;

                    UpdateStackText(inventory[i]);

                    if (itemInWorld != null && itemInWorld.GetComponent<ItemInWorld>())
                    {
                        Destroy(itemInWorld);
                        thirdPersonRaycast.selectedObject = null;
                    }

                    if (selectedItemSlot == i)
                    {
                        // add exception for rope to maintain the line renderer component
                        selectedInventoryItem = inventory[selectedItemSlot];
                        EndItemInspection();
                        HoldItemMainHand();

                    }

                    if (inventory[i].item.canSpoil) // if the item can spoil
                    {
                        Debug.Log(inventory[i].item.itemName + " is produce - initialising new item");
                        ProduceInInventory produce = inventory[i].AddComponent<ProduceInInventory>();

                        if (produceInInventory == null) // if it isn't coming from another stack
                        {
                            if (produceController == null)
                            {
                                if (itemInWorld != null && itemInWorld.GetComponent<ProduceController>())
                                {
                                    ProduceController currentProduce = itemInWorld.GetComponent<ProduceController>();
                                    produce.InitaliseInventoryProduce(inventory[i], currentProduce);

                                }
                                else if (inventory[i].physicalItem != null && inventory[i].physicalItem.GetComponent<ProduceController>() != null)
                                {
                                    ProduceController currentProduce = inventory[i].physicalItem.GetComponent<ProduceController>();
                                    produce.InitaliseInventoryProduce(inventory[i], currentProduce);

                                }
                                else if (inventory[i].item.prefab != null && inventory[i].item.prefab.GetComponent<ProduceController>() != null)
                                {
                                    ProduceController currentProduce = inventory[i].item.prefab.GetComponent<ProduceController>();
                                    currentProduce.age = currentProduce.ageOfMaturity;

                                    produce.InitaliseInventoryProduce(inventory[i], currentProduce);

                                }
                            }
                            else
                            {
                                produce.InitaliseInventoryProduce(inventory[i], produceController);

                            }
                            UpdateStackText(inventory[i]);

                        }
                        else // if it is coming from another stack
                        {
                            for (int j = 0; j < stackCount; j++)
                            {
                                //produce.InitaliseInventoryProduce(inventory[i], produceInInventory, j);
                                produce.InitaliseInventoryProduce(inventory[i], produceAge, produceInInventory.growthSpeed, produceInInventory.ageOfMaturity, produceInInventory.ageOfDecline, produceInInventory.ageOfSpoilage, produceInInventory.stackQuality);
                                Debug.Log("Initialising from another stack");
                            }
                            UpdateStackText(inventory[i]);

                        }

                        UpdateStackText(inventory[i]);

                    }
                    else Debug.Log(inventory[i].item.itemName + " isn't produce");
                    break;
                }

                if (inventory[i] != null)
                UpdateStackText(inventory[i]);

            }


        }
        else
        {
            //display a pop up if the player can't pick up an item
            textPopUp.SetAndDisplayPopUp("Inventory Capacity Reached");
        }

        GetInventoryWeight();
        //GetInventoryValue();
    }

    public void SpawnNewItem(InventoryItem item, int itemSlot, List<InventoryUIItem> inventory, ItemSlot[] inventorySlots, ChestInventory chest = null, ShopInventory shop = null)
    {
        GameObject newItemUI = Instantiate(inventoryItemPrefab, inventorySlots[itemSlot].transform);
        InventoryUIItem inventoryItem = newItemUI.GetComponent<InventoryUIItem>();
        inventoryItem.isEquipped = false;
        inventorySlots[itemSlot].inventoryItem = inventoryItem;

        Button button = newItemUI.AddComponent<Button>();
        button.onClick.AddListener(() => SelectInventoryItemAsButton(itemSlot));

        inventoryItem.InitialiseItem(item, itemSlot, 1, chest, shop);

        inventory[itemSlot] = inventoryItem;

    }

    void SpawnNewEquipment(InventoryItem item, int itemSlot)
    {
        GameObject newItemUI = Instantiate(inventoryItemPrefab, equipmentSlots[itemSlot].transform);
        InventoryUIItem inventoryItem = newItemUI.GetComponent<InventoryUIItem>();
        inventoryItem.isEquipped = true;
        equipmentSlots[itemSlot].inventoryItem = inventoryItem;

        AddModifiers(item);
        Button button = newItemUI.AddComponent<Button>();
        button.onClick.AddListener(() => SelectInventoryItemAsButton(itemSlot));

        inventoryItem.InitialiseItem(item, itemSlot);

        //inventory[itemSlot] = inventoryItem;

    }
    public void SpawnUsedItem(InventoryUIItem item, int itemSlot, List<InventoryUIItem> inventory, ItemSlot[] inventorySlots, ChestInventory chest = null, ShopInventory shop = null)
    {
        GameObject newItemUI = Instantiate(inventoryItemPrefab, inventorySlots[itemSlot].transform);
        InventoryUIItem inventoryItem = newItemUI.GetComponent<InventoryUIItem>();


        CopyItemVariables(item, inventoryItem, itemSlot);

        inventoryItem.image.sprite = inventoryItem.item.itemIcon;

        UpdateStackText(inventoryItem);

        inventory[itemSlot] = inventoryItem;
        inventorySlots[itemSlot].inventoryItem = inventoryItem;

        inventoryItem.InitialiseItem(item.item, itemSlot, item.numCarried, chest, shop);


    }

    public InventoryUIItem CopyItemVariables(InventoryUIItem originalInventoryItem, InventoryUIItem copyInventoryItem, int itemSlot)
    {
        copyInventoryItem.item = originalInventoryItem.item;
        copyInventoryItem.isInUse = originalInventoryItem.isInUse;
        copyInventoryItem.physicalItem = originalInventoryItem.physicalItem;
        //copyInventoryItem.image.sprite = copyInventoryItem.item.itemIcon;
        copyInventoryItem.ammo = originalInventoryItem.ammo;
        copyInventoryItem.batteryCharge = originalInventoryItem.batteryCharge;
        copyInventoryItem.numCarried = originalInventoryItem.numCarried;
        //copyInventoryItem.chest = originalInventoryItem.chest;
        //copyInventoryItem.shop = originalInventoryItem.shop;
        copyInventoryItem.slot = itemSlot;
        

        Debug.Log(copyInventoryItem.item.itemName + " copied");
        return copyInventoryItem;

        //selectedInventoryItem.GetComponentnsform);
    }

    public void DropItem(InventoryUIItem item, bool mainHand)
    {
        if (item.isInUse || item.batteryCharge < item.item.maxBatteryCharge || item.ammo < item.item.maxAmmo)
        {
            InventoryUIItem copyInventoryItem = item.physicalItem.AddComponent<InventoryUIItem>();
            CopyItemVariables(item, copyInventoryItem, item.slot);
        }

        GameObject itemObject = null;

        if (item.physicalItem != null)
        {
            item.physicalItem.transform.parent = null;
            itemObject = item.physicalItem;

            if (item.physicalItem.GetComponent<Rigidbody>())
            {
                item.physicalItem.GetComponent<Rigidbody>().useGravity = true;
                item.physicalItem.GetComponent<Rigidbody>().isKinematic = false;
            }

            if (item.physicalItem.AddComponent<StickToObject>())
            {
                Destroy(item.physicalItem.GetComponent<StickToObject>());
            }

            item.physicalItem.GetComponent<ItemInWorld>().enabled = true;
            item.physicalItem.GetComponent<Collider>().enabled = true;

        }
        else itemObject = Instantiate(item.item.prefab, rightHandPos.position, Quaternion.identity);

        if (itemObject != null) ParkStats.instance.TrackObject(itemObject);

        if (mainHand)
        {
            //selectedPhysicalItem.GetComponent<ItemInWorld>().enabled = true;
            //selectedPhysicalItem.GetComponent<Collider>().enabled = true;

            RemoveItemFromInventory(item, inventory);
        }
        else
        {
            //offHandItem.GetComponent<ItemInWorld>().enabled = true;
            //offHandItem.GetComponent<Collider>().enabled = true;

            //offHandSlot.inventoryItem.physicalItem.GetComponent<ItemInWorld>().enabled = true;
            //offHandSlot.inventoryItem.physicalItem.GetComponent<Collider>().enabled = true;

            RemoveEquipmentFromSlot(offHandSlot);
            //EndOffHandInspection();
        }
        //if (droppedItem.GetComponent<Breakable>())
        //{
        //    droppedItem.GetComponent<Breakable>().BreakObject();
        //}
    }

    public void ThrowItem(InventoryUIItem item, bool mainHand)
    {
        canThrow = false;

        GameObject itemObject = null;
        
        if (item.physicalItem != null)
        {
            item.physicalItem.transform.parent = null;
            itemObject = item.physicalItem;

            if (item.isInUse || item.batteryCharge < item.item.maxBatteryCharge || item.ammo < item.item.maxAmmo)
            {
                InventoryUIItem copyInventoryItem = item.physicalItem.AddComponent<InventoryUIItem>();
                CopyItemVariables(item, copyInventoryItem, item.slot);
            }

            Vector3 force = Camera.main.transform.forward * throwForce + transform.up * throwUpwardForce;

            item.physicalItem.GetComponent<Rigidbody>().useGravity = true;
            item.physicalItem.GetComponent<Rigidbody>().isKinematic = false;

            item.physicalItem.GetComponent<ItemInWorld>().enabled = true;
            item.physicalItem.GetComponent<Collider>().enabled = true;


            item.physicalItem.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

            if (item.physicalItem.AddComponent<StickToObject>())
            {
                Destroy(item.physicalItem.GetComponent<StickToObject>());
            }
        }
        else itemObject = Instantiate(item.item.prefab, rightHandPos.position, Quaternion.identity);

        if (itemObject != null) ParkStats.instance.TrackObject(itemObject);

        if (mainHand)
        {

            RemoveItemFromInventory(item, inventory);
        }
        else
        {
            //offHandSlot.inventoryItem.physicalItem.GetComponent<ItemInWorld>().enabled = true;
            //offHandSlot.inventoryItem.physicalItem.GetComponent<Collider>().enabled = true;
            RemoveEquipmentFromSlot(offHandSlot);
            //EndOffHandInspection();
        }
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        canThrow = true;
    }

    public void RemoveItemFromInventory(InventoryUIItem item, List<InventoryUIItem> inventory, int numToRemove = 1)
    {
        RemoveModifiers(item.item);


        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == item)
            {
                for (int r = 0; r < numToRemove; r++)
                {
                    inventory[i].numCarried--;
                    inventory[i].stackCountText.text = inventory[i].numCarried.ToString();
                }

                

                if (inventory[i].numCarried <= 0f)
                {
                    if (selectedInventoryItem == inventory[i])
                    {
                        selectedInventoryItem = null;
                        SetSelectedItemColour();
                    }

                    Destroy(inventory[i].gameObject);

                    inventory[i] = null;
                    selectedPhysicalItem = null;

                }
                else
                    if (i == selectedItemSlot)
                {
                    selectedPhysicalItem = null;
                    inventory[i].physicalItem = null;
                    HoldItemMainHand();
                }

                break;
            }
        }


        GetInventoryWeight();
        //GetInventoryValue();
    }

    public void RemoveEquipmentFromSlot(EquipmentSlot slot)
    {
        RemoveModifiers(slot.inventoryItem.item);

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i] == slot)
            {
                if (slot == offHandSlot) offHandItem = null;

                equipmentSlots[i].inventoryItem.numCarried--;
                equipmentSlots[i].inventoryItem.stackCountText.text = equipmentSlots[i].inventoryItem.numCarried.ToString();

                if (equipmentSlots[i].inventoryItem.numCarried <= 0f)
                {
                    //if (equipmentSlots[i].transform.childCount > 0)
                    //{
                    //    foreach (Transform child in equipmentSlots[i].transform)
                    //    {
                    //        Destroy(child.gameObject);
                    //    }
                    //    //SpawnNewEquipment(equipmentSlots[equipIndex].inventoryItem.item, equipmentSlots[equipIndex]); //a = originally B

                    //}
                    Destroy(equipmentSlots[i].inventoryItem.gameObject);

                    equipmentSlots[i].inventoryItem = null;
                }
            }
        }
    }

    public float GetInventoryWeight()
    {
        float weight = 0;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null)
            {
                weight += inventory[i].item.weight * inventory[i].numCarried;
            }
        }

        //player.stats.weight = weight;
        weightText.text = weight.ToString("0.00") + ("KG");
        return inventoryWeight = weight;
    }

    public float GetInventoryValue()
    {
        float value = 0;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null)
            {
                value += inventory[i].itemValue * inventory[i].numCarried;
            }
        }
        //player.weight = value;
        valueText.text = value.ToString("$" + "0.00");
        return inventoryValue = value;
    }

    public bool CheckInventoryForItem(InventoryItem desiredItem)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null)
            {
                if (inventory[i].item == desiredItem)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public int GetItemIndex(InventoryItem desiredItem)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null)
            {
                if (inventory[i].item == desiredItem)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public InventoryUIItem GetInventoryItem(InventoryItem desiredItem)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null)
            {
                if (inventory[i].item == desiredItem)
                {
                    return inventory[i];
                }
            }
        }
        return null;
    }

    public int CheckItemCount(InventoryItem desiredItem)
    {
        foreach (InventoryUIItem item in inventory)
        {
            if (item == desiredItem)
            {
                return item.numCarried;
            }
        }
        return 0;
    }

    public bool CheckIfConsumableItem(InventoryItem desiredItem)
    {
        foreach (InventoryUIItem item in inventory)
        {
            if (item == desiredItem)
            {
                return item.item.canConsume;
            }
        }
        return false;
    }

    private void AddModifiers(InventoryItem item)
    {
        if (item != null)
        {
            player.stats.armour.AddModifier(item.armourModifier);
            player.stats.attackDamage.AddModifier(item.attackDamageModifier);
            player.stats.passiveDamage.AddModifier(item.passiveDamageModifier);
            player.stats.attackSpeed.AddModifier(item.attackSpeedModifier);
            player.stats.knockBack.AddModifier(item.knockBackModifier);
        }
    }

    private void RemoveModifiers(InventoryItem item)
    {
        if (item != null)
        {
            player.stats.armour.RemoveModifier(item.armourModifier);
            player.stats.attackDamage.RemoveModifier(item.attackDamageModifier);
            player.stats.passiveDamage.RemoveModifier(item.passiveDamageModifier);
            player.stats.attackSpeed.RemoveModifier(item.attackSpeedModifier);
            player.stats.knockBack.RemoveModifier(item.knockBackModifier);
        }
    }

    private void HoldItemMainHand()
    {
        if (selectedInventoryItem != null)
        {

            if (selectedPhysicalItem == null && selectedInventoryItem.physicalItem == null)
            {
                //if (selectedInventoryItem.item.isTwoHanded)
                //    EndOffHandInspection(offHandSlot);

                selectedPhysicalItem = Instantiate(selectedInventoryItem.item.prefab, rightHandPos.position, Quaternion.identity, player.transform);

                selectedPhysicalItem.name = selectedInventoryItem.item.prefab.name;
                //selectedPhysicalItem.transform.localRotation = Quaternion.Inverse(rightHandPos.localRotation);

                //lastParentRotation = rightHandPos.localRotation;

                if (selectedPhysicalItem.GetComponent<WeaponItem>() && selectedPhysicalItem.GetComponent<WeaponItem>().isProjectile)
                {
                    StickToObject stick = selectedPhysicalItem.AddComponent<StickToObject>();
                    stick.followTransform = rightHandPos;
                    //selectedPhysicalItem.transform.eulerAngles = player.orientation.forward ;
                    selectedPhysicalItem.transform.LookAt(projectileTarget);
                }
                else if ((selectedPhysicalItem.GetComponent<WeaponItem>() && !selectedPhysicalItem.GetComponent<WeaponItem>().isProjectile) || !selectedPhysicalItem.GetComponent<WeaponItem>())
                {
                    selectedPhysicalItem.transform.parent = rightHandPos;
                    selectedPhysicalItem.transform.LookAt(swordTarget);
                    //Debug.Log("looking at sword target");
                }//selectedPhysicalItem.transform.parent = rightHandPos;
                // selectedPhysicalItem.transform.eulerAngles = Vector3.zero;
                //selectedPhysicalItem.transform.rotation = Quaternion.Inverse( rightHandPos.rotation);
                //selectedPhysicalItem.transform.eulerAngles = selectedInventoryItem.item.heldRotation;


                AddModifiers(selectedInventoryItem.item);

                selectedInventoryItem.physicalItem = selectedPhysicalItem;

                if (selectedPhysicalItem.GetComponent<ItemInWorld>())
                    selectedPhysicalItem.GetComponent<ItemInWorld>().enabled = false;

                if (selectedPhysicalItem.GetComponent<Collider>() && !selectedPhysicalItem.GetComponent<Collider>().isTrigger)
                    selectedPhysicalItem.GetComponent<Collider>().enabled = false;

                if (selectedPhysicalItem.GetComponent<Rigidbody>() && selectedPhysicalItem.GetComponent<Rigidbody>().useGravity != false)
                {
                    selectedPhysicalItem.GetComponent<Rigidbody>().useGravity = false;
                    selectedPhysicalItem.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
            else selectedPhysicalItem = selectedInventoryItem.physicalItem;
        }
    }

    public void HoldItemOffHand(InventoryUIItem item)
    {
        if (offHandItem != null)
        {
            Destroy(offHandItem);
        }

        offHandSlot.inventoryItem = item;


        offHandItem = Instantiate(offHandSlot.inventoryItem.item.prefab, leftHandPos.position, Quaternion.identity, leftHandPos);
        offHandItem.transform.LookAt(swordTarget);


        AddModifiers(item.item);

        item.physicalItem = offHandItem;

        offHandItem.GetComponent<ItemInWorld>().enabled = false;

        if (!offHandItem.GetComponent<Collider>().isTrigger)
            offHandItem.GetComponent<Collider>().enabled = false;

        if (offHandItem.GetComponent<Rigidbody>() && offHandItem.GetComponent<Rigidbody>().useGravity != false)
        {
            offHandItem.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    private void EndItemInspection() //Add exception for items which still function when not selected (e.g. torch, rope)
    {
        if (selectedPhysicalItem != null && selectedInventoryItem != null && !selectedInventoryItem.isInUse)
        {
            RemoveModifiers(selectedInventoryItem.item);

            Destroy(selectedPhysicalItem);
        }

        selectedPhysicalItem = null;
    }

    public void EndOffHandInspection()
    {
        if (offHandSlot.inventoryItem != null)
        {
            RemoveModifiers(offHandSlot.inventoryItem.item);
            offHandSlot.inventoryItem = null;
        }


        if (offHandItem != null && offHandSlot.inventoryItem == null)
        {
            Destroy(offHandItem);
        }
    }

    public void PauseOffHandInspection()
    {
        if (offHandSlot.inventoryItem != null)
        {
            RemoveModifiers(offHandSlot.inventoryItem.item);
            offHandPaused = offHandSlot.inventoryItem;
            offHandSlot.inventoryItem = null;

        }


        if (offHandItem != null)
        {
            Destroy(offHandItem);
        }
    }

    public void ResetHeldItem()
    {
        EndItemInspection();
        HoldItemMainHand();
    }

    public void ConsumeFood(bool mainHand)
    {
        if (mainHand)
        {
            if (selectedInventoryItem != null && selectedInventoryItem.item.canConsume)
            {
                //Apply food effects
                GameObject foodItem = selectedInventoryItem.physicalItem;
                foodItem.GetComponent<ItemAction>().ItemFunction();

                //AnimatorClipInfo[] clipInfo = player.animator.GetCurrentAnimatorClipInfo(1);
                //AnimatorStateInfo animState = player.animator.GetCurrentAnimatorStateInfo(1);


                StartCoroutine(DestroyFoodAfterConsumption(1f, foodItem, mainHand));

                //RemoveItemFromInventory(selectedInventoryItem);

                //if (foodItem != null) Destroy(foodItem);

            }
        }
        else
        {
            if (offHandItem != null && offHandSlot.inventoryItem.item.canConsume)
            {
                //Apply food effects
                GameObject foodItem = offHandSlot.inventoryItem.physicalItem;
                foodItem.GetComponent<ItemAction>().ItemFunction();

                StartCoroutine(DestroyFoodAfterConsumption(1f, foodItem, mainHand));

            }
        }
    }

    public IEnumerator DestroyFoodAfterConsumption(float delay, GameObject foodItem, bool mainHand)
    {
        yield return new WaitForSeconds(delay);

        if (mainHand)
            RemoveItemFromInventory(selectedInventoryItem, inventory);
        else
            RemoveEquipmentFromSlot(offHandSlot);

        if (foodItem != null) Destroy(foodItem);

    }

    public void SelectInventoryItemAsButton(int index)
    {

        if (index <= inventory.Count && inventory[index] != null)
        {

            EndItemInspection();

            selectedItemSlot = index;

            selectedInventoryItem = inventory[index];

            HoldItemMainHand();
        }

    }

    private void SelectInventoryItemWithNumbers()
    {
        if (inventorySlots.Length >= 1)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (inventorySlots.Length >= 1)
                {
                    selectedItemSlot = 0;
                    if (inventory[selectedItemSlot] != null)
                    {
                        selectedInventoryItem = inventory[0];
                        EndItemInspection();
                        HoldItemMainHand();
                    }
                }
            }
        }
        if (inventorySlots.Length >= 2)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (inventorySlots.Length >= 2)
                {
                    selectedItemSlot = 1;
                    if (inventory[selectedItemSlot] != null)
                    {
                        selectedInventoryItem = inventory[1];
                        EndItemInspection();
                        HoldItemMainHand();
                    }
                }
            }
        }
        if (inventorySlots.Length >= 3)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (inventorySlots.Length >= 3)
                {
                    selectedItemSlot = 2;
                    if (inventory[selectedItemSlot] != null)
                    {
                        selectedInventoryItem = inventory[2];
                        EndItemInspection();
                        HoldItemMainHand();
                    }
                }
            }
        }
        if (inventorySlots.Length >= 4)
        {
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (inventorySlots.Length >= 4)
                {
                    selectedItemSlot = 3;
                    if (inventory[selectedItemSlot] != null)
                    {
                        selectedInventoryItem = inventory[3];
                        EndItemInspection();
                        HoldItemMainHand();
                    }
                }
            }
        }
        if (inventorySlots.Length >= 5)
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (inventorySlots.Length >= 5)
                {
                    selectedItemSlot = 4;
                    if (inventory[selectedItemSlot] != null)
                    {
                        selectedInventoryItem = inventory[4];
                        EndItemInspection();
                        HoldItemMainHand();
                    }
                }
            }
        }
        if (inventorySlots.Length >= 6)
        {
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (inventorySlots.Length >= 6)
                {
                    selectedItemSlot = 5;
                    if (inventory[selectedItemSlot] != null)
                    {
                        selectedInventoryItem = inventory[5];
                        EndItemInspection();
                        HoldItemMainHand();
                    }
                }
            }
        }
        if (inventorySlots.Length >= 7)
        {
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (inventorySlots.Length >= 7)
                {
                    selectedItemSlot = 6;
                    if (inventory[selectedItemSlot] != null)
                    {
                        selectedInventoryItem = inventory[6];
                        EndItemInspection();
                        HoldItemMainHand();
                    }
                }
            }
        }
        if (inventorySlots.Length >= 8)
        {
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (inventorySlots.Length >= 8)
                {
                    selectedItemSlot = 7;
                    if (inventory[selectedItemSlot] != null)
                    {
                        selectedInventoryItem = inventory[7];
                        EndItemInspection();
                        HoldItemMainHand();
                    }
                }
            }
        }
        if (inventorySlots.Length >= 9)
        {
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                if (inventorySlots.Length >= 9)
                {
                    selectedItemSlot = 8;

                    if (inventory[selectedItemSlot] != null)
                    {
                        selectedInventoryItem = inventory[8];
                        EndItemInspection();
                        HoldItemMainHand();
                    }
                }
            }
        }
        if (inventorySlots.Length == 10)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (inventorySlots.Length >= 10)
                {
                    selectedItemSlot = 9;

                    if (inventory[selectedItemSlot] != null)
                    {
                        selectedInventoryItem = inventory[9];
                        EndItemInspection();
                        HoldItemMainHand();
                    }
                }
            }
        }
    }

    private void SelectInventoryItemWithScroll()
    {
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            selectedItemSlot += Mathf.RoundToInt(Input.mouseScrollDelta.y);
            //selectedItemSlot += Mathf.Clamp(Mathf.RoundToInt(Input.mouseScrollDelta.y), 1, inventorySlots.Length);

            if (inventoryWindowOpen)
            {
                //int lastFilledSlot = 0;

                //for (int i = 0; i < inventorySlots.Length; i++)
                //{
                //    if (inventorySlots[i].childCount <= 0)
                //        break;
                //    else
                //        lastFilledSlot = i;
                //}

                //Debug.Log("Last filled slot = " + lastFilledSlot);

                if (selectedItemSlot > inventorySlots.Length - 1)
                {
                    selectedItemSlot = 0;
                }
                else if (selectedItemSlot < 0)
                {
                    selectedItemSlot = inventorySlots.Length - 1;
                }
            }
            else
            {
                if (selectedItemSlot > inventorySlots.Length - inventoryWindowPanel.transform.childCount - 1)
                {
                    selectedItemSlot = 0;
                }
                else if (selectedItemSlot < 0)
                {
                    selectedItemSlot = inventorySlots.Length - inventoryWindowPanel.transform.childCount - 1;
                }
            }


            if (inventory[selectedItemSlot] != null)
            {
                EndItemInspection();
                selectedInventoryItem = inventory[selectedItemSlot];

                HoldItemMainHand();
            }
            else
            {
                EndItemInspection();
                selectedInventoryItem = null;

            }
        }
    }

    private void SetSelectedItemColour()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i == selectedItemSlot)
            {
                inventorySlots[i].GetComponent<Image>().color = selectedColour;
            }
            else inventorySlots[i].GetComponent<Image>().color = originalColour;
        }
    }

    public void SwapItemSlot(int indexA, int indexB, bool equippedItem)
    {
        //Debug.Log(inventorySlots[indexB].inventoryItem + " Num carried = " + inventorySlots[indexB].inventoryItem.numCarried);

        //if (inventorySlots[indexA].inventoryItem != null)
        //    Debug.Log(inventorySlots[indexA].inventoryItem + " Num carried = " + inventorySlots[indexA].inventoryItem.numCarried);


        if (!equippedItem)
        {
            // Store emote A info
            InventoryUIItem inventoryItem = inventorySlots[indexA].inventoryItem;
            ProduceInInventory produceB = null;
            if (inventorySlots[indexA].inventoryItem != null && inventorySlots[indexA].inventoryItem.GetComponent<ProduceInInventory>())
            {
                produceB = inventorySlots[indexA].inventoryItem.GetComponent<ProduceInInventory>();
            }

            ProduceInInventory produceA = null;
            if (inventorySlots[indexB].inventoryItem.GetComponent<ProduceInInventory>())
            {
                produceA = inventorySlots[indexB].inventoryItem.GetComponent<ProduceInInventory>();
            }
            //Swap Slot A Info for slot B
            inventorySlots[indexA].inventoryItem = inventorySlots[indexB].inventoryItem;
            produceItems[indexA] = produceA;

           
            // Swap slot B info for stored A info
            inventorySlots[indexB].inventoryItem = inventoryItem;
            produceItems[indexB] = produceB;

            //Delete & Spawn UI

            //Debug.Log("Num carried 1 = " + inventorySlots[indexA].inventoryItem.numCarried);


            EndItemInspection();

            if (inventorySlots[indexA].transform.childCount > 0)
            {
                foreach (Transform child in inventorySlots[indexA].transform)
                {
                    Destroy(child.gameObject);
                }

            }

            if (inventorySlots[indexB].transform.childCount > 0)
            {
                foreach (Transform child in inventorySlots[indexB].transform)
                {
                    Destroy(child.gameObject);
                }


                SpawnUsedItem(inventorySlots[indexA].inventoryItem, indexA, inventory, inventorySlots); //a = originally B

                if (produceItems[indexA] != null)
                {
                    ProduceInInventory prodInInv = inventory[indexA].AddComponent<ProduceInInventory>();

                    prodInInv.produceItem = inventory[indexA];

                    if (prodInInv.produceItem.item.isStackable)
                    {
                        for (int j = 0; j < produceItems[indexA].produceAgesInStack.Count; j++)
                        {
                            if (j == 0)
                            {
                                prodInInv.InitaliseInventoryProduce(inventory[indexA], produceItems[indexA], j);
                            }

                            prodInInv.produceAgesInStack.Add(produceItems[indexA].produceAgesInStack[j]);

                        }
                    }
                    else
                    {
                        prodInInv.InitaliseInventoryProduce(inventory[indexA], produceItems[indexA], 0);
                        prodInInv.produceAgesInStack.Add(produceItems[indexA].produceAgesInStack[0]);


                    }
                }

            }

            if (inventoryItem != null)
            {

                SpawnUsedItem(inventorySlots[indexB].inventoryItem, indexB, inventory, inventorySlots); // originally A


                if (produceItems[indexB] != null)
                {
                    ProduceInInventory prodInInv = inventory[indexB].AddComponent<ProduceInInventory>();

                    prodInInv.produceItem = inventory[indexB];
                    if (prodInInv.produceItem.item.isStackable)
                    {

                        for (int j = 0; j < produceItems[indexB].produceAgesInStack.Count; j++)
                        {
                            if (j == 0)
                            {
                                prodInInv.InitaliseInventoryProduce(inventory[indexB], produceItems[indexB], j);
                            }

                            prodInInv.produceAgesInStack.Add(produceItems[indexB].produceAgesInStack[j]);

                        }
                    }
                    else
                    {
                        prodInInv.InitaliseInventoryProduce(inventory[indexB], produceItems[indexB], 0);
                        prodInInv.produceAgesInStack.Add(produceItems[indexB].produceAgesInStack[0]);

                    }
                }



            }


            HoldItemMainHand();

            Debug.Log("Swapped " + indexA + " UI Slot with " + indexB + " UI Slot");
        }
        else
        {
            //Debug.Log("Entering Swap Equipment Method");
            SwapEquipmentSlot(indexA, indexB);
        }

    }

    public void SwapEquipmentSlot(int invIndex, int equipIndex)
    {
       // Debug.Log("Swapping Equipment");
        // Store equipment info
        InventoryUIItem inventoryItem = equipmentSlots[equipIndex].inventoryItem;

        //Swap  item Info for equipment info
        equipmentSlots[equipIndex].inventoryItem = inventorySlots[invIndex].inventoryItem;

        // Swap equipment info for stored item info
        inventorySlots[invIndex].inventoryItem = inventoryItem;

        //Delete & Spawn UI

        EndItemInspection();
        //EndOffHandInspection();

      

        if (inventorySlots[invIndex].transform.childCount > 0) // Destroy each child of the old inventory item
        {
            foreach (Transform child in inventorySlots[invIndex].transform)
            {
                Destroy(child.gameObject);
            }
        }

        if (equipmentSlots[equipIndex].transform.childCount > 0) // Destroy each child of the old equipment item
        {
            foreach (Transform child in equipmentSlots[equipIndex].transform)
            {
                Destroy(child.gameObject);
            }

            if (inventorySlots[invIndex].inventoryItem.numCarried > 1 || inventorySlots[invIndex].inventoryItem.isInUse)
                SpawnUsedItem(inventorySlots[invIndex].inventoryItem, invIndex, inventory, inventorySlots); //a = originally B
            else
                SpawnNewItem(inventorySlots[invIndex].inventoryItem.item, invIndex, inventory, inventorySlots); //a = originally B

        }

        if (equipmentSlots[equipIndex].inventoryItem != null)
        {
            //if (equipmentSlots[equipIndex].inventoryItem.numCarried > 0 || equipmentSlots[equipIndex].inventoryItem.isInUse)
            //    //SpawnUsedItem(equipmentSlots[equipIndex].inventoryItem, equipIndex, inventory, equipmentSlots); //a = originally B
            //else
            //    SpawnNewEquipment(equipmentSlots[equipIndex].inventoryItem.item, equipIndex); //spawn new equipment item (old inventory item)

        }

        HoldItemMainHand();



    }

    public void Sell(int shopIndex, int invIndex, ShopInventory shop, ItemSlot[] shopSlots, List<InventoryUIItem>shopInventory, int numToSell = 1, ProduceInInventory produceInInventory = null)
    {
        Debug.Log("Selling");

        for (int i = 0; i < shop.typesWillBuy.Length; i++)
        {
            if (shop.typesWillBuy[i] == inventorySlots[invIndex].inventoryItem.item.itemType) break;

            if (i + 1 >= shop.typesWillBuy.Length)
            {
                Debug.Log( "This shop won't buy this type of item");
                return;
            }
        }

        //Swap  item Info for equipment info
        shopSlots[shopIndex].inventoryItem = inventorySlots[invIndex].inventoryItem;


        if (inventorySlots[invIndex].inventoryItem != null)
            RemoveModifiers(inventorySlots[invIndex].inventoryItem.item);

        EndItemInspection();

        if (inventorySlots[invIndex].transform.childCount > 0)
        {

            for (int i = 0; i < numToSell; i++)
            {
                ProduceController produceController = null;
                if (shopSlots[shopIndex].inventoryItem.GetComponent<ProduceInInventory>())
                {
                    if (shopSlots[shopIndex].inventoryItem.GetComponent<ProduceInInventory>().produceAgesInStack != null && shopSlots[shopIndex].inventoryItem.GetComponent<ProduceInInventory>().produceAgesInStack.Count > numToSell)
                        produceController = shopSlots[shopIndex].inventoryItem.GetComponent<ProduceInInventory>().CreateNewProduceController(i);
                }

                AddItemToInventory(shopSlots[shopIndex].inventoryItem.item, shopInventory, shopSlots, null, produceController);

                //remove from current stack
                if (produceInInventory != null && produceInInventory.produceAgesInStack.Count > i)
                    produceInInventory.produceAgesInStack.RemoveAt(i);

                money += shopSlots[shopIndex].inventoryItem.itemValue;

            }

            if (shopSlots[shopIndex].inventoryItem == inventorySlots[invIndex].inventoryItem)
                shopSlots[shopIndex].inventoryItem = shopSlots[shopIndex].GetComponentInChildren<InventoryUIItem>();


            if (shopSlots[shopIndex].inventoryItem.GetComponent<ProduceInInventory>())
            {
                shop.produceItems[shopIndex] = shop.buyBackInventorySlots[shopIndex].inventoryItem.GetComponent<ProduceInInventory>();
            }

            inventorySlots[invIndex].inventoryItem.numCarried -= numToSell;


            if (inventorySlots[invIndex].inventoryItem.numCarried <= 0)
            {
                RemoveItemFromInventory(inventorySlots[invIndex].inventoryItem, inventory);
            }
            else UpdateStackText(inventorySlots[invIndex].inventoryItem);


           

            if (shopSlots[shopIndex].inventoryItem != null)
            {
                shopInventory[shopIndex] = shopSlots[shopIndex].inventoryItem;
                shopInventory[shopIndex].shop = shop;
            }
            else shopInventory[shopIndex] = null;

        }

        //if (inventoryItem != null)
        //    SpawnNewItem(inventorySlots[invIndex].inventoryItem.item, invIndex);

        HoldItemMainHand();
    }

    public void Buy(int invIndex, int shopIndex, ShopInventory shop, ItemSlot[] shopSlots, List<InventoryUIItem> shopInventory, int numToBuy = 1, bool buyBack = false, ProduceInInventory produceInInventory = null)
    {
        if (money >= numToBuy * shopSlots[shopIndex].inventoryItem.itemValue)
        {
            Debug.Log("Buying " + shopSlots[shopIndex].inventoryItem.item.itemName + " for $" + shopSlots[shopIndex].inventoryItem.itemValue);

            //Swap inv item Info for equipment info
            inventorySlots[invIndex].inventoryItem = shopSlots[shopIndex].inventoryItem;

            EndItemInspection();


            if (shopSlots[shopIndex].transform.childCount > 0)
            {
                for (int i = 0; i < numToBuy; i++)
                {
                    ProduceController produceController = null;
                    if (shopSlots[shopIndex].inventoryItem.GetComponent<ProduceInInventory>() && shopSlots[shopIndex].inventoryItem.GetComponent<ProduceInInventory>().produceAgesInStack.Count > numToBuy)
                    {
                        produceController = shopSlots[shopIndex].inventoryItem.GetComponent<ProduceInInventory>().CreateNewProduceController(i);
                        Debug.Log("Creating a produce controller for " + shopSlots[shopIndex].inventoryItem.item.itemName);
                    }

                    AddItemToInventory(inventorySlots[invIndex].inventoryItem.item, inventory, inventorySlots, null, produceController);
                    money -= shopInventory[shopIndex].itemValue;

                    if (buyBack)
                    {
                        if (shopInventory[shopIndex].GetComponent<ProduceInInventory>())
                        {
                            shop.produceItems[shopIndex] = null;
                        }

                        RemoveItemFromInventory(shopInventory[shopIndex], shopInventory);

                        Debug.Log("Removing item from shop");
                    }
                }

                inventorySlots[invIndex].inventoryItem = inventorySlots[invIndex].GetComponentInChildren<InventoryUIItem>();

               
            }

            HoldItemMainHand();
        }
        else textPopUp.SetAndDisplayPopUp("You don't have enough money");
    }

    public void ToggleBuyBackMenu()
    {
        if (currentShop != null)
            if (currentShop.buyBackPanelOpen)
            {
                shopPanel.SetActive(true);
                currentShop.SaveInventory();
                buyBackPanel.SetActive(false);
                currentShop.buyBackPanelOpen = false;
            }
            else
            {
                buyBackPanel.SetActive(true);
                shopPanel.SetActive(false);

                currentShop.buyBackPanelOpen = true;

            }
    }

    public void SwapToChest(int chestIndex, int invIndex, ChestInventory chest, int numToStash = 1, ProduceInInventory produceInInventory = null)
    {
        if (chest.inventorySlots[chestIndex].inventoryItem != null) // swap both whole stacks
        {
            Debug.Log("Storing in chest");

            // Store item info
            InventoryUIItem inventoryItem = chest.inventorySlots[chestIndex].inventoryItem;

            //Swap  item Info for equipment info
            chest.inventorySlots[chestIndex].inventoryItem = inventorySlots[invIndex].inventoryItem;

            // Swap equipment info for stored item info
            inventorySlots[invIndex].inventoryItem = inventoryItem;

            if (inventoryItem != null)
                RemoveModifiers(inventoryItem.item);

            EndItemInspection();

            if (inventorySlots[invIndex].transform.childCount > 0)
            {

                for (int i = 0; i < numToStash; i++)
                {
                    ProduceController produceController = null;
                    if (chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>())
                    {
                        if (chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>().produceAgesInStack != null && chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>().produceAgesInStack.Count > numToStash)
                            produceController = chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>().CreateNewProduceController(i);
                    }

                    AddItemToInventory(chest.inventorySlots[chestIndex].inventoryItem.item, chest.inventory, chest.inventorySlots, null, produceController);

                    //remove from current stack
                    if (produceInInventory != null && produceInInventory.produceAgesInStack.Count > i)
                        produceInInventory.produceAgesInStack.RemoveAt(i);
                }

                if (chest.inventorySlots[chestIndex].inventoryItem != null)
                {
                    chest.inventory[chestIndex] = chest.inventorySlots[chestIndex].inventoryItem;

                    chest.inventory[chestIndex].chest = chest;
                }
                else chest.inventory[chestIndex] = null;

            }
        }
        else
        {
            chest.inventorySlots[chestIndex].inventoryItem = inventorySlots[invIndex].inventoryItem;

            if (inventorySlots[invIndex].transform.childCount > 0)
            {
                for (int i = 0; i < numToStash; i++)
                {
                    AddItemToInventory(inventorySlots[invIndex].inventoryItem.item, chest.inventory, chest.inventorySlots);

                }
                //SpawnUsedItem(inventorySlots[invIndex].inventoryItem, invIndex, inventory, inventorySlots); //a = originally B
                inventorySlots[invIndex].inventoryItem.numCarried -= numToStash;

                if (inventorySlots[invIndex].inventoryItem.GetComponent<ProduceInInventory>())
                {
                    for (int i = 0; i < numToStash; i++)
                    {
                        if (inventorySlots[invIndex].inventoryItem.GetComponent<ProduceInInventory>().produceAgesInStack.Count > i)
                        {
                            inventorySlots[invIndex].inventoryItem.GetComponent<ProduceInInventory>().produceAgesInStack.RemoveAt(i);
                        }
                        else break;
                    }
                }

                if (inventorySlots[invIndex].inventoryItem.numCarried <= 0)
                {
                    RemoveItemFromInventory(inventorySlots[invIndex].inventoryItem, inventory);
                }
                else UpdateStackText(inventorySlots[invIndex].inventoryItem);

                chest.inventorySlots[chestIndex].inventoryItem.chest = chest;

                if (chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>())
                {
                    chest.produceItems[chestIndex] = chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>();
                }

            }

            if (inventorySlots[invIndex].inventoryItem != null)
            {

                //SpawnUsedItem(inventorySlots[invIndex].inventoryItem, invIndex, inventory, inventorySlots); //a = originally B

                inventory[invIndex] = inventorySlots[invIndex].inventoryItem;
                inventory[invIndex].chest = null;

            }
            else inventory[invIndex] = null;

        }

        HoldItemMainHand();
    }

    public void SwapFromChest(int invIndex, int chestIndex, ChestInventory chest, int numToTake = 1, ProduceInInventory produceInInventory = null)
    {
        Debug.Log("Taking from chest");

        if (inventorySlots[invIndex].inventoryItem != null) // swap both whole stacks
        {
            // Store item info
            InventoryUIItem inventoryItem = inventorySlots[invIndex].inventoryItem;

            //Swap  item Info for equipment info
            inventorySlots[invIndex].inventoryItem = chest.inventorySlots[chestIndex].inventoryItem;

            // Swap equipment info for stored item info
            chest.inventorySlots[chestIndex].inventoryItem = inventoryItem;

            //if (inventoryItem != null)
            //    RemoveModifiers(inventoryItem.item);

            EndItemInspection();


            if (chest.inventorySlots[chestIndex].transform.childCount > 0)
            {

                for (int i = 0; i < numToTake; i++)
                {
                    ProduceController produceController = null;
                    if (chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>() && chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>().produceAgesInStack.Count > numToTake)
                    {
                        produceController = chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>().CreateNewProduceController(i);
                        Debug.Log("Creating a produce controller for " + chest.inventorySlots[chestIndex].inventoryItem.item.itemName);
                    }

                    AddItemToInventory(inventorySlots[invIndex].inventoryItem.item, inventory, inventorySlots, null, produceController);

                    if (chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>())
                    {
                        chest.produceItems[chestIndex] = null;
                        Debug.Log("Removing produce from chest");
                    }

                    RemoveItemFromInventory(chest.inventory[chestIndex], chest.inventory);

                    Debug.Log("Removing item from chest");

                }

                inventorySlots[invIndex].inventoryItem = inventorySlots[invIndex].GetComponentInChildren<InventoryUIItem>();


                inventorySlots[invIndex].inventoryItem.chest = null;

               

            }

            if (inventoryItem != null)
            {

                //SpawnUsedItem(chest.inventorySlots[chestIndex].inventoryItem, chestIndex, chest.inventory, chest.inventorySlots); //a = originally B

                chest.inventory[chestIndex] = chest.inventorySlots[chestIndex].inventoryItem;
                chest.inventory[chestIndex].chest = chest;

                if (chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>())
                {
                    chest.produceItems[chestIndex] = null;
                    Debug.Log("Removing produce from chest");
                }
            }
            else chest.inventory[chestIndex] = null;

        }
        else
        {
            inventorySlots[invIndex].inventoryItem = chest.inventorySlots[chestIndex].inventoryItem;

            if (chest.inventorySlots[chestIndex].transform.childCount > 0)
            {
                for (int i = 0; i < numToTake; i++)
                {
                    AddItemToInventory(inventorySlots[invIndex].inventoryItem.item, inventory, inventorySlots);

                }
                //SpawnUsedItem(inventorySlots[invIndex].inventoryItem, invIndex, inventory, inventorySlots); //a = originally B

                chest.inventorySlots[chestIndex].inventoryItem.numCarried -= numToTake;

                if (chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>())
                {
                    for (int i = 0; i < numToTake; i++)
                    {
                        if (chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>().produceAgesInStack.Count > i)
                        {
                            chest.inventorySlots[chestIndex].inventoryItem.GetComponent<ProduceInInventory>().produceAgesInStack.RemoveAt(i);
                        }
                        else break;
                    }
                }

                if (chest.inventorySlots[chestIndex].inventoryItem.numCarried <= 0)
                {
                    RemoveItemFromInventory(chest.inventorySlots[chestIndex].inventoryItem, chest.inventory);
                }
                else UpdateStackText(chest.inventorySlots[chestIndex].inventoryItem);

                inventorySlots[invIndex].inventoryItem.chest = null;

            } 

            if (chest.inventorySlots[chestIndex].inventoryItem != null)
            {

                //SpawnUsedItem(chest.inventorySlots[chestIndex].inventoryItem, chestIndex, chest.inventory, chest.inventorySlots); //a = originally B

                chest.inventory[chestIndex] = chest.inventorySlots[chestIndex].inventoryItem;
                chest.inventory[chestIndex].chest = chest;

            }
            else chest.inventory[chestIndex] = null;


        }

        HoldItemMainHand();
    }
}
