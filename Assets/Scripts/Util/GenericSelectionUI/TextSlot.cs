using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextSlot : MonoBehaviour, ISelectableItem//UIのアイテムやポケモンの各スロット
{
    [SerializeField] Text text;
    [SerializeField] TextMeshProUGUI tMPro;

    [SerializeField] Color originalColor;
    Color color;

    public void Init()
    {
        color = new Color(0, 0, 0, 1);
        originalColor = color;
    }

    public void Clear()
    {
        text.color = originalColor;
    }

    public void OnSelectionChanged(bool selected)//選択中スロットハイライトの処理
    {
       text.color = (selected) ? GlobalSettings.i.HighlightedColor : originalColor;
    }

    public void SetText(string s)
    {
        text.text = s;
    }
}
