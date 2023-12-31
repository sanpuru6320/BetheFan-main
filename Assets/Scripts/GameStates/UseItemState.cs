using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UseItemState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;
    
    //Output
    public bool ItemUsed { get; private set; }
    public static UseItemState i { get; private set;}
    Inventory inventory;

    private void Awake()
    {
        i = this;
        inventory = Inventory.GetInventory();
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        ItemUsed = false;

        StartCoroutine(UseItem());
    }

    IEnumerator UseItem()
    {

        var item = inventoryUI.SelectedItem;
        var pokemon = partyScreen.SelectedMember;

        if(item is TmItem)//技マシン使用
        {
            yield return HandleTmItem();
        }
        else
        {
            if (item is EvolutionItem)//進化アイテム使用
            {
                var evolution = pokemon.CheckForEvolution(item);
                if (evolution != null)
                {
                    yield return EvolutionState.i.Evolve(pokemon, evolution);
                }
                else
                {

                    yield return DialogManager.Instance.ShowDialogText($"it won't have any affect!");
                    gc.StateMachine.Pop();
                    yield break;
                }
            }

            var usedItem = inventory.UseItem(item, partyScreen.SelectedMember);
            if (usedItem != null)
            {
                ItemUsed = true;
                
                if ((usedItem is RecoveryItem))//回復アイテム使用
                    yield return DialogManager.Instance.ShowDialogText($"The player used {usedItem.Name}");
            }
            else
            {
                if (inventoryUI.SelectedCategory == (int)ItemCategory.Item)
                    yield return DialogManager.Instance.ShowDialogText($"It won't have any affect!");
            }
        }
        gc.StateMachine.Pop();
    }

    IEnumerator HandleTmItem()//技マシンの判定
    {
        var tmItem = inventoryUI.SelectedItem as TmItem;
        if (tmItem == null)
            yield break;

        var pokemon = partyScreen.SelectedMember;

        if (pokemon.HasMove(tmItem.Move))//既知の技
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} already know {tmItem.Move.Name}");
            yield break;
        }

        if (!tmItem.CanBeTaught(pokemon))//覚えることが不可
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} can't learn {tmItem.Move.Name}");
            yield break;
        }

        if (pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)//技の上限以下
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} learned {tmItem.Move.Name}");
        }
        else//技の上限が超えている
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} is trying to learn {tmItem.Move.Name}");
            yield return DialogManager.Instance.ShowDialogText($"But it cannot learn more than {PokemonBase.MaxNumOfMoves} noves");

            yield return DialogManager.Instance.ShowDialogText($"Choose a move you want to forget", true, false);

            //技を忘れさせる
            MoveToForgetState.i.CurrentMoves = pokemon.Moves.Select(m => m.Base).ToList();
            MoveToForgetState.i.NewMove = tmItem.Move;
            yield return gc.StateMachine.PushAndWait(MoveToForgetState.i);

            var moveIndex = MoveToForgetState.i.Selection;
            if (moveIndex == PokemonBase.MaxNumOfMoves || moveIndex == -1)
            {
                // Don't learn the new move
                yield return StartCoroutine(DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} did not learn {tmItem.Move.Name}"));
            }
            else
            {
                // Forget the selected move and learn new move
                var selectedMove = pokemon.Moves[moveIndex].Base;
                yield return StartCoroutine(DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} forgot {selectedMove.Name} and learned {tmItem.Move.Name}"));
                pokemon.Moves[moveIndex] = new Move(tmItem.Move);
            }
        }
    }
}
