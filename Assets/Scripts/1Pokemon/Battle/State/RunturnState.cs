using GDEUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RunturnState : State<BattleSystem>
{
    public static RunturnState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    BattleUnit playerUnit;
    BattleUnit enemyUnit;
    BattleDialogBox dialogBox;
    PartyScreen partyScreen;
    bool isTrainerBattle;
    PokemonParty playerParty;
    PokemonParty trainerParty;

    BattleSystem bs;

    public override void Enter(BattleSystem owner)
    {
        bs = owner;

        playerUnit = bs.PlayerUnit;
        enemyUnit = bs.EnemyUnit;
        dialogBox = bs.DialogBox;
        partyScreen = bs.PartyScreen;
        playerParty = bs.PlayerParty;
        trainerParty = bs.TrainerParty;

        StartCoroutine(RunTurns(bs.SelectedAction));
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {

        if (playerAction == BattleAction.Move)//技を選択
        {
            //使用する技取得
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[bs.SelectedMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            //技の優先度取得
            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            //優先度判定
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)//優先度が同じ場合素早さで判定
                playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;

            //行動順にポケモンセット
            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Pokemon;

            // First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (bs.isBattleOver) yield break;

            if (secondPokemon.HP > 0)
            {
                // Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (bs.isBattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)//入れ替えを選択
            {
                yield return bs.SwitchPokemon(bs.SelectedPokemon);
            }
            else if (playerAction == BattleAction.UseItem)//アイテム仕様を選択
            {
                if(bs.SelectedItem is PokeballItem)
                {
                    yield return bs.ThrowPokeBall(bs.SelectedItem as PokeballItem);
                    if (bs.isBattleOver) yield break;
                }
                else
                {
                    // This is handle from item screen, so do nothing and skip to enemy move
                }


                //dialogBox.EnableActionSelector(false);
            }
            else if (playerAction == BattleAction.Run)//逃走を選択
            {
                yield return TryToEscape();
            }

            // Enemy Turn
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (bs.isBattleOver) yield break;
        }

        if (!bs.isBattleOver)
            bs.StateMachine.ChangeState(ActionSelectionState.i);
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        //行動可能か判定
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            //ステータス更新
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.WaitForHPUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Pokemon);

        move.PP--;//PP消費
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.name}");//行動ダイアログ

        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))//技がヒットした時
        {

            sourceUnit.PlayAttackAnimation();
            AudioManager.i.PlaySfx(move.Base.Sound);

            yield return new WaitForSeconds(1f);

            targetUnit.PlayHitAnimation();
            AudioManager.i.PlaySfx(AudioId.Hit);


            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else
            {
                //ダメージ処理
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.WaitForHPUpdate();
                yield return ShowDamageDetails(damageDetails);
            }

            //追加効果
            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                }
            }

            if (targetUnit.Pokemon.HP <= 0)//戦闘不能
            {
                yield return HandlePokemonFainted(targetUnit);
            }

        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed");
        }


    }

    //状態異常適用
    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {

        //Stat Boosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        //Status Condition
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        //Volatile Status Condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (bs.isBattleOver) yield break;

        //状態異常(炎、麻痺等の継続効果)
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.WaitForHPUpdate();
        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
        }
    }

    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)//ダメージ計算
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[StatPoke.Accuracy];
        int evasion = target.StatBoosts[StatPoke.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];
        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)//ポケモン戦闘不能
    {
        //戦闘不能ダイアログ、アニメーション
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} Fainted");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit)//相手が戦闘不能
        {
            bool battleWon = true;
            if (bs.IsTrainerBattle)//トレーナーバトルの場合、残りのポケモンがいるかチェック
                battleWon = trainerParty.GetHealthyPokemon() == null;

            if (battleWon)
                AudioManager.i.PlayMusic(bs.BattleVictoryMusic);

            // Exp Gain
            int expYield = faintedUnit.Pokemon.Base.ExpYield;
            int enemyLevel = faintedUnit.Pokemon.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            playerUnit.Pokemon.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} gained {expGain} exp");
            yield return playerUnit.Hud.SetExpSmooth();

            // Check Level Up
            while (playerUnit.Pokemon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level}");

                // Try to learn a new Move
                var newMove = playerUnit.Pokemon.GetLearnableMoveAtCurrLevel();
                if (newMove != null)
                {
                    if (playerUnit.Pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
                    {
                        playerUnit.Pokemon.LearnMove(newMove.Base);
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} learned {newMove.Base.Name}");
                        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
                    }
                    else//技の上限が最大数の場合
                    {
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} trying to learn {newMove.Base.Name}");
                        yield return dialogBox.TypeDialog($"But it cannot learn more than {PokemonBase.MaxNumOfMoves} moves");
                        yield return dialogBox.TypeDialog($"Choose a move a move to forget");

                        //忘れる技を選択
                        MoveToForgetState.i.CurrentMoves = playerUnit.Pokemon.Moves.Select(m => m.Base).ToList();
                        MoveToForgetState.i.NewMove = newMove.Base;
                        yield return GameController.Instance.StateMachine.PushAndWait(MoveToForgetState.i);

                        var moveIndex = MoveToForgetState.i.Selection;
                        if (moveIndex == PokemonBase.MaxNumOfMoves || moveIndex == -1)
                        {
                            // Don't learn the new move
                            yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{playerUnit.Pokemon.Base.Name} did not learn {newMove.Base.Name}"));
                        }
                        else
                        {
                            // Forget the selected move and learn new move
                            var selectedMove = playerUnit.Pokemon.Moves[moveIndex].Base;
                            yield return StartCoroutine(DialogManager.Instance.TypeDialog($"{playerUnit.Pokemon.Base.Name} forgot {selectedMove.Name} and learned {newMove.Base.Name}"));
                            playerUnit.Pokemon.Moves[moveIndex] = new Move(newMove.Base);
                        }
                    }
                }

                yield return playerUnit.Hud.SetExpSmooth(true);//Expバーの表示を変更
            }

            yield return new WaitForSeconds(1f);
        }

        yield return CheckForBattleOver(faintedUnit);
    }


    IEnumerator CheckForBattleOver(BattleUnit faintedunit)//戦闘終了かチェック
    {
        if (faintedunit.IsPlayerUnit)//プレイヤーのポケモンが戦闘不能
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
                yield return bs.SwitchPokemon(PartyState.i.SelectedPokemon);
            }
            else
                bs.BattleOver(false);
        }
        else//相手が戦闘不能
        {
            if (!isTrainerBattle)
            {
                bs.BattleOver(true);
            }
            else//トレーナーが相手
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                {
                    AboutToUseState.i.NewPokemon = nextPokemon;
                    yield return bs.StateMachine.PushAndWait(AboutToUseState.i);
                }
                else
                    bs.BattleOver(true);
            }
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)//ダメージダイアログ表示
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");
        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very super effective!");
    }

    IEnumerator TryToEscape()//逃走処理
    {
        if (isTrainerBattle)//トレーナー戦の場合
        {
            yield return dialogBox.TypeDialog("You can't run from trainer battles!");
            yield break;
        }

        //逃走判定
        ++bs.EscapeAttempts;

        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;

        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Ran away safely!");
            bs.BattleOver(true);

        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * bs.EscapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"Ran away safely!");
                bs.BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"Can't escape!");
            }
        }
    }
}
