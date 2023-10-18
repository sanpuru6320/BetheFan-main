using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryState : State<GameController>
{
    [SerializeField] InventoryUI inventoryUI;

    //Output
    public ItemBase SeletedItem { get; private set; }
    public static InventoryState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gc;

    Inventory inventory;

    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    public override void Enter(GameController owner)
    {
        gc = owner;

        SeletedItem = null;
        
        inventoryUI.gameObject.SetActive(true);
        inventoryUI.OnSelected += OnItemSelected;
        inventoryUI.OnBack += OnBack;
    }

    public override void Exit()
    {
        inventoryUI.gameObject.SetActive(false);
        inventoryUI.OnSelected -= OnItemSelected;
        inventoryUI.OnBack -= OnBack;
    }

    public override void Excute()
    {
        inventoryUI.HandleUpdete();
    }

    void OnItemSelected(int selection)
    {
        SeletedItem = inventoryUI.SelectedItem;

        if (gc.StateMachine.GetPrevState() != ShopSellingState.i)
            StartCoroutine(SelectPokemonAndUseItem());
        else
            gc.StateMachine.Pop();
    }

    void OnBack()
    {
        SeletedItem = null;
        gc.StateMachine.Pop();
    }

    //アイテム使用可能か判定
    IEnumerator SelectPokemonAndUseItem()
    {
        var prevState = gc.StateMachine.GetPrevState();
        if (prevState == BattleState.i)
        {
            //in Battle
            if (!SeletedItem.CanUseInBattle)
            {
                yield return DialogManager.Instance.ShowDialogText("This item cannot be used in battle");
                yield break;
            }
        }
        else
        {
            //Outside Battle
            if (!SeletedItem.CanUseOutsideBattle)
            {
                yield return DialogManager.Instance.ShowDialogText("This item cannot be used in outside battle");
                yield break;
            }

        }

        if (SeletedItem is PokeballItem)
        {

            inventory.UseItem(SeletedItem, null);
            gc.StateMachine.Pop();
            yield break;
        }
        
        yield return gc.StateMachine.PushAndWait(PartyState.i);

        
        if(prevState == BattleState.i)
        {
            if(UseItemState.i.ItemUsed)
                gc.StateMachine.Pop();
        }
    }
}
