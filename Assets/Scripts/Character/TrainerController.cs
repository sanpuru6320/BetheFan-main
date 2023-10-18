using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable, ISaveable
{
    [SerializeField] string name;//トレーナー名
    [SerializeField] Sprite sprite;//トレーナースプライト
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] GameObject exclamation; 
    [SerializeField] GameObject fov;//認識範囲

    [SerializeField] AudioClip trainerAppearsClip;

    //State
    bool battleLost = false;

    Character character;
    private void Awake()
    {
        character = GetComponent<Character>(); 
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
    }
    private void Update()
    {
        character.HandleUpdate();
    }
    public IEnumerator Interact(Transform initiator)//会話後に戦闘
    {
        character.LookTowards(initiator.position);

        if (!battleLost)//戦闘前
        {
            AudioManager.i.PlayMusic(trainerAppearsClip);

            yield return DialogManager.Instance.ShowDialog(dialog);
            GameController.Instance.StartTrainerBattle(this);
        }
        else//戦闘後
        {
            yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
        }
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)//プレイヤー発見後、戦闘
    {
        GameController.Instance.StateMachine.Push(CutsecneState.i);
        
        AudioManager.i.PlayMusic(trainerAppearsClip);
        
        // Show Exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // Walk towards the player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        // Show dialog
        yield return DialogManager.Instance.ShowDialog(dialog);

        GameController.Instance.StateMachine.Pop();

        GameController.Instance.StartTrainerBattle(this);
    }

    public void Battlelost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }
    public void SetFovRotation(FacingDirection dir)//認識方向をセット
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;

        if (battleLost)
            fov.gameObject.SetActive(false);
    }

    public string Name { 
        get => name; 
    }
    
    public Sprite Sprite
    {
        get => sprite;
    }
}
