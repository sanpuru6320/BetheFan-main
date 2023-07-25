using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text countText;
    [SerializeField] TextMeshProUGUI nameTexts;
    [SerializeField] TextMeshProUGUI countTexts;

    RectTransform rectTransform;

    public void Awake()
    {
        
    }

    public Text NameText => nameText;
    public Text CountText => countText;

    public TextMeshProUGUI NameTexts => nameTexts;
    public TextMeshProUGUI CountTexts => countTexts;

    public float Height => rectTransform.rect.height;

    public void SetData(ItemSlot itemSlot)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = itemSlot.Item.Name;
        countText.text = $"X {itemSlot.Count}";
    }

    //public void SetItemData(ItemsSlot itemsSlot)
    //{
    //    rectTransform = GetComponent<RectTransform>();
    //    Debug.Log(itemsSlot.Item.Name);
    //    nameTexts.text = itemsSlot.Item.Name;
    //    countTexts.text = $"X {itemsSlot.Count}";
    //}

    public void SetNameAndPrice(ItemBase item)
    {
        rectTransform = GetComponent<RectTransform>(); 
        nameText.text = item.Name;
        countText.text = $"X {item.Price}";
    }
}
