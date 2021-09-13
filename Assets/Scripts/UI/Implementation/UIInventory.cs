using ProjectW.DB;
using ProjectW.Dummy;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectW.UI
{
    public class UIInventory : UIWindow, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Transform itemSlotHolder; 
        private List<ItemSlot> itemSlots = new List<ItemSlot>();
        /// <summary>
        /// 아이템 드래그 시, 드래그를 끝낸 위치가 아이템 슬롯이 아니라면 아이템이 제자리로 돌아갈 수 있도록 
        /// 원래 위치하고 있던 곳의 좌표를 받아둔다.
        /// </summary>
        private Vector3 dragSlotOriginPos;
        private ItemSlot dragSlot;
        public GraphicRaycaster gr;

        public override void Start()
        {
            base.Start();

            // 아이템 슬롯 홀더 참조 바인딩
            itemSlotHolder = transform.Find("Frame/ItemSlotHolder");
            
            // 모든 아이템 슬롯의 참조를 찾아서 리스트에 넣음
            for (int i = 0; i < itemSlotHolder.childCount; ++i)
            {
                itemSlots.Add(itemSlotHolder.GetChild(i).GetComponent<ItemSlot>());
            }

            // 모든 아이템 슬롯 초기화
            for (int i = 0; i < itemSlots.Count; ++i)
            {
                itemSlots[i].Initialize();
            }

            InventoryUpdate();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (isOpen)
                    Close();
                else
                    Open();
            }
        }

        public void UpdateSlotIndex()
        {
            for (int i = 0; i < itemSlots.Count; ++i)
            {
                if (itemSlots[i].BoItem != null)
                    itemSlots[i].BoItem.slotIndex = i;
            }
        }

        /// <summary>
        /// 인벤토리에 아이템을 추가하는 기능
        /// 1. 서버에서 아이템정보를 받아서 추가할 때
        /// 2. 몬스터를 잡아서 드랍된 아이템을 루팅해서 추가할 때
        /// </summary>
        /// <param name="boItem"></param>
        public void AddItem(BoItem boItem)
        {
            // 추가된 아이템에 슬롯 인덱스가 정상적으로 존재한다면
            // 슬롯인덱스 맞춰 배치시키고, 아니라면 상단 좌측을 기준으로 비어있는 슬롯에 넣어준다.
            if (boItem.slotIndex >= 0) // 서버에서 준 거
            {
                itemSlots[boItem.slotIndex].SetSlot(boItem);
                return;
            }

            // 새로 생성된 아이템을 먹은 경우 (클라에서 만든 아이템)
            for (int i = 0; i < itemSlots.Count; ++i)
            {
                if (itemSlots[i].BoItem == null)
                {
                    boItem.slotIndex = i;
                    itemSlots[i].SetSlot(boItem);
                    break;
                }
            }
        }

        /// <summary>
        /// 가지고 있는 아이템이라면 수량 증가
        /// </summary>
        /// <param name="boItem"></param>
        public void IncreaseItem(BoItem boItem)
        {
            itemSlots[boItem.slotIndex].AmountUpdate();
        }

        public void UseItem(BoItem boItem)
        {
            if (boItem.slotIndex < 0 || boItem.slotIndex >= itemSlots.Count)
                return;

            if (boItem.amount < 1)
            {
                itemSlots[boItem.slotIndex].SetSlot();
                return;
            }



            --boItem.amount;
            itemSlots[boItem.slotIndex].AmountUpdate();



            DummyServer.Instance.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            DummyServer.Instance.Save();
        }

        public void EquipItem(BoItem boItem)
        {
            if (boItem.slotIndex < 0 || boItem.slotIndex >= itemSlots.Count)
                return;
            var equip = boItem as BoEquipment;
            equip.isEquip = true;
            return;

            DummyServer.Instance.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            DummyServer.Instance.Save();
        }

        public void RemoveItem(BoItem boItem)
        {
            if (boItem.slotIndex < 0 || boItem.slotIndex >= itemSlots.Count)
                return;
            itemSlots[boItem.slotIndex].SetSlot();
        }

        /// <summary>
        /// 유저의 아이템 정보를 받아 아이템 슬롯에 연결해주는 기능
        /// </summary>
        private void InventoryUpdate()
        {
            var userItems = GameManager.User.boItems;
            for (int i = 0; i < userItems.Count; ++i)
            {
                AddItem(userItems[i]);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var result = new List<RaycastResult>();
            gr.Raycast(eventData, result);
            
            // 드래그하려고 하는 객체가 아이템 슬롯의 아이템인지?
            for (int i = 0; i < result.Count; ++i)
            {
                if (result[i].gameObject.name.Contains("ItemSlot"))
                {
                    dragSlot = result[i].gameObject.GetComponent<ItemSlot>();
                    dragSlotOriginPos = dragSlot.ItemImage.rectTransform.position;
                    break;
                }
            }

        }

        public void OnDrag(PointerEventData eventData)
        {
            // dragSlot이 null이 아니라면 드래그하는 아이템이 존재한다는 것
            if (dragSlot == null)
                return;

            // dragSlot의 ItemImage 객체의 포지션을 마우스 포지션으로 설정
            dragSlot.ItemImage.rectTransform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // 드래그하던 아이템 이미지를 원래 위치로 되돌려 놓는다.
            // 아이템의 위치가 바뀔건데 이미지를 원래대로 되돌려놓는 이유?
            // 아이템의 데이터를 변경된 슬롯 위치로 세팅하고
            // 데이터가 변경되었으므로, 아이템 슬롯의 SetSlot을 통해 변경된 데이터에 알맞는
            // 이미지, 수량 텍스트 등을 알아서 업데이트 시킬 것이므로

            // -> 눈에 보이는 뷰에 신경을 쓰는게 아니라 데이터에 초점을 맞춰서 작업을 하겠다는 것

            // 드래그를 끝낸 시점, 마우스를 뗏을 때 마우스가 위치하고 있는 아이템 슬롯을 찾는다.
            var result = new List<RaycastResult>();
            gr.Raycast(eventData, result);

            ItemSlot targetSlot = null;

            for (int i = 0; i < result.Count; ++i)
            {
                if (result[i].gameObject.name.Contains("ItemSlot"))
                {
                    targetSlot = result[i].gameObject.GetComponent<ItemSlot>();
                    break;
                }
            }

            dragSlot.ItemImage.rectTransform.position = dragSlotOriginPos;

            // 만약 마우스 위치에 아이템 슬롯이 존재하지 않는다면 일단 메서드를 종료.

            if (targetSlot == null)
                return;

            // 아이템 슬롯이 존재한다면 데이터 스왑
            targetSlot.Swap(dragSlot);

            UpdateSlotIndex();
            // db에도 데이터를 업데이트해야함.
            DummyServer.Instance.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            DummyServer.Instance.Save();

        }
    }
}
