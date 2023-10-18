using DG.Tweening;
using GDEUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum BattleAction { Move, SwitchPokemon, UseItem, Run }//戦闘中のアクション

public enum BattleTrigger { LongGrass, Water}//エンカウント地形

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] MoveToForgetSelectionUI moveSelectionUI;
    [SerializeField] InventoryUI inventoryUI;

    [Header("Audio")]
    [SerializeField] AudioClip wildBattleMusic;
    [SerializeField] AudioClip trainerBattleMusic;
    [SerializeField] AudioClip battleVictoryMusic;

    [Header("Background Images")]
    [SerializeField] Image backgroundImage;
    [SerializeField] Sprite grassBackground;
    [SerializeField] Sprite waterBackground;

    public StateMachine<BattleSystem> StateMachine { get; private set; }
    
    public event Action<bool> OnBattleOver;

    public int SelectedMove { get; set; }
    public BattleAction SelectedAction { get; set; }
    public Pokemon SelectedPokemon { get; set; }
    public ItemBase SelectedItem { get; set; }

    public bool isBattleOver { get; private set; }

    public PokemonParty PlayerParty { get; private set; } 
    public PokemonParty TrainerParty { get; private set; }
    public Pokemon WildPokemon { get; private set; }

    public bool IsTrainerBattle { get; private set; } = false;
    PlayerController player; 
    public TrainerController trainer { get; private set; }

    public int EscapeAttempts { get; set; }

    BattleTrigger battleTrigger;

    //ランダムバトル開始
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon,
        BattleTrigger trigger = BattleTrigger.LongGrass)
    {
        this.PlayerParty = playerParty; 
        this.WildPokemon = wildPokemon;

        player = playerParty.GetComponent<PlayerController>();
        IsTrainerBattle = false;

        battleTrigger = trigger;

        AudioManager.i.PlayMusic(wildBattleMusic);//BGM開始
        
        StartCoroutine(SetupBattle());
    }
    
    //トレーナーバトル開始
    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty,
        BattleTrigger trigger = BattleTrigger.LongGrass)
    {
        this.PlayerParty = playerParty;
        this.TrainerParty = trainerParty;

        IsTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        battleTrigger = trigger;

        AudioManager.i.PlayMusic(trainerBattleMusic);//BGM開始

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        StateMachine = new StateMachine<BattleSystem>(this);
        
        //以前の状態をクリア
        playerUnit.Clear();
        enemyUnit.Clear();

        //背景の設定 
        backgroundImage.sprite = (battleTrigger == BattleTrigger.LongGrass) ? grassBackground : waterBackground;
        
        if (!IsTrainerBattle)
        {
            //wild Pokemon Battle
            playerUnit.Setup(PlayerParty.GetHealthyPokemon());
            enemyUnit.Setup(WildPokemon);

            //戦闘開始のダイアログ表示
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");
        }
        else
        {
            //Trainer Battle

            // Show trainer and player sprites
            playerUnit.gameObject.SetActive(false); 
            enemyUnit.gameObject.SetActive(false); 
            
            playerImage.gameObject.SetActive(true); 
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite; 
            trainerImage.sprite = trainer.Sprite;

            //戦闘開始のダイアログ表示
            yield return dialogBox.TypeDialog($"{trainer.Name} wants to battle");

            //トレーナーのポケモン呼び出し
            trainerImage.gameObject.SetActive(false); 
            enemyUnit.gameObject.SetActive(true); 
            var enemyPokemon = TrainerParty.GetHealthyPokemon(); 
            enemyUnit.Setup(enemyPokemon); 
            yield return dialogBox.TypeDialog($"{trainer.Name} send out {enemyPokemon.Base.Name}");

            //プレイヤーのポケモン呼び出し
            playerImage.gameObject.SetActive(false); 
            playerUnit.gameObject.SetActive(true);  
            var playerPokemon = PlayerParty.GetHealthyPokemon (); 
            playerUnit.Setup(playerPokemon); 
            yield return dialogBox.TypeDialog($"Go {playerPokemon.Base.Name}!");
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }

        isBattleOver = false;
        EscapeAttempts = 0;
        partyScreen.Init();//パーティ画面セット

        //行動選択ステート移行
        StateMachine.ChangeState(ActionSelectionState.i);
    }



    public void BattleOver(bool won)//戦闘終了
    {
        isBattleOver = true;
        //状態異常、HUDリセット
        PlayerParty.Pokemons.ForEach(p => p.OnBattleOver());
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();
        OnBattleOver(won);
    }

    public void HandleUpdate()
    {
        StateMachine.Excute();
    }

    public IEnumerator SwitchPokemon(Pokemon newPokemon)//ポケモン入れ替え
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            //ポケモンを戻す
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
        //入れ替るポケモンをセット
        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name} !");

    }

    public IEnumerator SendNextTrainerPokemon()//トレーナーの次のポケモン呼び出し
    {
        //次のポケモンとダイアログのセット
        var nextPokemon = TrainerParty.GetHealthyPokemon();
        enemyUnit.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{trainer.Name} send out {nextPokemon.Base.Name}!");
    }

    public IEnumerator ThrowPokeBall(PokeballItem pokeballItem)//ポケモンボール使用
    {

        //トレーナー戦の場合不可
        if (IsTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't steal the trainers pokemon!");
            yield break;
        }

        //ポケモンボール表示
        yield return dialogBox.TypeDialog($"{player.Name} used {pokeballItem.Name.ToUpper()}!");
        var pokeballObj = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = pokeballItem.Icon;

        // Animations
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(enemyUnit.Pokemon, pokeballItem);
        
        //ポケモンボールシェイク
        for (int i = 0; i < Mathf.Min(shakeCount, 3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if(shakeCount == 4)
        {
            //Pokemon is caught
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} was caught"); 
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();

            PlayerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} has been added to your party");

            Destroy(pokeball); 
            BattleOver(true);
        }
        else
        {
            //Pokemon break out
            yield return new WaitForSeconds(1f); 
            pokeball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakoutAnimation();

            if (shakeCount < 2) 
                yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base. Name} broke free"); 
            else 
                yield return dialogBox.TypeDialog($"Almost caught it"); 
            
            Destroy(pokeball); 
        }
    }

    //ポケモン捕獲の計算
    int TryToCatchPokemon(Pokemon pokemon, PokeballItem pokeballItem)
    {
        float a = (3 * pokemon.MaxHp - 2 * pokemon.HP) * pokemon.Base.CatchRate * pokeballItem.CatchRateModifier * ConditionsDB.GetStatusBonus(pokemon.Status) / (3 * pokemon.MaxHp);

        if (a >= 255) 
            return 4; 
        
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a)); 

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;


            ++shakeCount;
        }

        return shakeCount;
    }

    public BattleDialogBox DialogBox => dialogBox;

    public BattleUnit PlayerUnit => playerUnit;
    public BattleUnit EnemyUnit => enemyUnit;

    public PartyScreen PartyScreen => partyScreen;

    public AudioClip BattleVictoryMusic => battleVictoryMusic;
}
