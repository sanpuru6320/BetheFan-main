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

    //ポケモン進化イベント
    public IEnumerator Evolve(Pokemon pokmeon, Evolution evolution)
    {
        var gc = GameController.Instance;
        gc.StateMachine.Push(this);
        
        //進化画面表示
        evolutionUI.SetActive(true);

        AudioManager.i.PlayMusic(evolutionMusic);

        pokemonImage.sprite = pokmeon.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialogText($"{pokmeon.Base.Name} is evloving");

        var oldPokemon = pokmeon.Base;
        //ステータスやスプライト更新
        pokmeon.Evolve(evolution);

        pokemonImage.sprite = pokmeon.Base.FrontSprite;
        yield return DialogManager.Instance.ShowDialogText($"{oldPokemon.Name} evloved into {pokmeon.Base.Name}");

        evolutionUI.SetActive(false);

        //パーティ反映
        gc.PartyScreen.SetPartyData();
        AudioManager.i.PlayMusic(gc.CurrentScene.SceneMusic, fade: true);

        gc.StateMachine.Pop();
    }
}
