using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsecneState : State<GameController>
{
    public static CutsecneState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public override void Excute()
    {
        PlayerController.i.Character.HandleUpdate();
    }
}
