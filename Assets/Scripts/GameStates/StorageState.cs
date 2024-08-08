using GDEUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class StorageState : State<GameController>
{
    [SerializeField] PokemonStorageUI storageUI;

    bool isMovingPokemon = false;
    int selectedSlotToMove = 0;
    Pokemon selectedPokemonToMove = null;

    PokemonParty party;

    public static StorageState i { get; private set; }
    private void Awake()
    {
        i = this;
        party = PokemonParty.GetPokemonParty();
    }

    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        storageUI.gameObject.SetActive(true);
        storageUI.SetDataInPartySlots();
        storageUI.SetDataInStorageSlots();

        storageUI.OnSelected += OnSlotSelected;
        storageUI.OnBack += OnBack;
    }

    public override void Excute()
    {
        storageUI.HandleUpdete();
    }

    public override void Exit()
    {
        storageUI.gameObject.SetActive(false);
        storageUI.OnSelected -= OnSlotSelected;
        storageUI.OnBack -= OnBack;
    }

    void OnSlotSelected(int slotIndex)
    {
        if (!isMovingPokemon)
        {
            var pokemon = storageUI.TakePokemonFromSlot(slotIndex);
            if (pokemon != null)
            {
                isMovingPokemon = true;
                selectedSlotToMove = slotIndex;
                selectedPokemonToMove = pokemon;
            }
        }
        else
        {
            isMovingPokemon = false;

            int firstSlotIndex = selectedSlotToMove;
            int secondSlotIndex = slotIndex;

            var secondPokemon = storageUI.TakePokemonFromSlot(slotIndex);

            if (secondPokemon == null && storageUI.IsPartySlot(firstSlotIndex) && storageUI.IsPartySlot(secondSlotIndex))
            {
                storageUI.PutPokemonIntoSlot(selectedPokemonToMove, selectedSlotToMove);

                storageUI.SetDataInStorageSlots();
                storageUI.SetDataInPartySlots();
                return;
            }

            storageUI.PutPokemonIntoSlot(selectedPokemonToMove, secondSlotIndex);

            if(secondPokemon != null)
                storageUI.PutPokemonIntoSlot(secondPokemon, firstSlotIndex);
         

            party.Pokemons.RemoveAll(p => p == null);
            party.PartyUpdated();

            storageUI.SetDataInStorageSlots();
            storageUI.SetDataInPartySlots();
        }
    }

    void OnBack()
    {
        if(isMovingPokemon)
        {
            isMovingPokemon = false;
            storageUI.PutPokemonIntoSlot(selectedPokemonToMove, selectedSlotToMove);
        }
        else
        {
            gc.StateMachine.Pop();
        }
    }
}
