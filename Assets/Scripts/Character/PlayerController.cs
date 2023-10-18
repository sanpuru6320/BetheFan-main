using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISaveable
{

    [SerializeField] string name;//プレイヤー名
    [SerializeField] Sprite sprite;//プレイヤースプライト

 
    
    private Vector2 input;

    public static PlayerController i { get; private set; }
    private Character charactor;

    private void Awake()
    {
        i = this;
        charactor = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        if (!charactor.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;
            //remove diagonal movement

            if(input != Vector2.zero)
            {
                DelaySample();
                //GameController.Instance.Buddy.Follow(GameController.Instance.PlayerController.transform.position);
                StartCoroutine(charactor.Move(input, OnMoveOver));
            }
        }

        charactor.HandleUpdate();
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(InteractStart());
        }

    }

    IEnumerator InteractStart()//インタラクト
    {
        //インタラクトしたオブジェクトを取得
        var facingDir = new Vector3(charactor.Animator.MoveX, charactor.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer | GameLayers.i.WaterLayer);
        if (collider != null)
        {
            yield return collider.GetComponent<Interactable>()?.Interact(transform);//インタラクト可能ならリターン
        }
    }

    IPlayerTriggerable currentlyInTrigger;

    private void OnMoveOver() //colliderの範囲内に入った時
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, charactor.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);

        IPlayerTriggerable triggerable = null;
        
        foreach(var collider in colliders)//トリガーチェック
        {
            triggerable = collider.GetComponent<IPlayerTriggerable>(); 
            if (triggerable != null) 
            {
                if (triggerable == currentlyInTrigger && !triggerable.TriggerRepeatedly)
                    break;
                
                triggerable.OnPlayerTriggered(this);
                currentlyInTrigger = triggerable;
                break; 
            }
        }

        if (colliders.Count() == 0 || triggerable != currentlyInTrigger)
            currentlyInTrigger = null;
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()    //シリアライズ化可能なもの(System.Serializableなど)はそのまま返せるが、Unityなどの関数は返せないため変換する
        {
            position = new float[] { transform.position.x, transform.position.y },
            //pokemons = GetComponent<PokemonParty>().Pokemons.Select(p => p.GetSaveData()).ToList()
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;
       
        //Restor Position
        var pos = saveData.position; 
        transform.position = new Vector3(pos[0], pos[1]);

        // Restore Party
        //GetComponent<PokemonParty>().Pokemons = saveData.pokemons.Select(s => new Pokemon(s)).ToList();
    }

    public string Name
    {
        get => name;
    }

    public Sprite Sprite
    {
        get => sprite;
    }

    public Character Character => charactor;

    [Serializable]
    public class PlayerSaveData 
    { 
        public float[] position;
        //public List<PokemonSaveData> pokemons;
    }

    static async void DelaySample()
    {
        await Task.Delay(1000);
    }
}
