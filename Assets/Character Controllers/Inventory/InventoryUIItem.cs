using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting;

public class InventoryUIItem : MonoBehaviour, IPointerDownHandler, IDragHandler, /*IDropHandler,*/ IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{

    [HideInInspector]public PlayerInventory inventory;

    public InventoryItem item;
    public Image image;
    public float itemValue;
    //public SkinnedMeshRenderer mesh;

    public TextMeshProUGUI stackCountText;
    public int numCarried;

    public TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescText;
    public TextMeshProUGUI itemValueText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private GameObject infoPanel;

    [SerializeField] private GameObject rightClickMenuPanel;
    [SerializeField] private Button rightClickMenuButton;

    private RightClickMenu rightClickMenu;

    public bool isInUse;

    public float batteryCharge;
    public float ammo;

    public GameObject physicalItem;

    public bool isEquipped;
    public int slot;

    //public bool fromChest;
    public ChestInventory chest;
    public ShopInventory shop;

    [SerializeField] private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startAnchoredPos;

    public bool isMousedOver;
    public bool isSavedCopy;

    public float healthEffect;
    public float staminaEffect;
    public float healthModifier;
    public float staminaModifier;
    public float oxygenModifier;

    public float speedModifier;
    public float jumpModifier;

    public float armourModifier;
    public float attackDamageModifier;
    public float passiveDamageModifier;
    public float knockBackModifier;
    public float attackSpeedModifier;



    void Awake()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        if (GetComponent<RectTransform>() == null) isSavedCopy = true;

