using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    private PlayerInventory inventory;
    private PlayerUI playerUI;
    private GameObject selectedItem;
    private ItemAction itemAction;

    private void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        playerUI = FindObjectOfType<PlayerUI>();
    }

    private void Update()
    {
        if (inventory.selectedInventoryItem != null && inventory.selectedInventoryItem.item.canActivate)
        {
            if (selectedItem != inventory.selectedPhysicalItem)
            {
                if (inventory.selectedInventoryItem.item.usesBatteries)
                {
                    playerUI.batteryBar.gameObject.SetActive(true);
                    playerUI.InitialiseBatteryBar(inventory.selectedInventoryItem);
                }
                else playerUI.batteryBar.gameObject.SetActive(false);

                selectedItem = inventory.selectedPhysicalItem;
                itemAction = selectedItem.GetComponent<ItemAction>();
            }

            if (itemAction != null && Input.GetMouseButtonDown(0))
            {
                itemAction.ItemFunction();
            }
        }
        else playerUI.batteryBar.gameObject.SetActive(false);
    }
}
