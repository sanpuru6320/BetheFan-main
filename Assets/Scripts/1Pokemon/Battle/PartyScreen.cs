using GDE.GenericSelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : SelectionUI<TextSlot>//プレイヤーのパーティ
{
    [SerializeField] Text messageText; 
    
    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemons;
    PokemonParty party;

    public Pokemon SelectedMember => pokemons[selectedItem];

    public void Init()//パーティ画面初期化
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        SetSelectionSettings(SelectionType.Grid, 2);

        party = PokemonParty.GetPokemonParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }

    public void SetPartyData()
    {

        pokemons = party.Pokemons;

        ClearItems();

        for (int i = 0; i < memberSlots.Length; i++)//ポケモンの数だけスロットに表示
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(pokemons[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);

           
        }
        
        //アクティブなスロットのみ取得
        var textSlots = memberSlots.Select(m => m.GetComponent<TextSlot>());
        SetItems(textSlots.Take(pokemons.Count).ToList());
        
        messageText.text = "Choose a Pokemon";//案内テキスト表示
    }

    public void ShowIfTmIsUseable(TmItem tmItem)
    {
        for(int i = 0; i < pokemons.Count; i++)
        {
            string message = tmItem.CanBeTaught(pokemons[i]) ? "ABLE!" : "NOT ABLE";
            memberSlots[i].SetMessage(message);

        }
    }

    public void ClearMemberSlotMessage()
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
           memberSlots[i].SetMessage("");

        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}


