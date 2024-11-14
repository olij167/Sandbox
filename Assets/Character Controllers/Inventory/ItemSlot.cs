using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{

    private PlayerInventory inventory;
    public int slot;
    public InventoryUIItem inventoryItem;
    public ChestInventory chest;
    public ShopInventory shop;

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

                if (chest != null && invItem.chest == null)
                {
                    inventory.SwapToChest(slot, invItem.GetItemIndex(), chest, invItem.numCarried);
                    Debug.Log("Swapping to chest");
                }
                else if (chest == null && invItem.chest != null)
                {
                    inventory.SwapFromChest(slot, invItem.GetItemIndex(), invItem.chest, invItem.numCarried);
                    Debug.Log("Swapping from chest");
                }
                else if (chest != null && invItem.chest != null)
                {
                    if (inventoryItem != null && inventoryItem.item == invItem.item)
                    {
                        inventory.CombineStack(inventoryItem, invItem, chest.inventory, chest.inventorySlots);
                    }
                    else
                    {
                        chest.SwapItemSlot(slot, invItem.GetItemIndex());
                    }
                    Debug.Log("Swapping within chest");
                }
                else if (shop != null && invItem.shop == null)
                {
                    if (inventory.currentShop.typesWillBuy != null && inventory.currentShop.typesWillBuy.Length > 0) // check whether the shop will buy the item before allowing a sale
                    {
                        for (int i = 0; i < inventory.currentShop.typesWillBuy.Length; i++)
                        {
                            if (inventory.currentShop.typesWillBuy[i] == invItem.item.itemType)
                            {
                                inventory.Sell(slot, invItem.GetItemIndex(), shop, shop.buyBackInventorySlots, shop.buyBackInventory);
                                Debug.Log("Selling to shop");
                            }
                        }
                    }
                }
                else if (shop == null && invItem.shop != null)
                {
                    if (inventory.money >= invItem.itemValue)
                    {
                        //inventory.Buy(slot, invItem.GetItemIndex(), invItem.shop, invItem.shop.i);
                        if (shop.buyBackPanelOpen)
                            inventory.Buy(invItem.GetItemIndex(), slot, shop, shop.buyBackInventorySlots, shop.buyBackInventory, 1, true);
                        else
                            inventory.Buy(invItem.GetItemIndex(), slot, shop, shop.inventorySlots, shop.inventory);
                        Debug.Log("Buying from shop");
                    }
                    else Debug.Log("Not enough money");
                }
                else
                {
                    if (inventoryItem != null && inventoryItem.item == invItem.item)
                    {
                        inventory.CombineStack(inventoryItem, invItem, inventory.inventory, inventory.inventorySlots);
                        Debug.Log("Combining Slots");
                    }
                    else
                    {
                        inventory.SwapItemSlot(slot, invItem.GetItemIndex(), invItem.isEquipped);
                    Debug.Log("Swapping within inventory");
                    }

                }
            }
            else Debug.Log("No item found (Slot)");

            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            //Debug.Log("New Anchor Set");
        }
    }
}
