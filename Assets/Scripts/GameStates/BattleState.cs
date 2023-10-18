using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState : State<GameController>
{
    [SerializeField] BattleSystem battleSystem;

    //input
    public BattleTrigger trigger { get; set; }
    public TrainerController trainer { get; set; }

    public static BattleState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        //バトル画面へ移行
        battleSystem.gameObject.SetActive(true);
        gc.WorldCamera.gameObject.SetActive(false);

        var playerParty = gc.PlayerController.GetComponent<PokemonParty>();

        if (trainer == null)//ランダムバトル
        {
            var wildPokemon = gc.CurrentScene.GetComponent<MapArea>().GetRandomwildPokemon(trigger);//エンカウントするポケモン取得
            var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);
            battleSystem.StartBattle(playerParty, wildPokemonCopy, trigger);
        }
        else//トレーナーバトル
        {
            var trainerParty = trainer.GetComponent<PokemonParty>();
            battleSystem.StartTrainerBattle(playerParty, trainerParty);
        }

        battleSystem.OnBattleOver += EndBattle;

    }

    public override void Exit()
    {
        //マップ画面へ移行
        battleSystem.gameObject.SetActive(false);
        gc.WorldCamera.gameObject.SetActive(true);

        battleSystem.OnBattleOver -= EndBattle;
    }

    public override void Excute()
    {
        battleSystem.HandleUpdate();
    }

    void EndBattle(bool won)//
    {
        if(trainer != null && won == true)
        {
            trainer.Battlelost();
            trainer = null;
        }

        gc.StateMachine.Pop();
    }

    public BattleSystem BattleSystem => battleSystem;
}
