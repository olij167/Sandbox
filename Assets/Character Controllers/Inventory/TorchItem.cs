using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TorchItem : ItemAction
{
    public Light torchLight;
    public float battery;
    private float maxBattery = 100;
    public float batteryDecreaseRate = 1f;

    public InventoryItem batteryItem;

    //public Slider batterySlider;

    private PlayerInventory inventory;
    private InventoryUIItem inventoryItem;

    private PlayerUI playerUI;

    private void Start()
    {
        maxBattery = GetComponent<ItemInWorld>().item.maxBatteryCharge;

        //battery = maxBattery;
        torchLight.gameObject.SetActive(false);
        inventory = FindObjectOfType<PlayerInventory>();
        playerUI = FindObjectOfType<PlayerUI>();
    }

    public override void ItemFunction()
    {
        if (inventoryItem == null) inventoryItem = inventory.selectedInventoryItem;

        if (battery <= 0f)
        {
            if (inventory.CheckInventoryForItem(batteryItem))
            {
                InventoryUIItem foundBattery = inventory.GetInventoryItem(batteryItem);

                if (foundBattery != null)
                {
                    IncreaseBattery(foundBattery.batteryCharge);

                    inventory.RemoveItemFromInventory(foundBattery, inventory.inventory);
                }
                else Debug.Log("Out of battery");
            }
            else Debug.Log("Out of battery");
        }

        if (battery > 0)
        {
            torchLight.gameObject.SetActive(!torchLight.gameObject.activeInHierarchy);
            inventoryItem.isInUse = torchLight.gameObject.activeInHierarchy;

            if (torchLight.gameObject.activeInHierarchy)
            {
                DecreaseBattery();
            }
        }
        //else torchLight.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (battery > 0)
        {
            if (torchLight.gameObject.activeInHierarchy || (inventoryItem != null && inventoryItem.isInUse))
            {
                inventoryItem.isInUse = true;
                torchLight.gameObject.SetActive(true);
                DecreaseBattery();
            } 
            else if (inventoryItem == null)
            {
                if (inventory.selectedInventoryItem != null)
                {
                    inventoryItem = inventory.selectedInventoryItem;
                }
                else if (GetComponent<InventoryUIItem>())
                {
                    inventoryItem = GetComponent<InventoryUIItem>();
                }
            }
        }
        else
        {
            inventoryItem.isInUse = false;
            torchLight.gameObject.SetActive(false);
        }
    }

    public void DecreaseBattery()
    {
        if (torchLight.gameObject.activeInHierarchy)
        {
            if (inventoryItem == null) inventoryItem = GetComponent<InventoryUIItem>();
            if (battery != inventoryItem.batteryCharge)
                battery = inventoryItem.batteryCharge;

            battery -= Time.deltaTime * batteryDecreaseRate;
            playerUI.batteryBar.value = battery;
            inventoryItem.batteryCharge = battery;
        }
    }

    public void IncreaseBattery(float increaseAmount)
    {
        if (battery + increaseAmount < maxBattery)
        {
            battery += increaseAmount;
        }
        else battery = maxBattery;

        inventoryItem.batteryCharge = battery;

    }
}
