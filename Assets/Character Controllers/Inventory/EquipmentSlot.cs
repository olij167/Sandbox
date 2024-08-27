using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public event EventHandler<OnItemDroppedEventArgs> OnItemDropped;

    public class OnItemDroppedEventArgs : EventArgs
    {
        public InventoryUIItem newItem;
        //public InventoryItem oldItem;
    }

    //public EquipmentSlotType slotType;

    private PlayerInventory inventory;
    public int slot;
    public InventoryUIItem inventoryItem;
    public ItemType[] validTypes;

    void Awake()
    {
        inventory = FindObjectOfType<PlayerInventory>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("On Drop (Slot)");

        if (eventData.pointerDrag != null)
        {
            InventoryUIItem invItem = eventData.pointerDrag.GetComponent<InventoryUIItem>();

            if (invItem != null) // check whether it's a valid item for the slot
            {
                if (CheckItemType(invItem.item))
                {
                    OnItemDropped?.Invoke(this, new OnItemDroppedEventArgs { newItem = invItem });

                    invItem.isEquipped = true;

                    inventory.SwapItemSlot(invItem.GetItemIndex(), slot, invItem.isEquipped);

                    //Spawn in-game item on player (destroy existing equipment)

                }
                else Debug.Log("invalid item (Equipment Slot)");

            }
            else Debug.Log("no item found (Equipment Slot)");

            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            Debug.Log("New Anchor Set");
        }
    }

    public bool CheckItemType(InventoryItem item)
    {

        for (int i = 0; i < validTypes.Length; i++)
        {
            if (item.itemType == validTypes[i])
            {
                return true;
            }
        }

        return false;
    }
}

//[System.Serializable]
//public enum EquipmentSlotType
//{
//    Head, Chest, Legs, Feet, OffHand
//}
