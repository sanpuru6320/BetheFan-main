using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuState : State<GameController>
{
    [SerializeField] MenuController menuController;
    
    public static GameMenuState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;
        menuController.gameObject.SetActive(true);
        menuController.OnSelected += OnMenuItemSelected;
        menuController.OnBack += OnBack;
    }

    public override void Excute()
    {
        menuController.HandleUpdete();


    }

    public override void Exit()
    {
        menuController.gameObject.SetActive(false);
        menuController.OnSelected -= OnMenuItemSelected;
        menuController.OnBack -= OnBack;
    }

    void OnMenuItemSelected(int selection)
    {
        if (selection == 0)//pokemon
            gc.StateMachine.Push(PartyState.i);
        else if (selection == 1)//Bag
            gc.StateMachine.Push(InventoryState.i);
        else if(selection == 2)//Boxes
            gc.StateMachine.Push(StorageState.  i);
    }

    void OnBack()
    {
        gc.StateMachine.Pop();
    }
}
