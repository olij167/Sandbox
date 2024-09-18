using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInventory : Interactable
{
    [HideInInspector] public ItemSlot[] inventorySlots;
    public List<InventoryItem> inventory;
    //public List<InventoryUIItem> inventoryUI;
    public GameObject chestPanel;

    private ThirdPersonCam thirdPersonCam;
    private PlayerController playerController;
    private PlayerInventory playerInventory;

    public bool isCurrentChest;

    private void Awake()
    {
        thirdPersonCam = FindObjectOfType<ThirdPersonCam>();
        playerController = FindObjectOfType<PlayerController>();
        playerInventory = FindObjectOfType<PlayerInventory>();

        if (playerInventory != null)
        {
            chestPanel = playerInventory.chestPanel;

            inventorySlots = new ItemSlot[chestPanel.transform.childCount];

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventory.Add(null);
            }

            for (int i = 0; i < chestPanel.transform.childCount; i++)
            {
                if (i < chestPanel.transform.childCount)
                {
                    inventorySlots[i] = chestPanel.transform.GetChild(i).GetComponent<ItemSlot>();
                    inventorySlots[i].slot = i;

                }
            }
        }
    }

    private void Update()
    {
        if (isCurrentChest && Vector3.Distance(transform.position, playerController.transform.position) > radius)
        {
            CloseChest();
        }
    }

    public void CloseChest()
    {
        chestPanel.SetActive(false);
        //SaveInventory();
        playerInventory.CloseInventoryWindow();
        playerInventory.inventoryWindowOpen = false;
        isCurrentChest = false;
    }

    public void InitialiseChestPanel()
    {
        for (int i = 0; i < chestPanel.transform.childCount; i++)
        {
            if (i < chestPanel.transform.childCount)
            {
                inventorySlots[i] = chestPanel.transform.GetChild(i).GetComponent<ItemSlot>();
                inventorySlots[i].slot = i;
                inventorySlots[i].chest = this;

                if (inventorySlots[i].transform.childCount > 0)
                {
                    foreach(Transform c in inventorySlots[i].transform)
                    {
                        Destroy(c.gameObject);
                    }
                }

                if (inventory[i] != null)
                {
                    SpawnNewItem(inventory[i], i);
                }
            }
        }

        isCurrentChest = true;

        chestPanel.SetActive(true);

        if (!playerInventory.inventoryBarPanel.activeSelf)
        {
            playerInventory.OpenInventoryBar();
        }

        if (!playerInventory.inventoryWindowOpen)
        {
            playerInventory.inventoryWindowOpen = true;
            playerInventory.OpenInventoryWindow();
        }
    }

    public void SpawnNewItem(InventoryItem item, int itemSlot)
    {
        GameObject newItemUI = Instantiate(playerInventory.inventoryItemPrefab, inventorySlots[itemSlot].transform);
        InventoryUIItem inventoryItem = newItemUI.GetComponent<InventoryUIItem>();
        inventoryItem.isEquipped = false;
        inventorySlots[itemSlot].inventoryItem = inventoryItem;

        //Button button = newItemUI.AddComponent<Button>();
        //button.onClick.AddListener(() => playerInventory.SelectInventoryItemAsButton(itemSlot));

        inventoryItem.InitialiseItem(item, itemSlot, this);

        //inventoryUI[itemSlot] = inventoryItem;
        inventory[itemSlot] = item;

    }

    public void SaveInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventorySlots[i].inventoryItem != null)
                inventory[i] = inventorySlots[i].inventoryItem.item;
            else inventory[i] = null;
        }
    }

    public void SwapItemSlot(int indexA, int indexB)
    {

        // Store emote A info
        InventoryUIItem inventoryItem = inventorySlots[indexA].inventoryItem;

        //Swap Slot A Info for slot B
        inventorySlots[indexA].inventoryItem = inventorySlots[indexB].inventoryItem;


        inventory[indexA] = inventory[indexB];

        // Swap slot B info for stored A info
        inventorySlots[indexB].inventoryItem = inventoryItem;

        if (inventoryItem != null)
            inventory[indexB] = inventoryItem.item;
        else inventory[indexB] = null;


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
                                                                             //inventory[indexA] = inventorySlots[indexA].inventoryItem.item;

        }

        if (inventoryItem != null)
        {
            SpawnNewItem(inventorySlots[indexB].inventoryItem.item, indexB); // originally A
            //inventory[indexB] = inventorySlots[indexB].inventoryItem.item;

        }

        Debug.Log("Swapped " + indexA + " UI Slot with " + indexB + " UI Slot");
    }
        

    

    public override void Interact()
    {
        base.Interact();

        chestPanel.SetActive(true);

        thirdPersonCam.freezeCameraRotation = true;
    }
}
