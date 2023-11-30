using GDE.GenericSelectionUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicMenuUI : SelectionUI<TextSlot>
{
    public static DynamicMenuUI i;
    
    private void Awake()
    {
        i = this;
    }
}
