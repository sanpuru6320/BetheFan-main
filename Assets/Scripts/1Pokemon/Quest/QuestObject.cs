using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    [SerializeField] QuestBase questToCheck;
    [SerializeField] ObjectActions onStart;
    [SerializeField] ObjectActions onComplete;

    QuestLists questList;
    public void Start()
    {
        questList = QuestLists.GetQuestList();
        questList.OnUpdated += UpdateObjectStatus;

        UpdateObjectStatus();
    }

    private void OnDestroy()
    {
        questList.OnUpdated -= UpdateObjectStatus;
    }

    public void UpdateObjectStatus()//クエストに関連するオブジェクトのアクティブ化/非アクティブ化
    {
        if (onStart != ObjectActions.DoNothing && questList.IsStarted(questToCheck.Name))
        {
            foreach(Transform child in transform)
            {
                if (onStart == ObjectActions.Enable)
                {
                    child.gameObject.SetActive(true);

                    //var saveable = child.GetComponent<SavableEntity>();
                    //if(saveable != null)
                    //    SavingSystem.i.RestoreEntity(saveable);
                    
                }
                else if (onStart == ObjectActions.Disable)
                    child.gameObject.SetActive(false);
            }
        }

        if (onComplete != ObjectActions.DoNothing && questList.IsCompleted(questToCheck.Name))
        {
            foreach (Transform child in transform)
            {
                if (onComplete == ObjectActions.Enable)
                {
                    child.gameObject.SetActive(true);

                    //var saveable = child.GetComponent<SavableEntity>();
                    //if (saveable != null)
                    //    SavingSystem.i.RestoreEntity(saveable);

                }
                else if (onComplete == ObjectActions.Disable)
                    child.gameObject.SetActive(false);
            }
        }
    }
}

public enum ObjectActions { DoNothing, Enable, Disable}
