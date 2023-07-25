using GDEUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionState : State<GameController>
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image pokemonImage;

    [SerializeField] AudioClip evolutionMusic;

    public static EvolutionState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public IEnumerator Evolve(Pokemon pokmeon, Evolution evolution)
    {
        var gc = GameController.Instance;
        gc.StateMachine.Push(this);
        
        evolutionUI.SetActive(true);

        AudioManager.i.PlayMusic(evolutionMusic);

        pokemonImage.sprite = pokmeon.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialogText($"{pokmeon.Base.Name} is evloving");

        var oldPokemon = pokmeon.Base;
        pokmeon.Evolve(evolution);

        pokemonImage.sprite = pokmeon.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialogText($"{oldPokemon.Name} evloved into {pokmeon.Base.Name}");

        evolutionUI.SetActive(false);

        gc.PartyScreen.SetPartyData();
        AudioManager.i.PlayMusic(gc.CurrentScene.SceneMusic, fade: true);

        gc.StateMachine.Pop();
    }
}
