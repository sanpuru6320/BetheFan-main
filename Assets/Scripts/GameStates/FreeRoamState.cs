using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamState : State<GameController>//マップ初期ステート
{
    public static FreeRoamState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;
    }
    public override void Excute()
    {
        PlayerController.i.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Return))//メニュー表示
            gc.StateMachine.Push(GameMenuState.i);
    }
}
