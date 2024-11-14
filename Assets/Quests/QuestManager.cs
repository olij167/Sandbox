using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
   // public ItemQuestPreset testQuest;

    // manage all current quests
    public GameObject questUIPanel;
    public ItemQuestTracker itemQuestUI;

    public Sprite activeSprite;
    public Sprite completedSprite;
    public Sprite failedSprite;


    public List<QuestTracker> currentQuestUI;
    public List<QuestTracker> completeQuests;
    public List<QuestTracker> failedQuests;

    public ItemQuestPreset questToAdd;

    private void Awake()
    {
       // AddItemQuest(testQuest);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.J))
        //    ToggleQuestUI();

        for (int i = 0; i < currentQuestUI.Count; i++)
        {
            if (!currentQuestUI[i].isComplete && !currentQuestUI[i].isFailed)
            {
                if (!currentQuestUI[i].isActive)
                    currentQuestUI[i].StartQuest();
                else currentQuestUI[i].TrackQuest();
            }
            else FinishQuest(currentQuestUI[i].isComplete, currentQuestUI[i]);
        }
    }

    public void RefreshQuestUI()
    {
        foreach(Transform c in questUIPanel.transform)
        { Destroy(c.gameObject); }

        for (int i = 0; i < currentQuestUI.Count; i++)
        {

        }
    }

    public void AddItemQuest(ItemQuestPreset newQuest)
    {

        ItemQuestTracker newQuestUI = Instantiate(itemQuestUI, questUIPanel.transform);

        newQuestUI.quest = newQuest;
        newQuestUI.questItems = newQuest.questItems;

        if (!currentQuestUI.Contains(newQuestUI))
        currentQuestUI.Add(newQuestUI);

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

        currentQuestUI.Remove(quest);

    }

    public void ToggleQuestUI()
    {
        questUIPanel.SetActive(!questUIPanel.activeSelf);
    }

}
