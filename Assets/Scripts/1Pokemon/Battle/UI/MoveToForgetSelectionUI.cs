﻿using GDE.GenericSelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MoveToForgetSelectionUI : SelectionUI<TextSlot>//忘れさせる技UI
{
    [SerializeField] List<Text> moveTexts;
    [SerializeField] Color highlightedColor;
    
    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove) 
    { 
        for (int i = 0; i < currentMoves.Count; ++i) 
        {
            moveTexts[i].text = currentMoves[i].Name; 
        }

        moveTexts[currentMoves.Count].text = newMove.Name;

        SetItems(moveTexts.Select(m => m.GetComponent<TextSlot>()).ToList());
    }
}
