using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAction : CutsceneAction
{
    [SerializeField] Dialog dialog;

    public override IEnumerator Play()
    {
        yield return DialogManager.Instance.ShowDialog(dialog);
    }
}
