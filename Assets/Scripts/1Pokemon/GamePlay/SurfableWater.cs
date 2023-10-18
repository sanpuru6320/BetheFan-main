using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SurfableWater : MonoBehaviour, Interactable, IPlayerTriggerable
{
    bool isJumpingWater = false;

    public bool TriggerRepeatedly => true;

    public IEnumerator Interact(Transform initiator)
    {
        var animator = initiator.GetComponent<CharactorAnimator>();
        if (animator.IsSurfing || isJumpingWater)//�T�[�t�B�����ƃT�[�t�B���ڍs���͖���
            yield break;

        yield return DialogManager.Instance.ShowDialogText("The water is deep blue");

        var pokemonWithSurf = initiator.GetComponent<PokemonParty>().Pokemons.FirstOrDefault(p => p.Moves.Any(mbox => mbox.Base.Name == "Surf"));//�p�[�e�B�̃|�P������Surf�̋Z�����邩����

        if (pokemonWithSurf != null)
        {
            //�T�[�t�B�����s�����I��
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"Should {pokemonWithSurf.Base.Name} use surf?",
                choices: new List<string>() { "Yes", "No" },
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                //Yes
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWithSurf.Base.Name} used surf!");

                //�T�[�t�B���ֈڍs
                var dir = new Vector3(animator.MoveX, animator.MoveY);
                var targetPos = initiator.position + dir;

                isJumpingWater = true;
                yield return initiator.DOJump(targetPos, 0.3f, 1, 0.5f).WaitForCompletion();
                isJumpingWater = false;
                animator.IsSurfing = true;
            }
        }
    }

    public void OnPlayerTriggered(PlayerController player)//�T�[�t�B�����̃G���J�E���g
    {
        if (UnityEngine.Random.Range(1, 101) <= 5)
        {
            GameController.Instance.StartBattle(BattleTrigger.Water);

        }
    }
}
