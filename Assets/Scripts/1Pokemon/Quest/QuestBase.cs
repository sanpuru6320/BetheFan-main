using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Quests/Create a new quest")]

public class QuestBase : ScriptableObject//クエスト基本設定
{
    [SerializeField] string name;
    [SerializeField] string description;

    //クエストNPCの会話
    [SerializeField] Dialog startDialogue;
    [SerializeField] Dialog inProgressDialogue;
    [SerializeField] Dialog completedDialogue;

    [SerializeField] ItemBase requiredItem; //クリアに必要なアイテム
    [SerializeField] ItemBase rewardItem;

    public string Name => name;
    public string Description => description;

    public Dialog StartDialogue => startDialogue;
    public Dialog InPrigressDialogue => inProgressDialogue?.Lines?.Count > 0 ? inProgressDialogue : startDialogue;
    public Dialog CompletedDialogue => completedDialogue;

    public ItemBase RequiredItem => requiredItem;
    public ItemBase RewardItem => rewardItem;

}
