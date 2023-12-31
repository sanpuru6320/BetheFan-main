﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecound;

    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject choiceBox;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text ppTexts;
    [SerializeField] Text typeTexts;

    [SerializeField] Text yesText;
    [SerializeField] Text noText;

    Color highlightedColor;

    private void Start()
    {
        highlightedColor = GlobalSettings.i.HighlightedColor;
    }

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)//テキストを一文字ずつ順に表示
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecound);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void EnableChoiceBox(bool enabled) 
    { 
        choiceBox.SetActive(enabled);
    }

    public bool IsChoiceBoxEnabled => choiceBox.activeSelf;
    public void UpdateActionSelection(int selectedAction)
    {
        for(int i=0; i<actionTexts.Count; ++i)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = highlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i == selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }

        ppTexts.text = $"PP {move.PP}/{move.Base.PP}";
        typeTexts.text = move.Base.Type.ToString();

        if (move.PP == 0)
            ppTexts.color = Color.red; 
        else 
            ppTexts.color = Color.black;
    }

    public void SetMoveNames(List<Move> moves)//技をセット
    {
        for(int i=0; i<moveTexts.Count; ++i)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.Name;
            else
                moveTexts[i].text = "";
        }
    }

    public void UpdateChoiceBox(bool yesSelected)//選択肢ハイライト切り替え
    {
        if (yesSelected)
        {
            yesText.color = highlightedColor;
            noText.color = Color.black;
        }

        else 
        {
            yesText.color = Color.black;
            noText.color = highlightedColor;
        }
    }
}
