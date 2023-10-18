using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestLists : MonoBehaviour, ISaveable
{
    List<Quests> quests = new List<Quests>();//進行中、完了後のクエストリスト

    public event Action OnUpdated;

    public void AddQuest(Quests quest)//進行中、完了後のクエストを登録
    {
        if (!quests.Contains(quest))
            quests.Add(quest);

        OnUpdated?.Invoke();
    }

    public bool IsStarted(string questName)//進行中か判定
    {
        var questStatus = quests.FirstOrDefault(q => q.Base.Name == questName)?.Status;
        return questStatus == QusetStatus.Started || questStatus == QusetStatus.Completed;
    }

    public bool IsCompleted(string questName)//完了か判定
    {
        var questStatus = quests.FirstOrDefault(q => q.Base.Name == questName)?.Status;
        return questStatus == QusetStatus.Completed;
    }

    public static QuestLists GetQuestList()
    {
        return FindObjectOfType<PlayerController>().GetComponent<QuestLists>();
    }

    public object CaptureState()
    {
        return quests.Select(q => q.GetSaveData()).ToList();
    }

    public void RestoreState(object state)
    {
        var saveData = state as List<QuestSaveData>;
        if (saveData != null)
        {
            quests = saveData.Select(q => new Quests(q)).ToList();
            OnUpdated?.Invoke();
        }
    }
}