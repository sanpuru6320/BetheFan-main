using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;

    public Pokemon SelectedPokemon { get; private set; }
    public static PartyState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        SelectedPokemon = null;
        partyScreen.gameObject.SetActive(true);
        partyScreen.OnSelected += OnPokemonSelected;
        partyScreen.OnBack += OnBack;
    }

    public override void Excute()
    {
        partyScreen.HandleUpdete();
    }

    public override void Exit()
    {
        partyScreen.gameObject.SetActive(false);
        partyScreen.OnSelected -= OnPokemonSelected;
        partyScreen.OnBack -= OnBack;
    }

    void OnPokemonSelected(int selection)
    {
        SelectedPokemon = partyScreen.SelectedMember;
        
        var prevState = gc.StateMachine.GetPrevState();//一つ前のステート取得
        if(gc.StateMachine.GetPrevState() == InventoryState.i)//インベントリからアイテムを使用している場合
        {
            //Use Item
            StartCoroutine(GoToUseItemState());
            
        }
        else if(prevState == BattleState.i)
        {
            var battleState = prevState as BattleState;

            if (SelectedPokemon.HP <= 0)//瀕死の場合
            {
                partyScreen.SetMessageText("You can't send out a fainted pokemon");
                return;
            }
            if (SelectedPokemon == battleState.BattleSystem.PlayerUnit.Pokemon)//既に場に出ている場合
            {
                partyScreen.SetMessageText("You can't switch with the same pokemon");
                return;
            }

            gc.StateMachine.Pop();
        }
        else
        {
            //Todo : Open summary screen
        }
    }

    IEnumerator GoToUseItemState()
    {
        yield return gc.StateMachine.PushAndWait(UseItemState.i);
        gc.StateMachine.Pop();
    }

    void OnBack()
    {
        SelectedPokemon = null;
        
        var prevState = gc.StateMachine.GetPrevState();
        if(prevState == BattleState.i)
        {
            var battleState = prevState as BattleState;
            if (battleState.BattleSystem.PlayerUnit.Pokemon.HP <= 0)//戦闘でHPが0になった場合
            {
                partyScreen.SetMessageText("You have to choose a pokemon to continue");
                return;
            }
        }

        gc.StateMachine.Pop();
    }
}
