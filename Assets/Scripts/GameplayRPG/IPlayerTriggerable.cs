using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerTriggerable //範囲内での判定
{
    void OnPlayerTriggered(PlayerController player);

    bool TriggerRepeatedly { get;}//true:繰り返し判定　false:初回のみ判定
}
