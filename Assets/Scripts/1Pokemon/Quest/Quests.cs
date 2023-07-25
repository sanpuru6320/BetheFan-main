using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quests
{
    public QuestBase Base { get; private set; }

    public QusetStatus Status { get; private set; }

    public Quests(QuestBase _base)
    {
        Base = _base;
    }

    public Quests(QuestSaveData saveData)
    {
        Base = QuestDB.GetObjectByName(saveData.name);
        Status = saveData.status;
    }

    public QuestSaveData GetSaveData()
    {
        var saveData = new QuestSaveData()
        {
            name = Base.Name,
            status = Status
        };

        return saveData;
    }

    public IEnumerator StartQuest()
    {
        Status = QusetStatus.Started;

        yield return DialogManager.Instance.ShowDialog(Base.StartDialogue);

        var questList = QuestLists.GetQuestList();
        questList.AddQuest(this);
    }

    public IEnumerator CompletedQuest(Transform player)
    {
        Status = QusetStatus.Completed;

        yield return DialogManager.Instance.ShowDialog(Base.CompletedDialogue);

        var inventory = Inventory.GetInventory();
        if (Base.RequiredItem != null)
        {
            inventory.RemoveItem(Base.RequiredItem);
        }

        if (Base.RequiredItem != null)
        {
            inventory.AddItem(Base.RewardItem);

            string playerName = player.GetComponent<PlayerController>().Name;
            yield return DialogManager.Instance.ShowDialogText($"{playerName} recived {Base.RewardItem.Name}");
        }

        var questList = QuestLists.GetQuestList();
        questList.AddQuest(this);
    }

    public bool CanBeCompleted()
    {
        var inventory = Inventory.GetInventory();
        if (Base.RequiredItem != null)
        {
            if (!inventory.HasItem(Base.RequiredItem))
                return false;
        }

        return true;
    }



}

[System.Serializable]
public class QuestSaveData
{
    public string name;
    public QusetStatus status;
}

public enum QusetStatus { None, Started, Completed }