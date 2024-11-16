using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TimeWeather;
using Unity.VisualScripting;

public class ShopInventory : Interactable
{
    // manage shop inventory UI

    //Buy items - check value & player money

    //Sell items - place for sale in shop inventory

    //Item pool

    //Randomise items after x time


    [HideInInspector] public ItemSlot[] inventorySlots;
    public List<InventoryUIItem> inventory; 
    public List<ProduceInInventory> produceItems;

    [HideInInspector] public ItemSlot[] buyBackInventorySlots;
    public List<InventoryUIItem> buyBackInventory;

    public List<InventoryItem> constantShopItems;
    [field:ReadOnlyField] public int currentSeasonalItems;
    public List<SeasonalItem> seasonalShopItems;
    public int extraItemsToAdd = 5;
    public List<InventoryItem> possibleShopItems;
    [field: ReadOnlyField] public List<InventoryItem> dailyInventory;

    //public List<InventoryUIItem> inventoryUI;
    public GameObject shopPanel;
    [field: ReadOnlyField] public int numOfPages;
    [field: ReadOnlyField] public int itemsOnPageCount;
    [field: ReadOnlyField] public int startingItemCatalogueIndex;
    public int currentPage;
    public GameObject buyBackPanel;
    public bool buyBackPanelOpen;
    public Button changePageButton;

    private ThirdPersonCam thirdPersonCam;
    private PlayerController playerController;
    private PlayerInventory playerInventory;
    private TimeController timeController;

    public bool isCurrentShop;

    public ItemType[] typesWillBuy;

    private TimeController.Day currentDay;

    [SerializeField] private float timeSinceOpened;

    [System.Serializable]
    public class SeasonalItem
    {
        public List<string> requiredSeason;

        public InventoryItem item;
    }

    private void Start()
    {
        thirdPersonCam = FindObjectOfType<ThirdPersonCam>();
        playerController = FindObjectOfType<PlayerController>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        timeController = TimeController.instance;


        if (playerInventory != null)
        {
            shopPanel = playerInventory.shopPanel;

            inventorySlots = new ItemSlot[shopPanel.transform.childCount];

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventory.Add(null);
            }

            for (int i = 0; i < shopPanel.transform.childCount; i++)
            {
                if (i < shopPanel.transform.childCount)
                {
                    inventorySlots[i] = shopPanel.transform.GetChild(i).GetComponent<ItemSlot>();
                    inventorySlots[i].slot = i;
                }
            }

           

            buyBackPanel = playerInventory.buyBackPanel;
            buyBackInventorySlots = new ItemSlot[buyBackPanel.transform.childCount];
            for (int i = 0; i < buyBackInventorySlots.Length; i++)
            {
                buyBackInventory.Add(null);
                produceItems.Add(null);
            }
            for (int i = 0; i < buyBackPanel.transform.childCount; i++)
            {
                if (i < buyBackPanel.transform.childCount)
                {
                    buyBackInventorySlots[i] = buyBackPanel.transform.GetChild(i).GetComponent<ItemSlot>();
                    buyBackInventorySlots[i].slot = i;
                }
            }
        }

        currentDay =timeController.currentDay;



