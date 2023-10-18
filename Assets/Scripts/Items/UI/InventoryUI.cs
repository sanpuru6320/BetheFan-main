using GDE.GenericSelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : SelectionUI<TextSlot>
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Text categoryText;
    [SerializeField] Image itemIcon;
    [SerializeField] Text ItemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    int selectedCategory = 0;

    const int itemViewport = 8;

    List<ItemSlotUI> slotUIList;
    Inventory inventory;
    RectTransform itemListRect;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();

        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()//インベントリのアイテムリスト更新
    {
        //全てのアイテムクリア
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        //アイテムリスト生成、アイテムのセット
        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        SetItems(slotUIList.Select(s => s.GetComponent<TextSlot>()).ToList());

        UpdateSelectionUI();
    }

    public override void HandleUpdete()
    {

        int prevCategory = selectedCategory;

        //カテゴリの移動
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selectedCategory;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selectedCategory;
        
        if (selectedCategory > Inventory.ItemCategories.Count - 1)
            selectedCategory = 0;
        else if (selectedCategory < 0)
            selectedCategory = Inventory.ItemCategories.Count - 1;

        //表示カテゴリの更新
        if (prevCategory != selectedCategory)
        {
            ResetSelection();
            categoryText.text = Inventory.ItemCategories[selectedCategory];
            UpdateItemList();
        }

        base.HandleUpdete();
    }

    public override void UpdateSelectionUI()//リストの更新(スクロール移動)
    {
        base.UpdateSelectionUI();

        var slots = inventory.GetSlotsByCategory(selectedCategory);

        if (slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            ItemDescription.text = item.Description;
        }

        HandleScrolling();

    }

    void HandleScrolling()//リストのスクロール
    {
        if (slotUIList.Count <= itemViewport) return;
        
        float scrollPos = Mathf.Clamp(selectedItem - itemViewport/2, 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > itemViewport / 2 ;
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItem + itemViewport / 2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }

    void ResetSelection()
    {
        selectedItem = 0;

        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        ItemDescription.text = "";
    }

    public ItemBase SelectedItem => inventory.GetItem(selectedItem, selectedCategory);

    public int SelectedCategory => selectedCategory;
}
