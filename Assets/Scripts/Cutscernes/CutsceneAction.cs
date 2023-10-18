using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CutsceneAction
{
    [SerializeField] string name;
    [SerializeField] bool waitForCompletion = true;//���̃A�N�V������҂����邩

    public virtual IEnumerator Play()//�N�����鏈���������ɏ���
    {
        yield break;
    }

    public string Name {
        get => name;
        set => name = value;
    }

    public bool WaitForCompletion => waitForCompletion;
}
