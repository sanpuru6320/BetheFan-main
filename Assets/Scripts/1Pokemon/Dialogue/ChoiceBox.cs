using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceBox : MonoBehaviour
{
    [SerializeField] ChoiceText choiceTextPrefab;

    bool choiceSelected = false;

    List<ChoiceText> choiceTexts;
    int currentChoice;
    
    public IEnumerator ShowChoices(List<string> choices, Action<int> onChoiceSelected)
    {
        choiceSelected = false;
        currentChoice = 0;
        
        gameObject.SetActive(true);

        //Delete existing choices
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        //選択肢の生成
        choiceTexts = new List<ChoiceText>();
        foreach(var choice in choices)
        {
            var choiceTextObj = Instantiate(choiceTextPrefab, transform);
            choiceTextObj.TextField.text = choice;
            choiceTexts.Add(choiceTextObj);
        }

        yield return new WaitUntil(() => choiceSelected == true);

        onChoiceSelected?.Invoke(currentChoice);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentChoice;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentChoice;

        currentChoice = Mathf.Clamp(currentChoice, 0, choiceTexts.Count - 1); //currentChoiceの範囲の設定

        for (int i = 0;i < choiceTexts.Count; i++)//選択中のテキストの判定
        {
            choiceTexts[i].SetSelected(i == currentChoice);
        }

        if (Input.GetKeyDown(KeyCode.Z))//選択入力
            choiceSelected = true;
    }
}
