using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChestInventory : Interactable
{
    public ItemSlot[] inventorySlots;

    public List<InventoryUIItem> inventory;
    public List<ProduceInInventory> produceItems;
    //public List<InventoryUIItem> uiInventory;
    //public List<InventoryUIItem> inventoryUI;
    public GameObject chestPanel;

    private ThirdPersonCam thirdPersonCam;
    private PlayerController playerController;
    private PlayerInventory playerInventory;

    public bool isCurrentChest;

    [field: ReadOnlyField] public float timeSinceOpened;

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
                produceItems.Add(null);
                //uiInventory.Add(null);
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
        if (isCurrentChest)
        {
            if (playerInventory.currentChest != this) playerInventory.currentChest = this;

            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] != null && inventory[i].chest != this) inventory[i].chest = this;
            }

            if (Vector3.Distance(transform.position, playerController.transform.position) > radius)
            {
                CloseChest();
            }
        }

        if (produceItems != null && produceItems.Count > 0 && (!isCurrentChest || !chestPanel.activeSelf))
        {
            timeSinceOpened += Time.deltaTime;

        }
        else if (timeSinceOpened > 0)
        {
            for (int i = 0; i < produceItems.Count; i++)
            {
                if (produceItems[i] != null)
                {
                    for (int j = 0; j < produceItems[i].produceAgesInStack.Count; j++)
                        produceItems[i].produceAgesInStack[j] += timeSinceOpened * produceItems[i].growthSpeed;
                }
            }

            timeSinceOpened = 0;
        }
    }

    public void CloseChest()
    {
        chestPanel.SetActive(false);
        SaveInventory();
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
                    foreach (Transform c in inventorySlots[i].transform)
                    {
                        Destroy(c.gameObject);
                    }
                }

                if (produceItems[i] != null)
                {
                    playerInventory.SpawnUsedItem(produceItems[i].produceItem, i, inventory, inventorySlots, this);

                    ProduceInInventory prodInInv = inventory[i].AddComponent<ProduceInInventory>();

                    prodInInv.produceItem = inventory[i];


                    for (int j = 0; j < produceItems[i].produceAgesInStack.Count; j++)
                    {
                        if (j == 0)
                        {
                            prodInInv.InitaliseInventoryProduce(inventory[i], produceItems[i], j);
                        }

                        prodInInv.produceAgesInStack.Add(produceItems[i].produceAgesInStack[j]);

                    }

                    GameObject toDestroy = produceItems[i].gameObject;

                    produceItems[i] = prodInInv;

                    Destroy(toDestroy);

                }
                else if (inventory[i] != null)
                {
                    playerInventory.SpawnUsedItem(inventory[i], i, inventory, inventorySlots, this);
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

    public void SaveInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventorySlots[i].inventoryItem != null)
            {
                inventory[i] = inventorySlots[i].inventoryItem;


                if (produceItems[i] != null)
                {
                    GameObject savedProdInv = new GameObject(produceItems[i].produceItem.item.itemName + " Saved Produce Info");
                    savedProdInv.transform.parent = transform;
                    ProduceInInventory prodInInv = savedProdInv.AddComponent<ProduceInInventory>();


                    if (prodInInv.produceItem.item.isStackable)
                    {
                        for (int j = 0; j < produceItems[i].produceAgesInStack.Count; j++)
                        {
                            if (j == 0)
                            {
                                prodInInv.InitaliseInventoryProduce(inventory[i], produceItems[i], j);
                            }
                            prodInInv.produceAgesInStack.Add(produceItems[i].produceAgesInStack[j]);

                        }
                    }
                    else
                    {
                        prodInInv.InitaliseInventoryProduce(inventory[i], produceItems[i], 0);

                        prodInInv.produceAgesInStack.Add(produceItems[i].produceAgesInStack[0]);
                    }

                    produceItems[i] = prodInInv;

                    InventoryUIItem savedProdItem = savedProdInv.AddComponent<InventoryUIItem>();
                    playerInventory.CopyItemVariables(inventory[i], savedProdItem, i);

                    savedProdItem.InitialiseItem(inventory[i].item, i, savedProdItem.numCarried, this);

                    prodInInv.produceItem = savedProdItem;

                }
            }
            else inventory[i] = null;
        }

      
    }
    public void SwapItemSlot(int indexA, int indexB)
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

        inventory[indexA] = inventory[indexB];

        // Swap slot B info for stored A info
        inventorySlots[indexB].inventoryItem = inventoryItem;

        if (inventoryItem != null)
        {
            inventory[indexB] = inventoryItem;
            produceItems[indexB] = produceB;
        }
        else inventory[indexB] = null;


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


            playerInventory.SpawnUsedItem(inventorySlots[indexA].inventoryItem, indexA, inventory, inventorySlots); //a = originally B

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

            playerInventory.SpawnUsedItem(inventorySlots[indexB].inventoryItem, indexB, inventory, inventorySlots); // originally A


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

        Debug.Log("Swapped " + indexA + " UI Slot with " + indexB + " UI Slot");
    }
        

    

    public override void Interact()
    {
        base.Interact();

        chestPanel.SetActive(true);

        //thirdPersonCam.freezeCameraRotation = true;
        Pause.instance.freezeCameraRotation = true;
            Pause.instance.unlockCursor = true;
    }
}
