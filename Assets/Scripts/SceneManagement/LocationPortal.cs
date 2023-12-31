﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Teleports the player to a different position without swithcing scenes
public class LocationPortal : MonoBehaviour,IPlayerTriggerable
{
    [SerializeField] Transform spawnPoint;//テレポート後のポイント
    [SerializeField] DestinationIdentifier destinationPortal;//ポイントの番地

    PlayerController player;

    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        StartCoroutine(Teleport());
    }

    public bool TriggerRepeatedly => false;

    Fader fader;

    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator Teleport()
    {
        GameController.Instance.PauseGame(true);
        yield return fader.FadeIn(0.5f);


        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);//対応するポイントを持つ別のオブジェクトにテレポート
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);

    }



    public Transform SpawnPoint => spawnPoint;

    public enum DestinationIdentifier { A, B, C, D, E }//ポイントの番地
}
