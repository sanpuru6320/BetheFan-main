using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour//ポケモン回復NPC
{
    public IEnumerator Heal(Transform player, Dialog dialog)
    {
        int selectedChoice = 0;
        
        yield return DialogManager.Instance.ShowDialogText("You look tired! Would you like to rest here?",
           choices: new List<string>() { "Yes please","No thanks" },
            onChoiceSelected: (choiceIndex) => selectedChoice = choiceIndex);

        if(selectedChoice == 0)
        {
            //Yes
            yield return Fader.i.FadeIn(0.5f);

            //パーティのポケモンを回復
            var playerParty = player.GetComponent<PokemonParty>();
            playerParty.Pokemons.ForEach(p => p.Heal());
            playerParty.PartyUpdated();

            yield return Fader.i.FadeOut(0.5f);

            yield return DialogManager.Instance.ShowDialogText($"Your pokemon should be fully healed now");
        }
        else if(selectedChoice == 1)
        {
            //No
            yield return DialogManager.Instance.ShowDialogText($"Okay! Come back if you change your mind");
        }
            




    }
}
