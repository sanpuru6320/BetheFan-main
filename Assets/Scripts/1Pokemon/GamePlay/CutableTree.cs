using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutableTree : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiator)
    {
       yield return DialogManager.Instance.ShowDialogText("This tree looks like can be cut");

        var pokemonWithCut = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(mbox => mbox.Base.Name == "Cut"));

        if(pokemonWithCut != null)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"Should {pokemonWithCut.Base.Name} use cut?",
                choices: new List<string>() { "Yes", "No"},
                onChoiceSelected: (selection) => selectedChoice = selection);

            if(selectedChoice == 0)
            {
                //Yes
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWithCut.Base.Name} used cut!");
                gameObject.SetActive(false);
            }
        }
    }

}
