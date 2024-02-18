using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class StorageState : State<GameController>
{
    [SerializeField] PokemonStorageUI storageUI;

    public static StorageState i { get; private set; }
    private void Awake()
    {
        i = this; 
    }

    public override void Enter(GameController owner)
    {
        storageUI.gameObject.SetActive(true);
    }

    public override void Excute()
    {
        storageUI.HandleUpdete();
    }

    public override void Exit()
    {
        storageUI.gameObject.SetActive(false);
    }
}
