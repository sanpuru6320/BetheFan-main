using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageSlot : MonoBehaviour, ISelectableItem
{
    Image bgImage;

    void Awake()
    {
        bgImage = GetComponent<Image>();
    }

    Color orignalColor;

    public void Init()
    {
        orignalColor = bgImage.color;
    }

    public void Clear()
    {
        bgImage.color = orignalColor;
    }

    public void OnSelectionChanged(bool selected)//選択中スロットハイライトの処理
    {
        bgImage.color = (selected) ? GlobalSettings.i.BgHighlightedColor : orignalColor;
    }
}
