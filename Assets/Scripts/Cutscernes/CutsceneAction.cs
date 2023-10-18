using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CutsceneAction
{
    [SerializeField] string name;
    [SerializeField] bool waitForCompletion = true;//次のアクションを待たせるか

    public virtual IEnumerator Play()//起動する処理をここに書く
    {
        yield break;
    }

    public string Name {
        get => name;
        set => name = value;
    }

    public bool WaitForCompletion => waitForCompletion;
}
