﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour //Scene本体ではなくSceneTriggerにAttachする
{
    //エンカウントするポケモン
    [SerializeField] List<PokemonEncounterRecord> wildPokemons;
    [SerializeField] List<PokemonEncounterRecord> wildPokemonsInWater;

    [HideInInspector]
    [SerializeField] int totalChance = 0;

    [HideInInspector]
    [SerializeField] int totalChanceWater = 0;
    private void OnValidate()
    {
        CalculateChancePercentage();
    }
    private void Start()
    {
        CalculateChancePercentage();
    }

    void CalculateChancePercentage()
    {
        totalChance = -1;
        totalChanceWater = -1;

        if (wildPokemons.Count > 0)
        {
            totalChance = 0;
            foreach (var record in wildPokemons)
            {
                record.chanceLower = totalChance;
                record.chanceUpper = totalChance + record.chancePercentage;

                totalChance = totalChance + record.chancePercentage;
            }
        }

        if (wildPokemonsInWater.Count > 0)
        {
            totalChanceWater = 0;
            foreach (var record in wildPokemonsInWater)
            {
                record.chanceLower = totalChanceWater;
                record.chanceUpper = totalChanceWater + record.chancePercentage;

                totalChanceWater = totalChanceWater + record.chancePercentage;
            }
        }
    }
    public Pokemon GetRandomwildPokemon(BattleTrigger trigger)//エンカウントリストの中からランダムに決定
    {
        var pokemonList = (trigger == BattleTrigger.LongGrass) ? wildPokemons : wildPokemonsInWater;
        int randVal = Random.Range(1, 101);
        var pokemonRecord = pokemonList.First(p => randVal >= p.chanceLower && randVal <= p.chanceUpper);

        //レベルの設定
        var levelRange = pokemonRecord.levelRange;
        int level = levelRange.y == 0 ? levelRange.x : Random.Range(levelRange.x, levelRange.y + 1);

        var wildPokemon = new Pokemon(pokemonRecord.pokemon, level);
        wildPokemon.Init();
        return wildPokemon;
    }
}

[System.Serializable]
public class PokemonEncounterRecord
{
    public PokemonBase pokemon;
    public Vector2Int levelRange;
    public int chancePercentage;

    public int chanceLower { get; set; }
    public int chanceUpper { get; set; }
}
