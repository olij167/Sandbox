using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public event EventHandler<OnItemDroppedEventArgs> OnItemDropped;

    public class OnItemDroppedEventArgs : EventArgs
    {
        public InventoryUIItem newItem;
    }

    private PlayerInventory inventory;
    public int slot;
    public InventoryUIItem inventoryItem;
    public ChestInventory chest;

    void Awake()
    {
        inventory = FindObjectOfType<PlayerInventory>();
    }

    public int GetSlotIndex()
    {
        return slot;
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("On Drop (Slot)");

        if (eventData.pointerDrag != null)
        {
            InventoryUIItem invItem = eventData.pointerDrag.GetComponent<InventoryUIItem>();

            if (invItem != null)
            {
                OnItemDropped?.Invoke(this, new OnItemDroppedEventArgs { newItem = invItem });

                if (chest != null && invItem.chest == null)
                    inventory.SwapToChest(slot, invItem.GetItemIndex(), chest);
                else if (chest == null && invItem.chest != null)
                    inventory.SwapFromChest(slot, invItem.GetItemIndex(), invItem.chest);
                else if (chest != null && invItem.chest != null)
                    chest.SwapItemSlot(slot, invItem.GetItemIndex());
                else
                    inventory.SwapItemSlot(slot, invItem.GetItemIndex(), invItem.isEquipped);

            }
            else Debug.Log("No item found (Slot)");

            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            Debug.Log("New Anchor Set");
        }
    }
}
