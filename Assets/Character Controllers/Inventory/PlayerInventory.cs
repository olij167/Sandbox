using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

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

    public GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI selectedItemNameText;
    [SerializeField] private TextMeshProUGUI selectedItemDescText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private TextMeshProUGUI valueText;

    private ItemSlot[] inventorySlots;
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

    public List<InventoryUIItem> inventory;
    [SerializeField]private PlayerController player;
    private ThirdPersonCam cam;


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
        GetInventoryValue();

        infoPanel.SetActive(false);

        rightArm.weight = 0f;
        leftArm.weight = 0f;
    }

    private void Update()
    {
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
                    leftArm.weight = 1;

                    if (selectedInventoryItem.physicalItem.GetComponent<WeaponItem>() && selectedInventoryItem.physicalItem.GetComponent<WeaponItem>().isProjectile)
                    {
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

        if (infoPanel.activeSelf)
        {
            if ((selectedInventoryItem != null && selectedInventoryItem.item.itemName != selectedItemNameText.text) || (selectedInventoryItem == null && selectedItemNameText.text != null))
            {
                SetSelectedItemInfoUI();
                Debug.Log("Item info set");
            }
        }
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

        cam.freezeCameraRotation = true;

        infoPanel.SetActive(true);


        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void CloseInventoryWindow()
    {
        inventoryWindowPanel.SetActive(false);
        inventoryEquipmentPanel.SetActive(false);
        cam.freezeCameraRotation = false;

        infoPanel.SetActive(false);
        SetSelectedItemInfoUI();


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetSelectedItemInfoUI()
    {

        if (selectedInventoryItem != null)
        {
            selectedItemNameText.text = selectedInventoryItem.item.itemName;
            selectedItemDescText.text = selectedInventoryItem.item.itemDescription;

            statsText.text = null;

            if (selectedInventoryItem.item.healthEffect != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Health Effect: " + selectedInventoryItem.item.healthEffect;
            }

            if (selectedInventoryItem.item.staminaEffect != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Stamina Effect: " + selectedInventoryItem.item.staminaEffect;
            }

            if (selectedInventoryItem.item.healthModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Health: " + selectedInventoryItem.item.healthModifier;
            }

            if (selectedInventoryItem.item.staminaModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Stamina: " + selectedInventoryItem.item.staminaModifier;
            }

            if (selectedInventoryItem.item.oxygenModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Oxygen: " + selectedInventoryItem.item.oxygenModifier;
            }

            if (selectedInventoryItem.item.speedModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Speed: " + selectedInventoryItem.item.speedModifier;
            }

            if (selectedInventoryItem.item.jumpModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Jump: " + selectedInventoryItem.item.jumpModifier;
            }

            if (selectedInventoryItem.item.armourModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Armour: " + selectedInventoryItem.item.armourModifier;
            }

            if (selectedInventoryItem.item.attackDamageModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Attack Damage: " + selectedInventoryItem.item.attackDamageModifier;
            }  
            
            if (selectedInventoryItem.item.passiveDamageModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Passive Damage: " + selectedInventoryItem.item.passiveDamageModifier;
            }

            if (selectedInventoryItem.item.attackSpeedModifier != 0)
            {
                if (statsText.text != null) statsText.text += "\n";

                statsText.text += "Attack Speed: " + selectedInventoryItem.item.attackSpeedModifier;
            }

            if (selectedInventoryItem.item.knockBackModifier != 0)
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

    public void AddItemToInventory(InventoryItem item, GameObject itemInWorld)
    {
        if (player.stats.weight + item.weight <= player.stats.maxWeight.GetValue())
        {
            //if item is already in inventory increase num carried (in 'InventoryItem' scriptable object)
            if (item.isStackable)
            {
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
                            slotsWithItem[i].stackCountText.text = "[" + slotsWithItem[i].numCarried + "]";

                            if (itemInWorld.GetComponent<ItemInWorld>())
                            {
                                Destroy(itemInWorld);

                                if (setCamera.isFirstPerson)
                                    firstPersonRaycast.selectedObject = null;
                                else thirdPersonRaycast.selectedObject = null;
                                //playerInteractionRaycast.interactPromptIndicator.SetActive(false);
                            }
                            GetInventoryWeight();
                            GetInventoryValue();
                            break;
                        }
                        else if (i == slotsWithItem.Count - 1)// if the capacity is full check if it can take a new slot
                        {
                            AddItemToNewInventorySlot(item, itemInWorld);
                            break;
                        }
                    }
                }
                else AddItemToNewInventorySlot(item, itemInWorld);

            }
            else // otherwise add a new item to the inventory
            {
                AddItemToNewInventorySlot(item, itemInWorld);
            }
        }
        else
        {
            //display a pop up if the player can't pick up an item
            textPopUp.SetAndDisplayPopUp("Weight Limit Reached");
        }

    }

    int CheckEmptySlots(ItemSlot[] slots)
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

    void AddItemToNewInventorySlot(InventoryItem item, GameObject itemInWorld)
    {
        if (CheckEmptySlots(inventorySlots) > 0)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].transform.childCount == 0) // for the first empty slot
                {
                    if (setCamera.isFirstPerson)
                    {
                        if (firstPersonRaycast.selectedObject.GetComponent<InventoryUIItem>()) // check whether it has used variables
                        {
                            SpawnUsedItem(firstPersonRaycast.selectedObject.GetComponent<InventoryUIItem>(), i); // add a new inventory item with old variables

                            //if (inventory[i].physicalItem.GetComponent<RopeItem>())
                            //{
                            //    inventory[i].physicalItem.GetComponent<RopeItem>().inventoryItem = inventory[i];
                            //}

                            firstPersonRaycast.selectedObject = null;

                        }
                        else
                        {
                            SpawnNewItem(item, i);
                        }

                        if (itemInWorld.GetComponent<ItemInWorld>())
                        {
                            Destroy(itemInWorld);
                            firstPersonRaycast.selectedObject = null;
                        }
                    }
                    else if (thirdPersonRaycast.selectedObject.GetComponent<InventoryUIItem>()) // check whether it has used variables
                    {
                        SpawnUsedItem(thirdPersonRaycast.selectedObject.GetComponent<InventoryUIItem>(), i); // add a new inventory item with old variables

                        //if (inventory[i].physicalItem.GetComponent<RopeItem>())
                        //{
                        //    inventory[i].physicalItem.GetComponent<RopeItem>().inventoryItem = inventory[i];
                        //}

                        thirdPersonRaycast.selectedObject = null;

                    }
                    else
                    {
                        SpawnNewItem(item, i);


                    }

                    if (itemInWorld.GetComponent<ItemInWorld>())
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

                    break;
                }



                //if (inventory[i].physicalItem == itemInWorld && !inventory[i].isInUse)
                //{
                //    Destroy(itemInWorld);
                //    playerInteractionRaycast.selectedObject = null;
                //}
                //else if (inventory[i].isInUse) playerInteractionRaycast.selectedObject = null;
            }
            //if (itemInWorld.GetComponent<ItemInWorld>() && !inventory[])
            //{
            //    Destroy(itemInWorld);
            //    playerInteractionRaycast.selectedObject = null;
            //}

        }
        else
        {
            //display a pop up if the player can't pick up an item
            textPopUp.SetAndDisplayPopUp("Inventory Capacity Reached");
        }

        GetInventoryWeight();
        GetInventoryValue();
    }

    void SpawnNewItem(InventoryItem item, int itemSlot)
    {
        GameObject newItemUI = Instantiate(inventoryItemPrefab, inventorySlots[itemSlot].transform);
        InventoryUIItem inventoryItem = newItemUI.GetComponent<InventoryUIItem>();
        inventoryItem.isEquipped = false;
        inventorySlots[itemSlot].inventoryItem = inventoryItem;

        Button button = newItemUI.AddComponent<Button>();
        button.onClick.AddListener(() => SelectInventoryItemAsButton(itemSlot));

        inventoryItem.InitialiseItem(item, itemSlot);

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
    void SpawnUsedItem(InventoryUIItem usedItem, int itemSlot)
    {
        GameObject newItemUI = Instantiate(inventoryItemPrefab, inventorySlots[itemSlot].transform);
        InventoryUIItem inventoryItem = newItemUI.GetComponent<InventoryUIItem>();

        inventorySlots[itemSlot].inventoryItem = inventoryItem;

        CopyItemVariables(usedItem, inventoryItem);

        inventoryItem.image.sprite = inventoryItem.item.itemIcon;

        inventory[itemSlot] = inventoryItem;

    }

    InventoryUIItem CopyItemVariables(InventoryUIItem originalInventoryItem, InventoryUIItem copyInventoryItem)
    {
        copyInventoryItem.item = originalInventoryItem.item;
        copyInventoryItem.isInUse = originalInventoryItem.isInUse;
        copyInventoryItem.physicalItem = originalInventoryItem.physicalItem;
        //copyInventoryItem.image.sprite = copyInventoryItem.item.itemIcon;
        copyInventoryItem.ammo = originalInventoryItem.ammo;
        copyInventoryItem.batteryCharge = originalInventoryItem.batteryCharge;
        copyInventoryItem.numCarried = originalInventoryItem.numCarried;

        Debug.Log("InventoryItem copied");
        return copyInventoryItem;

        //selectedInventoryItem.GetComponentnsform);
    }

    public void DropItem(InventoryUIItem item, bool mainHand)
    {
        if (item.isInUse || item.batteryCharge < item.item.maxBatteryCharge || item.ammo < item.item.maxAmmo)
        {
            InventoryUIItem copyInventoryItem = item.physicalItem.AddComponent<InventoryUIItem>();
            CopyItemVariables(item, copyInventoryItem);
        }

        item.physicalItem.transform.parent = null;

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

        if (mainHand)
        {
            //selectedPhysicalItem.GetComponent<ItemInWorld>().enabled = true;
            //selectedPhysicalItem.GetComponent<Collider>().enabled = true;

            RemoveItemFromInventory(item);
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

        item.physicalItem.transform.parent = null;

        if (item.isInUse || item.batteryCharge < item.item.maxBatteryCharge || item.ammo < item.item.maxAmmo)
        {
            InventoryUIItem copyInventoryItem = item.physicalItem.AddComponent<InventoryUIItem>();
            CopyItemVariables(item, copyInventoryItem);
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

        if (mainHand)
        {
            
            RemoveItemFromInventory(item);
        }
        else
        {
            //offHandSlot.inventoryItem.physicalItem.GetComponent<ItemInWorld>().enabled = true;
            //offHandSlot.inventoryItem.physicalItem.GetComponent<Collider>().enabled = true;
            RemoveEquipmentFromSlot( offHandSlot);
            //EndOffHandInspection();
        }
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        canThrow = true;
    }

    public void RemoveItemFromInventory(InventoryUIItem item)
    {
        RemoveModifiers(item.item);


        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == item)
            {
                inventory[i].numCarried--;
                inventory[i].stackCountText.text = "[" + inventory[i].numCarried + "]";

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
            }
        }


        GetInventoryWeight();
        GetInventoryValue();
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
                equipmentSlots[i].inventoryItem.stackCountText.text = "[" + equipmentSlots[i].inventoryItem.numCarried + "]";

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
                value += inventory[i].item.itemValue * inventory[i].numCarried;
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
                }//selectedPhysicalItem.transform.parent = rightHandPos;
                // selectedPhysicalItem.transform.eulerAngles = Vector3.zero;
                //selectedPhysicalItem.transform.rotation = Quaternion.Inverse( rightHandPos.rotation);
                //selectedPhysicalItem.transform.eulerAngles = selectedInventoryItem.item.heldRotation;


                AddModifiers(selectedInventoryItem.item);

                selectedInventoryItem.physicalItem = selectedPhysicalItem;

                selectedPhysicalItem.GetComponent<ItemInWorld>().enabled = false;

                if (!selectedPhysicalItem.GetComponent<Collider>().isTrigger)
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
            RemoveItemFromInventory(selectedInventoryItem);
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

        if (!equippedItem)
        {
            // Store emote A info
            InventoryUIItem inventoryItem = inventorySlots[indexA].inventoryItem;

            //Swap Slot A Info for slot B
            inventorySlots[indexA].inventoryItem = inventorySlots[indexB].inventoryItem;

            // Swap slot B info for stored A info
            inventorySlots[indexB].inventoryItem = inventoryItem;

            //Delete & Spawn UI

            EndItemInspection();


            if (inventorySlots[indexA].transform.childCount > 0)
            {
                foreach (Transform child in inventorySlots[indexA].transform)
                {
                    Destroy(child.gameObject);
                }
                //SpawnNewEmoteUI(inventorySlots[indexB].inventoryItem.emote, indexB);

            }

            if (inventorySlots[indexB].transform.childCount > 0)
            {
                foreach (Transform child in inventorySlots[indexB].transform)
                {
                    Destroy(child.gameObject);
                }
                SpawnNewItem(inventorySlots[indexA].inventoryItem.item, indexA); //a = originally B

            }

            if (inventoryItem != null)
                SpawnNewItem(inventorySlots[indexB].inventoryItem.item, indexB); // originally A


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

            //SpawnNewEquipment(equipmentSlots[equipIndex].inventoryItem.item, equipIndex); //spawn new equipment item (old inventory item)

        }

        if (equipmentSlots[equipIndex].transform.childCount > 0) // Destroy each child of the old equipment item
        {
            foreach (Transform child in equipmentSlots[equipIndex].transform)
            {
                Destroy(child.gameObject);
            }
            //SpawnNewEquipment(equipmentSlots[equipIndex].inventoryItem.item, equipmentSlots[equipIndex]); //a = originally B
            SpawnNewItem(inventorySlots[invIndex].inventoryItem.item, invIndex);

        }

        if (equipmentSlots[equipIndex].inventoryItem != null)
        {
            SpawnNewEquipment(equipmentSlots[equipIndex].inventoryItem.item, equipIndex); //spawn new equipment item (old inventory item)
            //SpawnNewItem(inventorySlots[invIndex].inventoryItem.item, invIndex);
           // Debug.Log("Item from inventory slot wasn't null");

        } 
        //else
        //    Debug.Log("Item from inventory slot IS null");

        HoldItemMainHand();


       //Debug.Log("Equipped " + equipmentSlots[equipIndex].inventoryItem.item.itemName + " from inventory slot #" + invIndex);

    }

    public void SwapToChest(int chestIndex, int invIndex, ChestInventory chest)
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

        if (chest.inventorySlots[chestIndex].transform.childCount > 0)
        {
            foreach (Transform child in chest.inventorySlots[chestIndex].transform)
            {
                Destroy(child.gameObject);
            }
        }

        if (inventorySlots[invIndex].transform.childCount > 0)
        {
            foreach (Transform child in inventorySlots[invIndex].transform)
            {
                Destroy(child.gameObject);
            }
            chest.SpawnNewItem(chest.inventorySlots[chestIndex].inventoryItem.item, chestIndex); //a = originally B

            if (chest.inventorySlots[chestIndex].inventoryItem != null)
                chest.inventory[chestIndex] = chest.inventorySlots[chestIndex].inventoryItem.item;
            else chest.inventory[chestIndex] = null;

        }

        if (inventoryItem != null)
            SpawnNewItem(inventorySlots[invIndex].inventoryItem.item, invIndex);


        HoldItemMainHand();
    }

    public void SwapFromChest(int invIndex, int chestIndex, ChestInventory chest)
    {
        Debug.Log("Storing in chest");

        // Store item info
        InventoryUIItem inventoryItem = inventorySlots[invIndex].inventoryItem;

        //Swap  item Info for equipment info
        inventorySlots[invIndex].inventoryItem = chest.inventorySlots[chestIndex].inventoryItem;

        // Swap equipment info for stored item info
        chest.inventorySlots[chestIndex].inventoryItem = inventoryItem;

        if (inventoryItem != null)
            RemoveModifiers(inventoryItem.item);

        EndItemInspection();

      

        if (inventorySlots[invIndex].transform.childCount > 0)
        {
            foreach (Transform child in inventorySlots[invIndex].transform)
            {
                Destroy(child.gameObject);
            }

        }

        if (chest.inventorySlots[chestIndex].transform.childCount > 0)
        {
            foreach (Transform child in chest.inventorySlots[chestIndex].transform)
            {
                Destroy(child.gameObject);
            }

            
            SpawnNewItem(inventorySlots[invIndex].inventoryItem.item, invIndex);
        }

        if (inventoryItem != null)
        {
            chest.SpawnNewItem(chest.inventorySlots[chestIndex].inventoryItem.item, chestIndex); //a = originally B
            chest.inventory[chestIndex] = chest.inventorySlots[chestIndex].inventoryItem.item;
        }
        else chest.inventory[chestIndex] = null;



        HoldItemMainHand();
    }
}
