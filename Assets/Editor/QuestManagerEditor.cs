using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuestManager))]
public class QuestManagerEditor : Editor
{
   
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        QuestManager questManager = (QuestManager)target;
        if (questManager == null) return;

        if (questManager.questToAdd != null && GUILayout.Button("Add Quest"))
        {
            questManager.AddItemQuest(questManager.questToAdd);
        }
    }
}
