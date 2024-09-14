using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest Presets/Item")]
public class ItemQuestPreset : QuestPreset
{
    public List<ItemQuestTracker.QuestItem> questItems;
}