        ResetShop();


        
    }

    private void Update()
    {
        if (isCurrentShop)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] != null && inventory[i].shop != this) inventory[i].shop = this;
            }

            if (playerInventory.currentShop != this) playerInventory.currentShop = this;

            if (Vector3.Distance(transform.position, playerController.transform.position) > radius)
                CloseShop();

            if (Input.GetKeyDown(KeyCode.Tilde) && numOfPages > 1)
                ChangeShopPage();
        }

        if (currentDay != timeController.currentDay)
        {
            ResetShop();

            currentDay = timeController.currentDay;
        }

        if (produceItems != null && produceItems.Count > 0 && (!isCurrentShop || !buyBackPanelOpen))
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

    public void ResetShop()
    {
        if (possibleShopItems.Count < extraItemsToAdd) extraItemsToAdd = possibleShopItems.Count;

        currentSeasonalItems = 0;
        for (int i = 0; i < seasonalShopItems.Count; i++)
        {
            if (seasonalShopItems[i].requiredSeason.Contains(timeController.currentMonthData.season))
                currentSeasonalItems += 1;

        }

        //if (constantShopItems.Count + currentSeasonalItems + extraItemsToAdd > inventorySlots.Length)
        //{

        SetDailyItems();

        if ((dailyInventory.Count - 1) / inventorySlots.Length % 1 != 0)
        {
            numOfPages = (dailyInventory.Count - 1) / inventorySlots.Length + 1;
        }
        else
        {
            numOfPages = (dailyInventory.Count - 1) / inventorySlots.Length;
        }

            SetShopItems(currentPage);
            //set number of pages
            //set shop items for first page
            //enable page change UI
        //}
        //else
        //{
        //    SetShopItems();
        //    //disable change page UI
        //}
    }

    public void SetDailyItems()
    {
        dailyInventory = new List<InventoryItem>();

        for (int i = 0; i < constantShopItems.Count; i++)
        {
            dailyInventory.Add(constantShopItems[i]);
        }

        for (int i = 0; i < seasonalShopItems.Count; i++)
        {
            if (seasonalShopItems[i].requiredSeason.Contains(timeController.currentMonthData.season) && !dailyInventory.Contains(seasonalShopItems[i].item))
            {
                dailyInventory.Add(seasonalShopItems[i].item);

            }
        }

        if (extraItemsToAdd > 0 && possibleShopItems.Count > 0)
            for (int i = 0; i <= extraItemsToAdd; i++)
            {
                int r = Random.Range(0, possibleShopItems.Count - 1);

                while (dailyInventory.Contains(possibleShopItems[r]))
                    r = Random.Range(0, possibleShopItems.Count - 1);

                if (!dailyInventory.Contains(possibleShopItems[r])) // make sure the item isnt already in the shop
                {
                    dailyInventory.Add(possibleShopItems[r]);
                }
            }
    }

    public void SetShopItems()
    {
        for (int i = 0; i < inventorySlots.Length; i++) // destroy existing items
        {
            if (inventorySlots[i].transform.childCount > 0)
            {
                foreach (Transform c in inventorySlots[i].transform)
                {
                    Destroy(c.gameObject);
                }
            }
        }

        for(int i = 0;i < dailyInventory.Count; i++)
        {
            playerInventory.SpawnNewItem(dailyInventory[i], i, inventory, inventorySlots, null, this);

        }
    }

    public void ChangeShopPage()
    {
        if (currentPage < numOfPages)
            currentPage++;
        else currentPage = 0;


        SetShopItems(currentPage);
    }

    public void SetShopItems(int page)
    {
        for (int i = 0; i < inventorySlots.Length; i++) // destroy existing items
        {
            if (inventorySlots[i].transform.childCount > 0)
            {
                foreach (Transform c in inventorySlots[i].transform)
                {
                    Destroy(c.gameObject);
                }
            }
        }
        if ((dailyInventory.Count - 1) - inventorySlots.Length * page < inventorySlots.Length) //if there are no more items past this page
        {
            itemsOnPageCount = (dailyInventory.Count) - inventorySlots.Length * page; 
        }
        else
        {
            itemsOnPageCount = inventorySlots.Length; 
        }

        startingItemCatalogueIndex = (dailyInventory.Count - 1) - ((dailyInventory.Count - 1) - inventorySlots.Length * page);

        for (int i = 0; i < itemsOnPageCount; i++)
        {
            playerInventory.SpawnNewItem(dailyInventory[i + startingItemCatalogueIndex], i, inventory, inventorySlots, null, this);
        }
    }

    public int GetNewItemIndex()
    {
        int r = Random.Range(0, possibleShopItems.Count - 1);

        while (CheckForItem(possibleShopItems[r]))
        {
            r = Random.Range(0, possibleShopItems.Count - 1);
        }

        return r;
    }
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        buyBackPanel.SetActive(false);
        buyBackPanelOpen = false;
        SaveInventory();
        playerInventory.shopParent.SetActive(false);
        playerInventory.CloseInventoryWindow();
        playerInventory.inventoryWindowOpen = false;
        isCurrentShop = false;
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

    public bool CheckForItem(InventoryItem item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null)
                if (inventory[i].item == item) return true;
        }

        return false;
    }
    public void InitialiseShopPanel()
    {
        for (int i = 0; i < shopPanel.transform.childCount; i++)
        {
            if (i < shopPanel.transform.childCount)
            {
                inventorySlots[i] = shopPanel.transform.GetChild(i).GetComponent<ItemSlot>();
                inventorySlots[i].slot = i;
                inventorySlots[i].shop = this;

                if (inventorySlots[i].transform.childCount > 0)
                {
                    foreach (Transform c in inventorySlots[i].transform)
                    {
                        Destroy(c.gameObject);
                    }
                }
            }
        }

        for (int i = 0; i < buyBackPanel.transform.childCount; i++)
        {
            if (i < buyBackPanel.transform.childCount)
            {
                buyBackInventorySlots[i] = buyBackPanel.transform.GetChild(i).GetComponent<ItemSlot>();
                buyBackInventorySlots[i].slot = i;
                buyBackInventorySlots[i].shop = this;

                if (buyBackInventorySlots[i].transform.childCount > 0)
                {
                    foreach (Transform c in buyBackInventorySlots[i].transform)
                    {
                        Destroy(c.gameObject);
                    }
                }

                if (produceItems[i] != null)
                {
                    playerInventory.SpawnUsedItem(produceItems[i].produceItem, i, buyBackInventory, buyBackInventorySlots, null, this);
                    
                    ProduceInInventory prodInInv = buyBackInventory[i].AddComponent<ProduceInInventory>();

                    prodInInv.produceItem = buyBackInventory[i];



                    if (prodInInv.produceItem.item.isStackable)
                    {
                        for (int j = 0; j < produceItems[i].produceAgesInStack.Count; j++)
                        {
                            if (j == 0)
                            {
                                prodInInv.InitaliseInventoryProduce(buyBackInventory[i], produceItems[i], j);
                            }

                            prodInInv.produceAgesInStack.Add(produceItems[i].produceAgesInStack[j]);

                        }
                    }
                    else
                    {
                        prodInInv.InitaliseInventoryProduce(buyBackInventory[i], produceItems[i], 0);

                            prodInInv.produceAgesInStack.Add(produceItems[i].produceAgesInStack[0]);
                    }

                    GameObject toDestroy = produceItems[i].gameObject;

                    produceItems[i] = prodInInv;

                    Destroy(toDestroy);

                }
                else if (buyBackInventory[i] != null)
                {
                    playerInventory.SpawnUsedItem(buyBackInventory[i], i, buyBackInventory, buyBackInventorySlots, null, this);
                }
            }
        }

        isCurrentShop = true;

        if (numOfPages == 0) changePageButton.gameObject.SetActive(false);
        else
        {
            changePageButton.onClick.RemoveAllListeners();
            changePageButton.onClick.AddListener(() => ChangeShopPage());
            changePageButton.gameObject.SetActive(true);
        }

        SetShopItems(0);

        playerInventory.shopParent.SetActive(true);

        shopPanel.SetActive(true);

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
        for (int i = 0; i < buyBackInventory.Count; i++)
        {
            if (buyBackInventorySlots[i].inventoryItem != null)
            {
                buyBackInventory[i] = buyBackInventorySlots[i].inventoryItem;


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
                                prodInInv.InitaliseInventoryProduce(buyBackInventory[i], produceItems[i], j);
                            }
                            prodInInv.produceAgesInStack.Add(produceItems[i].produceAgesInStack[j]);

                        }
                    }
                    else
                    {
                        prodInInv.InitaliseInventoryProduce(buyBackInventory[i], produceItems[i], 0);

                        prodInInv.produceAgesInStack.Add(produceItems[i].produceAgesInStack[0]);
                    }

                    produceItems[i] = prodInInv;

                    InventoryUIItem savedProdItem = savedProdInv.AddComponent<InventoryUIItem>();
                    playerInventory.CopyItemVariables(buyBackInventory[i], savedProdItem, i);

                    savedProdItem.InitialiseItem(buyBackInventory[i].item, i, savedProdItem.numCarried, null, this);

                    prodInInv.produceItem = savedProdItem;
                }
            }
            else buyBackInventory[i] = null;
        }
    }

    public void SwapItemSlot(int indexA, int indexB)
    {

        // Store emote A info
        InventoryUIItem inventoryItem = inventorySlots[indexA].inventoryItem;

        ProduceInInventory produceB = null;
        if (inventorySlots[indexA].inventoryItem.GetComponent<ProduceInInventory>())
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
            //SpawnNewEmoteUI(inventorySlots[indexB].inventoryItem.emote, indexB);

        }

        if (inventorySlots[indexB].transform.childCount > 0)
        {
            foreach (Transform child in inventorySlots[indexB].transform)
            {
                Destroy(child.gameObject);
            }
            if (inventorySlots[indexA].inventoryItem != null && (inventorySlots[indexA].inventoryItem.numCarried > 1 || inventorySlots[indexA].inventoryItem.isInUse))
            {
                playerInventory.SpawnUsedItem(inventorySlots[indexA].inventoryItem, indexA, inventory, inventorySlots); //a = originally B
            }
            else
            {
                playerInventory.SpawnNewItem(inventorySlots[indexA].inventoryItem.item, indexA, inventory, inventorySlots); //a = originally B

            }
            inventorySlots[indexA].inventoryItem.shop = inventorySlots[indexA].shop;

        } //else Debug.Log(indexB + " Slot Empty");

        if (inventoryItem != null)
        {
            if (inventorySlots[indexB].inventoryItem.numCarried > 1 || inventorySlots[indexB].inventoryItem.isInUse)
            {
                playerInventory.SpawnUsedItem(inventorySlots[indexB].inventoryItem, indexB, inventory, inventorySlots); // originally A
            }
            else
            {
                playerInventory.SpawnNewItem(inventorySlots[indexB].inventoryItem.item, indexB, inventory, inventorySlots); // originally A

            }
            inventorySlots[indexB].inventoryItem.shop = inventorySlots[indexB].shop;
        }

        Debug.Log("Swapped " + indexA + " UI Slot with " + indexB + " UI Slot");
    }


    public override void Interact()
    {
        base.Interact();

        shopPanel.SetActive(true);

        //thirdPersonCam.freezeCameraRotation = true;
        Pause.instance.freezeCameraRotation = true;
        Pause.instance.unlockCursor = true;
    }
}
