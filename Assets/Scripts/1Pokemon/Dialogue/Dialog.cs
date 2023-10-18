using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog//表示する会話
{
    [SerializeField] List<string> lines;
    public List<string> Lines
    {
        get { return lines; }
    }
}
