using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//トレーナーのポケモン入れ替え時にプレイヤーもポケモン入れ替えを行うか選択
public class AboutToUseState : State<BattleSystem>
{
    public Pokemon NewPokemon { get; set; }

    bool aboutToUseChoice;


    public static AboutToUseState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    BattleSystem bs;

    public override void Enter(BattleSystem owner)
    {
        bs = owner;
        StartCoroutine(StartState());
    }

    IEnumerator StartState()
    {
        yield return bs.DialogBox.TypeDialog($"{bs.trainer.Name} is about to use {NewPokemon.Base.Name}. Do you want to change pokemon?");
        bs.DialogBox.EnableChoiceBox(true);
    }

    public override void Excute()
    {
        if (!bs.DialogBox.IsChoiceBoxEnabled)
            return;
        
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            aboutToUseChoice = !aboutToUseChoice;

        bs.DialogBox.UpdateChoiceBox(aboutToUseChoice);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            bs.DialogBox.EnableChoiceBox(false);
            if (aboutToUseChoice == true)
            {
                // Yes Option
                StartCoroutine(SwitchAndContinueBattle());
            }
            else
            {
                //No Option
                StartCoroutine(ContinueBattle());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            bs.DialogBox.EnableChoiceBox(false);
            StartCoroutine(ContinueBattle());
        }
    }

    IEnumerator SwitchAndContinueBattle()//プレイヤーポケモン入れ替え
    {
        yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
        var selectedPokemon = PartyState.i.SelectedPokemon;
        if(selectedPokemon != null)
        {
            yield return bs.SwitchPokemon(selectedPokemon);
        }

        yield return ContinueBattle();
    }

    IEnumerator ContinueBattle()//トレーナーポケモン入れ替え
    {
        yield return bs.SendNextTrainerPokemon();
        bs.StateMachine.Pop();
    }
}
