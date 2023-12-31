﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    //アイテム基本設定
    public string nameItem;
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    [SerializeField] float price;
    [SerializeField] bool isSellable;

    public virtual string Name => nameItem;
    public string Description => description;
    public Sprite Icon => icon;

    public float Price => price;
    public bool IsSellable => isSellable;

    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }

    public virtual void UseItem(int charToUseOn, ItemBase itemBase)
    {

    }

    public virtual bool IsReuseable => false;

    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;
}
