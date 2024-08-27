using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class ItemUI : MonoBehaviour, IPointerDownHandler, IDragHandler, /*IDropHandler,*/ IBeginDragHandler, IEndDragHandler
{
    public InventoryItem item;

    public Inventory inventory;

    [SerializeField] private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startAnchoredPos;

    public Image image;

    public int slot;

    void Awake()
    {
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

    void Start()
    {
        startAnchoredPos = rectTransform.anchoredPosition;

    }

    public void InitialiseItem(InventoryItem newItem, int newSlot)
    {
        item = newItem;
        image.sprite = newItem.itemIcon;
        slot = newSlot;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        inventory.SelectItemUIAsButton(slot);
        Debug.Log(item.itemName + " selected");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
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

    public int GetIndex()
    {
        return slot;
    }

}
