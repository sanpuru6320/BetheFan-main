using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour//商人
{
    [SerializeField] List<ItemBase> availableItems;//販売アイテム
    public IEnumerator Trade()
    {
        ShopMenuState.i.AvailableItems = availableItems;
        yield return GameController.Instance.StateMachine.PushAndWait(ShopMenuState.i);
    }

    public List<ItemBase> AvaliableItems => availableItems;
}
