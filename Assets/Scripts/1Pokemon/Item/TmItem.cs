﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Item/Create new Tm or Hm")]
public class TmItem : ItemBase
{
    [SerializeField] MoveBase move;
    [SerializeField] bool isHM;

    public override string Name => base.Name + $": {move.Name}";

    public override bool Use(Pokemon pokemon)
    {
        // Learning move is hamdled from Inventory UI, if it was learned them return true
        return pokemon.HasMove(move);
    }

    public bool CanBeTaught(Pokemon pokemon)
    {
        return pokemon.Base.LearnableByItem.Contains(move);
    }

    public override bool IsReuseable => isHM;

    public override bool CanUseInBattle => false;

    public MoveBase Move => move;

    public bool IsHM => isHM;
}
