﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable, ISaveable
{
    [Header("Quests")]
    [SerializeField] Dialog dialog;
    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete;

    [Header("Movement")]
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;
    Quests activeQuest;

    Character character;
    ItemGiver itemGiver;
    PokemonGiver pokemonGiver;
    Healer healer;
    Merchant merchant;

    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        pokemonGiver = GetComponent<PokemonGiver>();
        healer = GetComponent<Healer>();
        merchant = GetComponent<Merchant>();
    }
    public IEnumerator Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);

            if(questToComplete != null)
            {
                var quest = new Quests(questToComplete);
                yield return quest.CompletedQuest(initiator);
                questToComplete = null;

                Debug.Log($"{quest.Base.Name} completed");
            }

            if(itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if (pokemonGiver != null && pokemonGiver.CanBeGiven())
            {
                yield return pokemonGiver.GivePokemon(initiator.GetComponent<PlayerController>());
            }
            else if(questToStart != null)
            {
                activeQuest = new Quests(questToStart);
                yield return activeQuest.StartQuest();
                questToStart = null;

                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompletedQuest(initiator);
                    activeQuest = null;
                }
            }
            else if(activeQuest != null)
            {
                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompletedQuest(initiator);
                    activeQuest = null;
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialog(activeQuest.Base.InPrigressDialogue);
                }
            }
            else if(healer != null)
            {
                yield return healer.Heal(initiator, dialog);
            }
            else if(merchant != null)
            {
                yield return merchant.Trade();
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog(dialog);
            }

            idleTimer = 0f;
            state = NPCState.Idle;
        }
    }

    private void Update()
    {
        if(state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if(movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }
        
        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;
        
        yield return character.Move(movementPattern[currentPattern]); 

        if(transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count; 
        
        state = NPCState.Idle;
    }

    public object CaptureState()
    {
        var saveData = new NPCQuestSaveData();
        saveData.activeQuest = activeQuest?.GetSaveData();

        if (questToStart != null) //nullの場合エラーがでるので先に判定
            saveData.questToStart = (new Quests(questToStart)).GetSaveData();//QuestBaseクラスをQuestクラスにConvertしてセーブできるようにする

        if (questToComplete != null)
            saveData.questToComplete = (new Quests(questToComplete)).GetSaveData();

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as NPCQuestSaveData;
        if(saveData != null)
        {
            activeQuest = (saveData.activeQuest != null) ? new Quests(saveData.activeQuest): null;

            questToStart = (saveData.questToStart != null) ? new Quests(saveData.questToStart).Base: null;
            questToComplete = (saveData.questToComplete != null) ? new Quests(saveData.questToComplete).Base: null;
        }
    }
}

[System.Serializable]
public class NPCQuestSaveData
{
    public QuestSaveData activeQuest;
    public QuestSaveData questToStart;
    public QuestSaveData questToComplete;
}

public enum NPCState { Idle, Walking, Dialog }
