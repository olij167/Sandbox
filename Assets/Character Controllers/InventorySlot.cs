using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    private Inventory inventory;
    public int slot;
    public ItemUI itemUI;

    void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    public int GetEmoteIndex()
    {
        return slot;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("On Drop (Slot)");

        if (eventData.pointerDrag != null)
        {
            ItemUI item = eventData.pointerDrag.GetComponent<ItemUI>();

            if (item != null)
            {
                inventory.SwapSlot(slot, item.GetIndex());
            }
            else Debug.Log("No item found (Slot)");

            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            Debug.Log("New Anchor Set");
        }
    }
}
