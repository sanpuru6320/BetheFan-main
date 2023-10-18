using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour,ISaveable
{
    [SerializeField] float money;

    public event Action OnMoneyChanged; //所持金変更イベント

    public static Wallet i { get; private set; }

    public void Awake()
    {
        i = this;
    }

    public void AddMoney(float amount)
    {
        money += amount;
        OnMoneyChanged?.Invoke();
    }

    public void TakeMoney(float amount)
    {
        money -= amount;
        OnMoneyChanged?.Invoke();
    }

    public bool HasMoney(float amount)
    {
        return amount <= money;
    }

    public object CaptureState()
    {
        return money;
    }

    public void RestoreState(object state)
    {
        money = (float)state;
    }

    public float Money => money;
}
