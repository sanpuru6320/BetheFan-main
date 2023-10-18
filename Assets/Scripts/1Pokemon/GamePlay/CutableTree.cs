using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutableTree : MonoBehaviour, Interactable//木のオブジェクトの削除
{
    public IEnumerator Interact(Transform initiator)
    {
       yield return DialogManager.Instance.ShowDialogText("This tree looks like can be cut");

        var pokemonWithCut = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(mbox => mbox.Base.Name == "Cut"));//パーティのポケモンにCutの技があるか判定

        if (pokemonWithCut != null)
        {
            //木を切るか選択
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"Should {pokemonWithCut.Base.Name} use cut?",
                choices: new List<string>() { "Yes", "No"},
                onChoiceSelected: (selection) => selectedChoice = selection);

            if(selectedChoice == 0)
            {
                //Yes
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWithCut.Base.Name} used cut!");
                gameObject.SetActive(false);//木の削除
            }
        }
    }

}
