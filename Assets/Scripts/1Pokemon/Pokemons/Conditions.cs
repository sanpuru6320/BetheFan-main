using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conditions//状態異常
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }//状態異常でのメッセージ

    public Action<Pokemon> OnStart { get; set; }//行動開始時
    public Func<Pokemon, bool> OnBeforeMove { get; set; }//技発動前
    public Action<Pokemon> OnAfterTurn { get; set; }//行動後
}
