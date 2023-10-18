using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour, Interactable, ISaveable//マップ上で取得できるアイテム
{
    [SerializeField] ItemBase item;

    public bool Used { get; set; } = false;

    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            //インベントリに追加
            initiator.GetComponent<Inventory>().AddItem(item);

            Used = true;

            //アイテム非表示
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;

            //アイテム取得イベント
            string playerName = initiator.GetComponent<PlayerController>().Name;

            AudioManager.i.PlaySfx(AudioId.ItemObtained, pauseMusic: true);
            yield return DialogManager.Instance.ShowDialogText($"{playerName} found {item.Name}");
        }
    }

    public object CaptureState()
    {
        return Used;
    }

    public void RestoreState(object state)
    {
        Used = (bool)state;

        if (Used)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
