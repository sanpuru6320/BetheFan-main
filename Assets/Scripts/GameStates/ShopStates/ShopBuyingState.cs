using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBuyingState : State<GameController>
{
    [SerializeField] Vector2 shopCameraOffset;
    [SerializeField] ShopUI shopUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;

    public static ShopBuyingState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    Inventory inventory;

    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    // Input 
    public List<ItemBase> AvailableItems { get; set; }
    
    bool browseItems = false;

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        StartCoroutine(StartBuyingState());
    }

    public override void Excute()
    {
        if (browseItems)
        {
            shopUI.HandleUpdete();
        }
    }

    IEnumerator StartBuyingState()
    {
        yield return GameController.Instance.MoveCamera(shopCameraOffset);
        walletUI.Show();
        shopUI.Show(AvailableItems, (item) => StartCoroutine(BuyItem(item)),
            () => StartCoroutine(onBackFromBuying()));

        browseItems = true;
    }

    IEnumerator BuyItem(ItemBase item)
    {
        browseItems = false;
        
        yield return DialogManager.Instance.ShowDialogText($"How many would you like to buy?",
            waitForInput: false, autoClose: false);

        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(100, item.Price,
            (selectedCount) => countToBuy = selectedCount);

        DialogManager.Instance.CloseDialog();

        float totalPrice = item.Price * countToBuy;

        if (Wallet.i.HasMoney(totalPrice))
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"That will be {totalPrice}",
                waitForInput: false,
                choices: new List<string>() { "Yes", "No" },
                onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

            if (selectedChoice == 0)
            {
                //Selected Yes
                inventory.AddItem(item, countToBuy);
                Wallet.i.TakeMoney(totalPrice);
                yield return DialogManager.Instance.ShowDialogText($"Thank you for shopping with us!");
            }
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"Not enough money for that!");
        }

        browseItems = true;
    }

    IEnumerator onBackFromBuying()
    {
        yield return GameController.Instance.MoveCamera(-shopCameraOffset);
        shopUI.Close();
        walletUI.Close();
        gc.StateMachine.Pop();
    }
}
