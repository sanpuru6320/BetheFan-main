﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    //ショップの商品アイテムリスト、アイコンUI
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] Text ItemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    int selectedItem;

    List<ItemBase> availableItems;//商品リスト
    Action<ItemBase> onItemSelected;
    Action onBack;

    List<ItemSlotUI> slotUIList;

    const int itemViewport = 8;

    RectTransform itemListRect;

    private void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    //商品をショップUIに表示
    public void Show(List<ItemBase> availableItems, Action<ItemBase> onItemSelected, 
        Action onBack)
    {
        this.availableItems = availableItems;
        this.onItemSelected = onItemSelected;
        this.onBack = onBack;
        
        gameObject.SetActive(true);
        UpdateItemList();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    //ショップUI操作
    public void HandleUpdete()
    {
        var prevSelection = selectedItem;
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);

        if (selectedItem != prevSelection)
            UpdateItemSlection();

        if (Input.GetKeyDown(KeyCode.Z))
            onItemSelected?.Invoke(availableItems[selectedItem]);
        else if (Input.GetKeyDown(KeyCode.X))
            onBack?.Invoke();
    }

    //ショップリスト更新
    void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (var item in availableItems)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetNameAndPrice(item);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSlection();
    }

    //選択中の画面更新
    void UpdateItemSlection()
    {

        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }

        if (availableItems.Count > 0)
        {
            var item = availableItems[selectedItem];
            itemIcon.sprite = item.Icon;
            ItemDescription.text = item.Description;
        }

        HandleScrolling();
    }

    //ショップ画面更新
    void HandleScrolling()
    {
        if (slotUIList.Count <= itemViewport) return;

        float scrollPos = Mathf.Clamp(selectedItem - itemViewport / 2, 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > itemViewport / 2;
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItem + itemViewport / 2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }
}
