using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemQuestTracker : QuestTracker
{
    private PlayerInventory inventory;

    public bool allItemsCollected;
    public List<QuestItem> questItems;

    [System.Serializable]
    public class QuestItem
    {
        public InventoryItem item;
        public float requiredAmount;
        public float currentAmount;
        public bool isCollected;
    }

    private void Awake()
    {
        inventory = FindObjectOfType<PlayerInventory>();
    }

    public override void TrackQuest()
    {
        foreach(QuestItem qItem in questItems)
        {
            if (inventory.CheckInventoryForItem(qItem.item))
            {
                Debug.Log("Item in inventory");
                qItem.currentAmount = inventory.GetInventoryItem(qItem.item).numCarried;

                progressText.text = qItem.currentAmount + " / " + qItem.requiredAmount;

                if (qItem.currentAmount >= qItem.requiredAmount)
                {
                    qItem.isCollected = true;
                }
                else qItem.isCollected = false;
            }
            else qItem.isCollected = false;
        }

        for (int i = 0; i < questItems.Count; i++)
        {
            if (!questItems[i].isCollected)
            {
                allItemsCollected = false;
            }
            else if (i + 1 >= questItems.Count)
                allItemsCollected = true;
        }


        if (allItemsCollected)
            CompleteQuest();
    }
}
