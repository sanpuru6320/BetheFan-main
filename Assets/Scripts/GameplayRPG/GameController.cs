﻿using GDEUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;
    private BuddyController buddy;

    public StateMachine<GameController> StateMachine { get; private set; }

    public SceneDetail CurrentScene { get; private set; }
    public SceneDetail PrevScene { get; private set; }

    public static GameController Instance{get; private set; }

    public BuddyController Buddy { get => buddy; set => buddy = value; }
    //public PlayerController PlayerController { get => playerController; set => playerController = value; }

    private void Awake()
    {
        Instance = this;

        //マウス無効化
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        PokemonDB.Init();
        MoveDB.Init();
        ConditionsDB.Init();
        ItemDB.Init();
        QuestDB.Init();

        //buddy = FindObjectOfType<BuddyController>().GetComponent<BuddyController>();
    }
    private void Start()
    {
        //初期ステート移行
        StateMachine = new StateMachine<GameController>(this);
        StateMachine.ChangeState(FreeRoamState.i);
        
        battleSystem.OnBattleOver += EndBattle;

        partyScreen.Init();


        DialogManager.Instance.OnShowDialog += () => 
        {
            StateMachine.Push(DialogueState.i);
        }; 
        
        DialogManager.Instance.OnDialogFinished += () => 
        {
            StateMachine.Pop();
        };
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            StateMachine.Push(PauseState.i);
        }
        else
        {
            StateMachine.Pop();
        }
    }

    //戦闘ステートへ移行
    public void StartBattle(BattleTrigger trigger)
    {
        BattleState.i.trigger = trigger;
        StateMachine.Push(BattleState.i);

    }

    TrainerController trainer;

    public void StartTrainerBattle(TrainerController trainer)
    {
        BattleState.i.trainer = trainer;
        StateMachine.Push(BattleState.i);
    }

    public void OnEnterTrainersView(TrainerController trainer)//トレーナーの視界にプレイヤーが入る
    {
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    void EndBattle(bool won)
    {
        if (trainer != null && won == true)
        {
            trainer.Battlelost();
            trainer = null;
        }

        //マップ中のパーティ画面更新
        partyScreen.SetPartyData();

        //カメラ切り替え
        battleSystem.gameObject.SetActive(false); 
        worldCamera.gameObject.SetActive(true);

        //進化可能かチェック
        var playerParty = playerController.GetComponent<PokemonParty>();
        bool hasEvolution = playerParty.CheckForEvolutions();

        if (hasEvolution)
            StartCoroutine(playerParty.RunEvolutions());
        else
            AudioManager.i.PlayMusic(CurrentScene.SceneMusic, fade: true);

    }

    private void Update()
    {
        StateMachine.Excute();

    }

    public void SetCurrentScene(SceneDetail currScene)//現在のシーン取得
    {
        PrevScene = CurrentScene; 
        CurrentScene = currScene;
    }

    public IEnumerator MoveCamera(Vector2 moveOffset, bool waitForFadeOut=false)//カメラ移動
    {
        yield return Fader.i.FadeIn(0.5f);
        
        worldCamera.transform.position += new Vector3(moveOffset.x, moveOffset.y);

        if (waitForFadeOut)
            yield return Fader.i.FadeOut(0.5f);
        else
            StartCoroutine(Fader.i.FadeOut(0.5f));


    }

    private void OnGUI()//ステートの表示
    {
        var style = new GUIStyle();
        style.fontSize = 24;

        GUILayout.Label("STATE STACK", style);
        foreach(var state in StateMachine.StateStack)
        {
            GUILayout.Label(state.GetType().ToString(), style);
        }
    }

    public PlayerController PlayerController => playerController;

    public Camera WorldCamera => worldCamera;

    public PartyScreen PartyScreen => partyScreen;
}
