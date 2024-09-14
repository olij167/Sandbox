using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class QuestTracker : MonoBehaviour
{
    //tracks each individual quest - create derivative scripts for specific quest types

    public QuestPreset quest;

    public bool isActive;
    public bool isComplete;
    public bool isFailed;

    public Transform questLocation;

    public TextMeshProUGUI questName;
    public TextMeshProUGUI questDetails;
    public TextMeshProUGUI progressText;
    public Image progressImage;

    public virtual void TrackQuest()
    {
        //yield return null;
    }

    public virtual void StartQuest()
    {
        isActive = true;
        isComplete = false;
        isFailed = false;

        //StartCoroutine(TrackQuest());
    }

    public virtual void CompleteQuest()
    {
        isActive = false;
        isComplete = true;
        isFailed = false;
        //Reward player
    }

    public virtual void FailQuest()
    {
        isActive = false;
        isComplete = false;
        isFailed = true;
        //punish player
    }
    
}
