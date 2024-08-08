using GDE.GenericSelectionUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PokemonStorageUI : SelectionUI<ImageSlot>
{
    [SerializeField] List<ImageSlot> boxSlots;
    [SerializeField] Image moveingPokemonImage;

    List<BoxPartySlotUI> partySlots = new List<BoxPartySlotUI>();
    List<BoxStorageSlotUI> storageSlots = new List<BoxStorageSlotUI>();

    List<Image> boxSlotImages = new List<Image>();

    PokemonParty party;
    PokemonStorageBoxes storageBoxes;

    int totalCokums = 7;
    public int SelectedBox { get; private set; } = 0;

    private void Awake()
    {
        foreach(var boxslot in boxSlots)
        {
            var storageSlot = boxslot.GetComponent<BoxStorageSlotUI>();
            if(storageSlot != null)
            {
                storageSlots.Add(storageSlot);
            }
            else
            {
                partySlots.Add(boxslot.GetComponent<BoxPartySlotUI>());
            }
        }

        party = PokemonParty.GetPokemonParty();
        storageBoxes = PokemonStorageBoxes.GetPlayerStorageBoxes();

        boxSlotImages = boxSlots.Select(b => b.transform.GetChild(0).GetComponent<Image>()).ToList();
    }

    private void Start()
    {
        SetItems(boxSlots);
        SetSelectionSettings(SelectionType.Grid, totalCokums);
    }

    public void SetDataInPartySlots()
    {
        for(int i = 0; i < partySlots.Count; i++)
        {
            if(i < party.Pokemons.Count)
                partySlots[i].SetData(party.Pokemons[i]);
            else
                partySlots[i].ClearData();
        }
    }

    public void SetDataInStorageSlots()
    {
        for (int i = 0; i < storageSlots.Count; i++)
        {
            var pokemon = storageBoxes.GetPoikemon(SelectedBox, i);

            if(pokemon != null)
                storageSlots[i].SetData(pokemon);
            else
                storageSlots[i].ClearData();
        }
    }

    public override void UpdateSelectionUI()
    {
        base.UpdateSelectionUI();

        if(moveingPokemonImage.gameObject.activeSelf)
            moveingPokemonImage.transform.position = boxSlotImages[selectedItem].transform.position + Vector3.up * 50f;
    }

    public bool IsPartySlot(int slotIndex)
    {
        return slotIndex % totalCokums == 0;
    }

    public Pokemon TakePokemonFromSlot(int slotIndex)
    {
        Pokemon pokemon;
        if (IsPartySlot(slotIndex))
        {
            int partyIndex = slotIndex / totalCokums;

            if (partyIndex >= party.Pokemons.Count)
                return null;

            pokemon = party.Pokemons[partyIndex];
            party.Pokemons[partyIndex] = null;
        }
        else
        {
            int boxSlotIndex = slotIndex - (slotIndex / totalCokums + 1);
            pokemon = storageBoxes.GetPoikemon(SelectedBox, boxSlotIndex);
            storageBoxes.RemovePokemon(SelectedBox, boxSlotIndex);
        }

        moveingPokemonImage.sprite = boxSlotImages[slotIndex].sprite;
        moveingPokemonImage.transform.position = boxSlotImages[slotIndex].transform.position + Vector3.up * 50f;
        boxSlotImages[slotIndex].color = new Color(1, 1, 1, 0);
        moveingPokemonImage.gameObject.SetActive(true);    

        return pokemon;
    }

    public void PutPokemonIntoSlot(Pokemon pokemon, int slotIndex)
    {
        if (IsPartySlot(slotIndex))
        {
            int partyIndex = slotIndex / totalCokums;

            if(partyIndex >= party.Pokemons.Count)
                party.Pokemons.Add(pokemon);
            else
                party.Pokemons[partyIndex] = pokemon;
        }
        else
        {
            int boxSlotIndex = slotIndex - (slotIndex / totalCokums + 1);
            storageBoxes.AddPokemon(pokemon, SelectedBox, boxSlotIndex);
        }

        moveingPokemonImage.gameObject.SetActive(false);

    }
}
