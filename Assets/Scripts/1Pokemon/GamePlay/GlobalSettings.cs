using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour//UIテキストのハイライト
{
    [SerializeField] Color highlightedColor;
    [SerializeField] Color bghighlightedColor;

    public Color HighlightedColor => highlightedColor;
    public Color BgHighlightedColor => bghighlightedColor;

    public static GlobalSettings i { get; private set; }

    private void Awake()
    {
        i = this;
    }
}
