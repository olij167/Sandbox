using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public ItemQuestPreset testQuest;

    // manage all current quests
    public GameObject questUIPanel;
    public ItemQuestTracker itemQuestUI;

    public Sprite activeSprite;
    public Sprite completedSprite;
    public Sprite failedSprite;


    public List<QuestTracker> currentQuests;
    public List<QuestTracker> completeQuests;
    public List<QuestTracker> failedQuests;

    private void Awake()
    {
       // AddItemQuest(testQuest);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleQuestUI();

        for (int i = 0; i < currentQuests.Count; i++)
        {
            if (!currentQuests[i].isComplete && !currentQuests[i].isFailed)
            {
                if (!currentQuests[i].isActive)
                    currentQuests[i].StartQuest();
                else currentQuests[i].TrackQuest();
            }
            else FinishQuest(currentQuests[i].isComplete, currentQuests[i]);
        }
    }

    public void AddItemQuest(ItemQuestPreset newQuest)
    {

        ItemQuestTracker newQuestUI = Instantiate(itemQuestUI, questUIPanel.transform);

        newQuestUI.quest = newQuest;
        newQuestUI.questItems = newQuest.questItems;

        currentQuests.Add(newQuestUI);

        newQuestUI.questName.text = newQuest.questName;
        newQuestUI.questDetails.text = newQuest.questDetails;

        newQuestUI.progressImage.sprite = activeSprite;
    }

    public void FinishQuest(bool questCompleted, QuestTracker quest)
    {
        if (questCompleted)
        {
            completeQuests.Add(quest);
            quest.progressImage.sprite = completedSprite;
        }
        else
        {
            failedQuests.Add(quest);
            quest.progressImage.sprite = failedSprite;
        }

        currentQuests.Remove(quest);

    }

    public void ToggleQuestUI()
    {
        questUIPanel.SetActive(!questUIPanel.activeSelf);
    }

}