        if (!isSavedCopy)
        {

            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            rightClickMenu = FindObjectOfType<RightClickMenu>();

            Transform testCanvasTransform = transform;
            do
            {
                testCanvasTransform = testCanvasTransform.parent;
                canvas = testCanvasTransform.GetComponent<Canvas>();
            }
            while (canvas == null);
        }

    }

    private void Start()
    {
        if (!isSavedCopy)
            startAnchoredPos = rectTransform.anchoredPosition;
    }

    public void InitialiseItem(InventoryItem newItem, int newSlot, int stackCount = 1, ChestInventory chestInventory = null, ShopInventory shopInventory = null)
    {
        item = newItem;
        if (image != null)
            image.sprite = newItem.itemIcon;
        numCarried = stackCount;
        itemValue = newItem.itemValue;
        if (stackCountText != null)
        {
            if (item.isStackable && numCarried > 1)
            {
                stackCountText.text = numCarried.ToString();
            }
            else stackCountText.text = null;
        }

        if (item.canSpoil)
        {
            // use battery slider to show spoilage level
        }
        else if (item.usesBatteries)
        {
            batteryCharge = item.maxBatteryCharge;
        }

        if (item.isProjectile)
        {
            ammo = item.maxAmmo;
        }

        slot = newSlot;

        chest = chestInventory;
        shop = shopInventory;

        SetItemStats();

        if (infoPanel != null)
        {
            SetItemInfoUI();

            infoPanel.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        //transform.GetComponent<LayoutElement>().ignoreLayout = true;
        //transform.SetParent(transform.parent.parent);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        //transform.GetComponent<LayoutElement>().ignoreLayout = false;

        rectTransform.anchoredPosition = startAnchoredPos;

        SetItemInfoUI();
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Output to console the GameObject's name and the following message
        //Debug.Log("Cursor Entering " + name + " GameObject");
        isMousedOver = true;
        rightClickMenu.itemIsMousedOver = true;
        ToggleItemInfoPanel(true);

      
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Output the following message with the GameObject's name
        //Debug.Log("Cursor Exiting " + name + " GameObject");
        isMousedOver = false;
        rightClickMenu.itemIsMousedOver = false;
        ToggleItemInfoPanel(false);
        //ToggleRightClickMenu(false);
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        //Output the name of the GameObject that is being clicked
        //Debug.Log(name + "Game Object Click in Progress");
        if (Input.GetMouseButtonDown(1))
        {
            ToggleRightClickMenu(true);
            rightClickMenuPanel.transform.SetAsLastSibling();
        }
    }

    public int GetItemIndex()
    {
        return slot;
    }

    public void ToggleItemInfoPanel(bool openPanel)
    {
        if (openPanel)
        {
            SetItemInfoUI();
            infoPanel.SetActive(true);
        }
        else infoPanel.SetActive(false);
    }

    public void SetItemStats()
    {
        healthEffect = item.healthEffect;
        staminaEffect = item.staminaEffect;
        healthModifier = item.healthModifier;
        staminaModifier = item.staminaModifier;
        oxygenModifier = item.oxygenModifier;
        speedModifier = item.speedModifier;
        jumpModifier = item.jumpModifier;
        armourModifier = item.armourModifier;
        attackDamageModifier = item.attackDamageModifier;
        attackSpeedModifier = item.attackSpeedModifier;
        passiveDamageModifier = item.passiveDamageModifier;
        knockBackModifier = item.knockBackModifier;
    }

    public void SetItemInfoUI()
    {
        if (GetComponent<ProduceInInventory>())
        {
            itemNameText.text = GetComponent<ProduceInInventory>().stackQuality.ToString() + " " + item.itemName;
        }
        else
            itemNameText.text = item.itemName;

        if (item.itemDescription == null || item.itemDescription == "")
            itemDescText.gameObject.SetActive(false);
        else
        {
            itemDescText.gameObject.SetActive(true);
            itemDescText.text = item.itemDescription;
        }

        itemValueText.text = "$" + itemValue;

        foreach (Transform c in infoPanel.transform)
        {
            if (c.name.Contains("Stat"))
                Destroy(c.gameObject);
        }

        //if (numCarried > 1) itemValueText.text += "\n(x" + numCarried + " = $" + item.itemValue * numCarried +")";

        if (healthEffect != 0)
        {
            //if (statsText.text != null) statsText.text += "\n";
            TextMeshProUGUI newStatsText = Instantiate(statsText, infoPanel.transform);

            newStatsText.text = "Health Effect: " + healthEffect;
        }

        if (staminaEffect != 0)
        {
            TextMeshProUGUI newStatsText = Instantiate(statsText, infoPanel.transform);

            newStatsText.text = "Stamina Effect: " + staminaEffect;
        }

        if (healthModifier != 0)
        {
            TextMeshProUGUI newStatsText = Instantiate(statsText, infoPanel.transform);

            newStatsText.text = "Health: " + healthModifier;
        }

        if (staminaModifier != 0)
        {
            TextMeshProUGUI newStatsText = Instantiate(statsText, infoPanel.transform);

            newStatsText.text = "Stamina: " + staminaModifier;
        }

        if (oxygenModifier != 0)
        {
            TextMeshProUGUI newStatsText = Instantiate(statsText, infoPanel.transform);

            newStatsText.text = "Oxygen: " + oxygenModifier;
        }

        if (speedModifier != 0)
        {
            TextMeshProUGUI newStatsText = Instantiate(statsText, infoPanel.transform);

            newStatsText.text = "Speed: " + speedModifier;
        }

        if (jumpModifier != 0)
        {
            TextMeshProUGUI newStatsText = Instantiate(statsText, infoPanel.transform);

            newStatsText.text = "Jump: " + jumpModifier;
        }

        if (armourModifier != 0)
        {
            TextMeshProUGUI newStatsText = Instantiate(statsText, infoPanel.transform);

            newStatsText.text = "Armour: " + armourModifier;
        }

        if (attackDamageModifier != 0)
        {
            TextMeshProUGUI newStatsText = Instantiate(statsText, infoPanel.transform);

            newStatsText.text = "Attack Damage: " + attackDamageModifier;
        }

        if (passiveDamageModifier != 0)
        {
            TextMeshProUGUI newStatsText = Instantiate(statsText, infoPanel.transform);

            newStatsText.text = "Passive Damage: " + passiveDamageModifier;
        }

        if (attackSpeedModifier != 0)
        {
            TextMeshProUGUI newStatsText = Instantiate(statsText, infoPanel.transform);

            newStatsText.text = "Attack Speed: " + attackSpeedModifier;
        }

        if (knockBackModifier != 0)
        {
            TextMeshProUGUI newStatsText = Instantiate(statsText, infoPanel.transform);

            newStatsText.text = "Knockback: " + knockBackModifier;
        }

    }

    public void ToggleRightClickMenu(bool openPanel)
    {
        if (openPanel)
        {
            SetRightClickMenu();
            rightClickMenuPanel.SetActive(true);
        }
        else
        {
            rightClickMenu.DestroyMenu();
            //rightClickMenuPanel.SetActive(false);
        }
    }

    public void SetRightClickMenu()
    {
        rightClickMenu.CreateMenu();

        rightClickMenuPanel = rightClickMenu.currentPanel;

        foreach(Transform c in rightClickMenuPanel.transform)
        {
            Destroy(c.gameObject);
        }

        if (chest != null)
        {
            if (inventory.CheckEmptySlots(inventory.inventorySlots) > 0)
            {
                if (item.isStackable && item.maxNumCarried > 1)
                {
                    if (numCarried > 1)
                    {
                        Button takeOneButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);
                        takeOneButton.onClick.AddListener(() => inventory.SwapFromChest(inventory.GetFirstEmptySlot(inventory.inventorySlots), slot, chest, 1));
                        takeOneButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                        takeOneButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Take 1 " + item.itemName;

                        if (numCarried / 2 > 1)
                        {
                            Button takeHalfButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);
                            takeHalfButton.onClick.AddListener(() => inventory.SwapFromChest(inventory.GetFirstEmptySlot(inventory.inventorySlots), slot, chest, numCarried / 2));
                            takeHalfButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                            takeHalfButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Take " + numCarried / 2;
                        }
                    }

                    Button takeAllButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);
                    takeAllButton.onClick.AddListener(() => inventory.SwapFromChest(inventory.GetFirstEmptySlot(inventory.inventorySlots), slot, chest, numCarried));
                    takeAllButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                    takeAllButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Take " + numCarried;
                }
                else Debug.Log("Inventory is full");
            }
            else
            {
                Button takeButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);
                takeButton.onClick.AddListener(() => inventory.SwapFromChest(inventory.GetFirstEmptySlot(inventory.inventorySlots), slot, chest, 1));
                takeButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                takeButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Take " + item.itemName;
            }
        }
        else if (inventory.chestPanel.activeSelf)
        {
            if (inventory.CheckEmptySlots(inventory.inventorySlots) > 0)
            {
                if (numCarried > 1)
                {
                    Button stashOneButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);
                    stashOneButton.onClick.AddListener(() => inventory.SwapToChest(inventory.GetFirstEmptySlot(inventory.currentChest.inventorySlots), slot, inventory.currentChest, 1));
                    stashOneButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                    stashOneButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Stash 1 " + item.itemName;

                    if (numCarried / 2 > 1)
                    {
                        Button stashHalfButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);
                        stashHalfButton.onClick.AddListener(() => inventory.SwapToChest(inventory.GetFirstEmptySlot(inventory.currentChest.inventorySlots), slot, inventory.currentChest, numCarried / 2));
                        stashHalfButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                        stashHalfButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Stash " + numCarried / 2;
                    }
                }

                Button stashAllButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);
                stashAllButton.onClick.AddListener(() => inventory.SwapToChest(inventory.GetFirstEmptySlot(inventory.currentChest.inventorySlots), slot, inventory.currentChest, numCarried));
                stashAllButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                stashAllButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Stash " + numCarried;

            }
        }

        if (shop != null)
        {
            if (inventory.CheckEmptySlots(inventory.inventorySlots) > 0)
            {
                int invIndex = inventory.CheckInventoryForItem(item) && item.isStackable && numCarried < item.maxNumCarried ? inventory.GetItemIndex(item) : inventory.GetFirstEmptySlot(inventory.inventorySlots);

                ItemSlot[] slots = shop.buyBackPanelOpen ? shop.buyBackInventorySlots : shop.inventorySlots;
                List<InventoryUIItem> inv = shop.buyBackPanelOpen ? shop.buyBackInventory : shop.inventory;

                ProduceInInventory produceInInventory = null;

                if (GetComponent<ProduceInInventory>()) produceInInventory = GetComponent<ProduceInInventory>();

                Button buyOneButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);

                buyOneButton.onClick.AddListener(() => inventory.Buy(inventory.GetFirstEmptySlot(inventory.currentShop.buyBackInventorySlots), slot, inventory.currentShop, slots, inv, 1, inventory.currentShop.buyBackPanelOpen ? true : false, produceInInventory));
                buyOneButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                buyOneButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Buy 1 " + item.itemName + " [$" + itemValue + "]";

                if (item.isStackable)
                {
                    if (item.maxNumCarried > 1)
                    {

                        if (!inventory.currentShop.buyBackPanelOpen)
                        {

                            Button buyHalfStackButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);

                            buyHalfStackButton.onClick.AddListener(() => inventory.Buy(inventory.GetFirstEmptySlot(inventory.currentShop.buyBackInventorySlots), slot, inventory.currentShop, slots, inv, item.maxNumCarried / 2, inventory.currentShop.buyBackPanelOpen ? true : false, produceInInventory));
                            buyHalfStackButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                            buyHalfStackButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Buy " + (item.maxNumCarried / 2) + " [$" + (itemValue * (item.maxNumCarried / 2)) + "]";

                            Button buyStackButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);

                            buyStackButton.onClick.AddListener(() => inventory.Buy(inventory.GetFirstEmptySlot(inventory.currentShop.buyBackInventorySlots), slot, inventory.currentShop, slots, inv, item.maxNumCarried, inventory.currentShop.buyBackPanelOpen ? true : false, produceInInventory));
                            buyStackButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                            buyStackButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Buy " + item.maxNumCarried + " [$" + (itemValue * item.maxNumCarried) + "]";
                        }
                        else
                        {
                            if (numCarried > 1)
                            {
                                Button buyHalfButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);

                                buyHalfButton.onClick.AddListener(() => inventory.Buy(inventory.GetFirstEmptySlot(inventory.currentShop.buyBackInventorySlots), slot, inventory.currentShop, slots, inv, numCarried / 2, inventory.currentShop.buyBackPanelOpen ? true : false, produceInInventory));
                                buyHalfButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                                buyHalfButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Buy " + (numCarried / 2) + " [$" + (itemValue * (numCarried / 2)) + "]";

                                Button buyAllButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);

                                buyAllButton.onClick.AddListener(() => inventory.Buy(inventory.GetFirstEmptySlot(inventory.currentShop.buyBackInventorySlots), slot, inventory.currentShop, slots, inv, numCarried, inventory.currentShop.buyBackPanelOpen ? true : false, produceInInventory));
                                buyAllButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                                buyAllButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Buy " + numCarried + " [$" + (itemValue * numCarried) + "]";
                            }
                        }
                    }
                }

            }
            else Debug.Log("Inventory is full");
        }
        else if (inventory.shopParent.activeSelf)
        {
            //inventory.cam.freezeCameraRotation = true;
            Pause.instance.freezeCameraRotation = true;
            Pause.instance.unlockCursor = true;

            if (inventory.currentShop.typesWillBuy != null && inventory.currentShop.typesWillBuy.Length > 0) // check whether the shop will buy the item before generating the sell button
            {
                for (int i = 0; i < inventory.currentShop.typesWillBuy.Length; i++)
                {
                    if (inventory.currentShop.typesWillBuy[i] == item.itemType)
                    {
                        int sellSlot = inventory.GetFirstEmptySlot(inventory.currentShop.buyBackInventorySlots);

                        for (int j = 0; j < inventory.currentShop.buyBackInventory.Count; j++)
                        {
                            if (inventory.currentShop.buyBackInventory[j] != null && inventory.currentShop.buyBackInventory[j].item == item && inventory.currentShop.buyBackInventory[j].numCarried < item.maxNumCarried)
                            {
                                sellSlot = j;
                                break;
                            }
                        }

                        ProduceInInventory produceInInventory = null;

                        if (GetComponent<ProduceInInventory>()) produceInInventory = GetComponent<ProduceInInventory>();

                        Button sellOneButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);

                        sellOneButton.onClick.AddListener(() => inventory.Sell(sellSlot, slot, inventory.currentShop, inventory.currentShop.buyBackInventorySlots, inventory.currentShop.buyBackInventory, 1, produceInInventory));
                        sellOneButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                        sellOneButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Sell 1 " + item.itemName + " [$" + itemValue + "]";

                        if (numCarried > 1)
                        {
                            if (numCarried / 2 > 1)
                            {
                                Button sellHalfButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);

                                sellHalfButton.onClick.AddListener(() => inventory.Sell(sellSlot, slot, inventory.currentShop, inventory.currentShop.buyBackInventorySlots, inventory.currentShop.buyBackInventory, numCarried / 2, produceInInventory));
                                sellHalfButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                                sellHalfButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Sell " + (numCarried / 2) + " [$" + itemValue * (numCarried / 2) + "]";
                            }

                            if (numCarried > 1)
                            {
                                Button sellAllButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);

                                sellAllButton.onClick.AddListener(() => inventory.Sell(sellSlot, slot, inventory.currentShop, inventory.currentShop.buyBackInventorySlots, inventory.currentShop.buyBackInventory, numCarried, produceInInventory));
                                sellAllButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                                sellAllButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Sell " + numCarried + " [$" + itemValue * numCarried + "]";
                            }
                        }

                        break;
                    }
                }

                
            }
        }

        if (shop == null)
        {
            List<InventoryUIItem> inv = chest == null ? inventory.inventory : chest.inventory;
            ItemSlot[] slot = chest == null ? inventory.inventorySlots : chest.inventorySlots;

            ProduceInInventory produceInInventory = null;

            if (GetComponent<ProduceInInventory>()) produceInInventory = GetComponent<ProduceInInventory>();

            if (numCarried > 1)
            {
                //Button takeAllButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);
                //takeAllButton.onClick.AddListener(() => inventory.SplitStack(this, numCarried));
                //takeAllButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                //takeAllButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Take All " + item.itemName;
                

                Button splitOneButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);
                splitOneButton.onClick.AddListener(() => inventory.SplitStack(this, 1, inv, slot, produceInInventory));
                splitOneButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                splitOneButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Split 1 " + item.itemName;

                if (numCarried / 2 > 1)
                {
                    Button splitHalfButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);
                    splitHalfButton.onClick.AddListener(() => inventory.SplitStack(this, numCarried / 2, inv, slot, produceInInventory));
                    splitHalfButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                    splitHalfButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Split " + numCarried / 2;
                }
                //ADD DROP & THROW ALL BUTTONS
            }

                Button dropButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);
                dropButton.onClick.AddListener(() => inventory.DropItem(this, true));
                dropButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                dropButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Drop 1 " + item.itemName;

                Button throwButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);
                throwButton.onClick.AddListener(() => inventory.ThrowItem(this, true));
                throwButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                throwButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Throw 1 " + item.itemName;


            
                //if (inventory.CountStacksOfType(this, inv) > 1)
                //{
                //    Button combineButton = Instantiate(rightClickMenuButton, rightClickMenuPanel.transform);
                //    combineButton.onClick.AddListener(() => inventory.CombineAllStacksOfType(this, inv, slot));
                //    combineButton.onClick.AddListener(() => rightClickMenu.DestroyMenu());
                //    combineButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Combine Stacks of " + item.itemName;
                //}
            
            
        }

    }

   
}
