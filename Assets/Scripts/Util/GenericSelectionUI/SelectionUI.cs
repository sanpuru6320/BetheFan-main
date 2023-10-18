using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDE.GenericSelectionUI
{
    public enum SelectionType { List, Grid}//UIタイプ

    public class SelectionUI<T> : MonoBehaviour where T :ISelectableItem
    {
        List<T> items;//UIのリスト
        protected int selectedItem = 0;

        SelectionType selectionType;
        int gridWidth = 2;


        float seletionTimer = 0;

        const float selectionSpeed = 1;

        public event Action<int> OnSelected;
        public event Action OnBack;

        //使用するUIタイプの設定
        public void SetSelectionSettings(SelectionType selectionType, int gridWidth)
        {
            this.selectionType = selectionType;
            this.gridWidth = gridWidth;
        }

        public void SetItems(List<T> items)//UIのセット
        {
            this.items = items;

            items.ForEach(i => i.Init());
            UpdateSelectionUI();
        }

        public void ClearItems()
        {
            if (items != null)
            {
                items.ForEach(i => i.Clear());

                this.items = null;
            }

        }

        public virtual void HandleUpdete()
        {
            UpdateSelectionTimer();
            float prevSelection = selectedItem;

            //UIタイプによって入力を切り替え
            if (selectionType == SelectionType.List)
                HandleListSelection();
            else if (selectionType == SelectionType.Grid)
                HandleGridSelection();

            if(items != null)
            {
                selectedItem = Mathf.Clamp(selectedItem, 0, items.Count - 1);
            }
            

            if (selectedItem != prevSelection)
                UpdateSelectionUI();

            if (Input.GetButtonDown("Action"))
                OnSelected?.Invoke(selectedItem);
            else if (Input.GetButtonDown("Back"))
                OnBack?.Invoke();
        }

        void HandleListSelection()
        {
            float v = Input.GetAxis("Vertical");

            if(seletionTimer == 0 && Mathf.Abs(v) > 0.2f)
            {
                selectedItem += -(int)Mathf.Sign(v);

                seletionTimer = 1 / selectionSpeed;
            }
            
        }

        void HandleGridSelection()
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            if (seletionTimer == 0 && (Mathf.Abs(v) > 0.2f || Mathf.Abs(h) > 0.2f))
            {
                if(Mathf.Abs(h) > Mathf.Abs(v))
                    selectedItem += (int)Mathf.Sign(h);
                else
                    selectedItem += -(int)Mathf.Sign(v) * gridWidth;
                seletionTimer = 1 / selectionSpeed;
            }
        }

        public virtual void UpdateSelectionUI()
        {
            if(items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].OnSelectionChanged(i == selectedItem);
                }
            }

        }

        void UpdateSelectionTimer()//UIの入力速度調整
        {
            if (seletionTimer > 0)
                seletionTimer = Mathf.Clamp(seletionTimer - Time.deltaTime, 0, seletionTimer);
        }
    }
}

