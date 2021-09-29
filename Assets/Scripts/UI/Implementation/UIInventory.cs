using ProjectW.DB;
using ProjectW.Dummy;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        /// 아이템을 드래그 시, 드래그를 끝낸 위치가 아이템 슬롯이 아니라면 아이템이 제자리로 돌아갈 수 있도록
        /// 원래 위치하고 있던 곳의 좌표를 받아둔다.
        /// </summary>
        private Vector3 dragSlotOriginPos;
        private ItemSlot dragSlot;

        public Button sortButton;
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

            InitAllSlot();

            InventoryUpdate();

            // 인벤토리 정렬 버튼 이벤트 바인딩
            sortButton.onClick.AddListener(() => {

                // 슬롯으로 접근할 필요없이 유저 데이터에 접근해서 아이템 데이터를 받아옴
                var boItems = GameManager.User.boItems;

                // OrderBy 함수 쿼리를 이용하여 아이템 데이터를 이름 순으로 정렬
                var sortedItems = boItems.OrderBy(_ => _.sdItem?.name).ToList();

                // 전체 슬롯 초기화를 하면 슬롯에 바인딩 되있던 아이템 정보 참조가 전부 날아감
                InitAllSlot();

                // 정렬시켜던 아이템 정보를 순서대로 슬롯에 적용
                // 위와 같은 방식으로 작업할 시 빈슬롯에 대해 고려하지않아도 됌
                for (int i = 0; i < sortedItems.Count; ++i)
                {
                    itemSlots[i].SetSlot(sortedItems[i]);
                    itemSlots[i].BoItem.slotIndex = i;
                }

                // 아이템 정보가 변경되었으므로 db에 업데이트
                DummyServer.Instance.userData.dtoItem = new DtoItem(boItems);
                DummyServer.Instance.Save();
            });
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

        // 드래그 시작 시 한 번 호출 (드래그 시작을 위한 클릭 시)
        public void OnBeginDrag(PointerEventData eventData)
        {
            // 그래픽 레이캐스팅을 통해 현재 마우스로 클릭한 객체의 정보를 받아옴
            var result = new List<RaycastResult>();
            gr.Raycast(eventData, result);

            // 드래그하려고 하는 객체가 아이템슬롯의 아이템인지?
            for (int i = 0; i < result.Count; ++i)
            {
                if (result[i].gameObject.name.Contains("ItemSlot"))
                {
                    dragSlot = result[i].gameObject.GetComponent<ItemSlot>();
                    dragSlotOriginPos = dragSlot.ItemImage.transform.position;
                    break;
                }
            }
        }

        // 드래그 도중에 계속 들어옴
        public void OnDrag(PointerEventData eventData)
        {
            // dragSlot이 null이 아니라면 드래그하는 아이템이 존재한다는 것
            if (dragSlot == null)
                return;

            // dragSlot의 ItemImage 객체의 포지션을 마우스 포지션으로 설정
            dragSlot.ItemImage.transform.position = Input.mousePosition;
        }

        // 드래그를 끝내기 위해 마우스에서 버튼을 뗐을 때
        public void OnEndDrag(PointerEventData eventData)
        {
            var result = new List<RaycastResult>();
            gr.Raycast(eventData, result);

            // 드래그하던 아이템 이미지를 원래 위치로 되돌려놓는다.
            // 아이템의 위치가 바뀔건데 이미지를 원래대로 되돌려놓는 이유?
            // 아이템의 데이터를 변경된 슬롯 위치로 세팅하고
            // 데이터가 변경되었으므로, 아이템 슬롯의 SetSlot을 통해 변경된 데이터에 알맞는
            // 이미지, 수량 텍스트 등을 알아서 업데이트 시킬 것이므로
            dragSlot.ItemImage.transform.position = dragSlotOriginPos;

            // -> 눈에 보이는 뷰에 신경을 쓰는게 아니라, 데이터에 초점을 맞춰서 작업을 하겠다는 것

            // 드래그를 끝낸 시점, 마우스를 뗐을 때 마우스가 위치하고 있는 아이템 슬롯을 찾는다.
            // 만약 마우스 위치에 아이템 슬롯이 존재하지 않는다면 일단 메서드를 종료..
            ItemSlot destSlot = null;

            for (int i = 0; i < result.Count; ++i)
            {
                if (result[i].gameObject == dragSlot.gameObject)
                    continue;

                if (result[i].gameObject.name.Contains("ItemSlot"))
                {
                    destSlot = result[i].gameObject.GetComponent<ItemSlot>();
                    break;
                }    
            }

            if (destSlot == null)
                return;

            // 아이템 슬롯이 존재한다면 데이터 스왑
            var boItems = GameManager.User.boItems;

            // 드래그 하고 있던 아이템의 정보를 깊은 복사함
            var tempBoItem = dragSlot.BoItem.DeepCopy();

            // 기존 드래그 하던 아이템 정보를 유저의 아이템 정보에서 지우고, 
            // 복사한 아이템 정보를 추가함
            boItems.Remove(dragSlot.BoItem);
            boItems.Add(tempBoItem);

            // 드래그 하고 있던 슬롯에는 방금 마우스를 뗀 위치에 존재하는 슬롯의 아이템 정보를 세팅
            dragSlot.SetSlot(destSlot.BoItem);
            SetIndex(dragSlot);

            // 마우스를 뗀 위치에 존재하는 슬롯에는 기존 드래그하던 슬롯의 정보를 복사한 아이템을 세팅 
            destSlot.SetSlot(tempBoItem);
            SetIndex(destSlot);

            // db에도 데이터를 업데이트해야함
            DummyServer.Instance.userData.dtoItem = new DtoItem(boItems);
            DummyServer.Instance.Save();

            void SetIndex(ItemSlot slot)
            {
                if (slot.BoItem == null)
                    return;

                // 정수가 아니라면 빈 문자열로 만듬 (한 마디로 문자열에서 정수만 추출)
                //  -> 현재 아이템 슬롯의 이름에는 0~ 15까지 넘버링이 되어있음
                string index = Regex.Replace(slot.gameObject.name, @"[^\d]", "");
                slot.BoItem.slotIndex = int.Parse(index);
            }
        }

        private void InitAllSlot()
        {
            // 모든 아이템 슬롯 초기화
            for (int i = 0; i < itemSlots.Count; ++i)
            {
                itemSlots[i].Initialize();
            }
        }
    }
}
