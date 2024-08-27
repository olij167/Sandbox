using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class Inventory : MonoBehaviour
{
    public int selectedSlot = 0;
    public InventoryItem selectedItem;

    public List<InventoryItem> inventoryContents;
    private InventorySlot[] slots;

    [SerializeField] private GameObject uiPrefab;

    [SerializeField] private GameObject barPanel;
    [SerializeField] private GameObject windowPanel;
    public bool inventoryWindowOpen;

    [SerializeField] private Color selectedColour;
    private Color originalColour;

    private void Awake()
    {
        slots = new InventorySlot[barPanel.transform.childCount + windowPanel.transform.childCount];

        originalColour = slots[0].GetComponent<Image>().color;

    }

    public void AddItemToInventory(InventoryItem item, GameObject itemInWorld)
    {
        if (CheckEmptySlots(slots) > 0)
        {
            for (int s = 0; s < slots.Length; s++)
            {
                if (slots[s].transform.childCount == 0) // for the first empty slot
                {
                    SpawnNewUI(item, s);


                    Destroy(itemInWorld);

                    inventoryContents.Add(item);
                    //inventoryContents[s] = item;

                    Debug.Log("item added");
                    return;
                }
            }
        }
    }

    void SpawnNewUI(InventoryItem item, int uiSlot)
    {
        GameObject newUI = Instantiate(uiPrefab, slots[uiSlot].transform);
        ItemUI newItem = newUI.GetComponent<ItemUI>();

        slots[uiSlot].itemUI = newItem;

        Button button = newUI.AddComponent<Button>();
        button.onClick.AddListener(() => SelectItemUIAsButton(uiSlot));

        newItem.InitialiseItem(item, uiSlot);

        //inventoryContents[uiSlot] = itemItem;

    }

    int CheckEmptySlots(InventorySlot[] slots)
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

    public void SelectItemUIAsButton(int index)
    {
        if (index <= slots.Length && slots[index] != null)
        {
            selectedSlot = index;

            selectedItem = slots[index].itemUI.item;
            //Debug.Log("Item selected with UI Button");
        }
        //SetSelecteditemColour();

    }

    private void SelectItemWithNumbers()
    {
        if (slots.Length >= 1)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (slots.Length >= 1)
                {
                    selectedSlot = 0;
                    if (inventoryContents[selectedSlot] != null)
                    {
                        selectedItem = inventoryContents[0];
                    }
                }
            }
        }
        if (slots.Length >= 2)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (slots.Length >= 2)
                {
                    selectedSlot = 1;
                    if (inventoryContents[selectedSlot] != null)
                    {
                        selectedItem = inventoryContents[1];
                    }
                }
            }
        }
        if (slots.Length >= 3)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (slots.Length >= 3)
                {
                    selectedSlot = 2;
                    if (inventoryContents[selectedSlot] != null)
                    {
                        selectedItem = inventoryContents[2];
                    }
                }
            }
        }
        if (slots.Length >= 4)
        {
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (slots.Length >= 4)
                {
                    selectedSlot = 3;
                    if (inventoryContents[selectedSlot] != null)
                    {
                        selectedItem = inventoryContents[3];
                    }
                }
            }
        }
        if (slots.Length >= 5)
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (slots.Length >= 5)
                {
                    selectedSlot = 4;
                    if (inventoryContents[selectedSlot] != null)
                    {
                        selectedItem = inventoryContents[4];
                    }
                }
            }
        }
        if (slots.Length >= 6)
        {
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (slots.Length >= 6)
                {
                    selectedSlot = 5;
                    if (inventoryContents[selectedSlot] != null)
                    {
                        selectedItem = inventoryContents[5];
                    }
                }
            }
        }
        if (slots.Length >= 7)
        {
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (slots.Length >= 7)
                {
                    selectedSlot = 6;
                    if (inventoryContents[selectedSlot] != null)
                    {
                        selectedItem = inventoryContents[6];
                    }
                }
            }
        }
        if (slots.Length >= 8)
        {
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (slots.Length >= 8)
                {
                    selectedSlot = 7;
                    if (inventoryContents[selectedSlot] != null)
                    {
                        selectedItem = inventoryContents[7];
                    }
                }
            }
        }
        if (slots.Length >= 9)
        {
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                if (slots.Length >= 9)
                {
                    selectedSlot = 8;

                    if (inventoryContents[selectedSlot] != null)
                    {
                        selectedItem = inventoryContents[8];
                    }
                }
            }
        }
        if (slots.Length == 10)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (slots.Length >= 10)
                {
                    selectedSlot = 9;

                    if (inventoryContents[selectedSlot] != null)
                    {
                        selectedItem = inventoryContents[9];
                    }
                }
            }
        }
    }

    private void SelectItemWithScroll()
    {
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            selectedSlot += Mathf.RoundToInt(Input.mouseScrollDelta.y);
            //selectedItemSlot += Mathf.Clamp(Mathf.RoundToInt(Input.mouseScrollDelta.y), 1, inventorySlots.Length);

            if (inventoryWindowOpen)
            {
                int lastFilledSlot = 0;

                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].transform.childCount >= 0)
                        lastFilledSlot = i;
                }

                //Debug.Log("Last filled slot = " + lastFilledSlot);

                if (selectedSlot > lastFilledSlot)
                {
                    selectedSlot = 0;
                }
                else if (selectedSlot < 0)
                {
                    selectedSlot = lastFilledSlot;
                }
            }
            else
            {
                //only scroll through filled item slots


                if (selectedSlot > slots.Length - windowPanel.transform.childCount - 1)
                {
                    selectedSlot = 0;
                }
                else if (selectedSlot < 0)
                {
                    selectedSlot = slots.Length - windowPanel.transform.childCount - 1;
                }
            }


            if (slots[selectedSlot].itemUI != null)
            {
                selectedItem = slots[selectedSlot].itemUI.item;
                //SetSelecteditemColour();
            }
        }
    }

    private void SetSelectedColour(Transform[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i == selectedSlot)
            {
                slots[i].GetComponent<Image>().color = selectedColour;
            }
            else slots[i].GetComponent<Image>().color = originalColour;
        }
    }

    public void SwapSlot(int indexA, int indexB)
    {
        // Store item info
        ItemUI itemItem = slots[indexA].itemUI;

        //Swap Slot Info
        slots[indexA].itemUI = slots[indexB].itemUI;
        //slots[indexA].slot = indexB;

        slots[indexB].itemUI = itemItem;
        //slots[indexB].slot = indexA;

        //Delete & Spawn UI
        if (slots[indexA].transform.childCount > 0)
        {
            foreach (Transform child in slots[indexA].transform)
            {
                Destroy(child.gameObject);
            }
            SpawnNewUI(slots[indexB].itemUI.item, indexB);

        }
        if (slots[indexB].transform.childCount > 0)
        {
            foreach (Transform child in slots[indexB].transform)
            {
                Destroy(child.gameObject);
            }
            SpawnNewUI(slots[indexA].itemUI.item, indexA);

        }
        Debug.Log("Swapped " + indexA + " UI Slot with " + indexB + " UI Slot");

    }
}
