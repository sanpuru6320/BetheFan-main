using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalletUI : MonoBehaviour//所持金UI
{
    [SerializeField] Text moneyTxt;

    private void Start()
    {
        Wallet.i.OnMoneyChanged += SetMoneyTxt;
    }

    public void Show()//所持金表示
    {
        gameObject.SetActive(true);
        SetMoneyTxt();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void SetMoneyTxt()
    {
        moneyTxt.text = "$ " + Wallet.i.Money;
    }
}
