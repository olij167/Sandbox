using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryUIItem : MonoBehaviour, /*IPointerDownHandler,*/ IDragHandler, /*IDropHandler,*/ IBeginDragHandler, IEndDragHandler
{
    private PlayerInventory inventory;

    public InventoryItem item;
    public Image image;
    //public SkinnedMeshRenderer mesh;

    public TextMeshProUGUI stackCountText;
    public int numCarried;

    public bool isInUse;

    public float batteryCharge;
    public float ammo;

    public GameObject physicalItem;

    public bool isEquipped;
    public int slot;

    //public bool fromChest;
    public ChestInventory chest;

    [SerializeField] private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startAnchoredPos;


    void Awake()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        Transform testCanvasTransform = transform;
        do
        {
            testCanvasTransform = testCanvasTransform.parent;
            canvas = testCanvasTransform.GetComponent<Canvas>();
        }
        while (canvas == null);
    }

    private void Start()
    {
        startAnchoredPos = rectTransform.anchoredPosition;
    }

    public void InitialiseItem(InventoryItem newItem, int newSlot, /*bool isFromChest = false,*/ ChestInventory chestInventory = null)
    {
        item = newItem;
        image.sprite = newItem.itemIcon;
        numCarried = 1;
        if (item.isStackable)
        {
            stackCountText.text = "[" + numCarried.ToString() + "]";
        }
        else stackCountText.gameObject.SetActive(false);

        if (item.usesBatteries)
        {
            batteryCharge = item.maxBatteryCharge;
        }
        //else ba
        if (item.isProjectile)
        {
            ammo = item.maxAmmo;
        }

        slot = newSlot;

        //fromChest = isFromChest;
        chest = chestInventory;
    }
    
    //public void InitialiseEquipment(Item newItem)
    //{
    //    item = newItem;
    //    image.sprite = newItem.itemIcon;
    //    numCarried = 1;
    //    if (item.isStackable)
    //    {
    //        stackCountText.text = "[" + numCarried.ToString() + "]";
    //    }
    //    else stackCountText.gameObject.SetActive(false);

    //    if (item.usesBatteries)
    //    {
    //        batteryCharge = item.maxBatteryCharge;
    //    }
    //}

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    inventory.SelectInventoryItemAsButton(slot);
    //    Debug.Log(item.itemName + " selected");
    //}

    public void OnBeginDrag(PointerEventData eventData)
    {
        //canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

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

        rectTransform.anchoredPosition = startAnchoredPos;
    }

    public int GetItemIndex()
    {
        return slot;
    }

    //public void InitialiseUsedItem(InventoryItem usedItem)
    //{
    //    item = usedItem.item;
    //    image.sprite = usedItem.item.itemIcon;
    //    physicalItem = usedItem.physicalItem;

    //    numCarried = 1;
    //    if (item.isStackable)
    //    {
    //        stackCountText.text = "[" + numCarried.ToString() + "]";
    //    }
    //    else stackCountText.gameObject.SetActive(false);

    //    if (item.usesBatteries)
    //    {
    //        batteryCharge = usedItem.batteryCharge;
    //    }

    //    isInUse = true;
    //}
}
