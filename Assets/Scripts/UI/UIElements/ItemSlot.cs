using ProjectW.DB;
using ProjectW.Dummy;
using ProjectW.Resource;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectW.UI
{
    public class ItemSlot : MonoBehaviour /*, IPointerClickHandler*/
    {
        private TMP_Text itemAmount;
        public Image ItemImage { get; private set; }

        public BoItem BoItem { get; private set; }

        public void Initialize()
        {
            itemAmount ??= GetComponentInChildren<TMP_Text>();
            ItemImage ??= transform.GetChild(0).GetComponent<Image>();

            // 빈 슬롯들의 색상을 설정해주기 위해서..
            this.SetSlot();
        }

        /// <summary>
        /// 슬롯을 세팅하는 기능 
        /// </summary>
        /// <param name="boItem"></param>
        public void SetSlot(BoItem boItem = null)
        {
            // 해당 슬롯에 아이템 존재 여부에 따라 알맞는 설정을 함
            BoItem = boItem;

            if (boItem == null)
            {
                itemAmount.text = "";
                ItemImage.sprite = null;
                ItemImage.color = new Color(1, 1, 1, 0);
            }
            else
            {
                itemAmount.text = boItem.amount.ToString();
                ItemImage.sprite = SpriteLoader.GetSprite(Define.Resource.AtlasType.ItemAtlas, boItem.sdItem.resourcePath);
                ItemImage.color = Color.white;
            }
        }

        /// <summary>
        /// 동일한 아이템을 먹었을 시, 변경된 수량을 UI 상에 업데이트
        /// </summary>
        public void AmountUpdate()
        {
            itemAmount.text = BoItem.amount.ToString();
        }

        //public void OnPointerClick(PointerEventData eventData)
        //{
        //    Debug.Log("슬롯 눌럿음 ㅋ");

        //    // 예외처리 
        //    // 슬롯에 아이템이 존재하는지 확인
        //    if (BoItem == null)
        //        return;

        //    // 유저 아이템 정보에서 수량이 0 이하라면 정보를 지움
        //    var boItems = GameManager.User.boItems;

        //    // 아이템의 종류에 따라 사용처리
        //    switch (BoItem.sdItem.itemType)
        //    {
        //        case Define.Item.ItemType.Equipment:
        //            // 장비라면 아이템을 장착할 수 있는 아이템인지 확인해서
        //            // 장착할 수 있다면 장착 처리
        //            break;
        //        case Define.Item.ItemType.Expendables:
        //            // 소모품이라면 아이템을 사용 처리 후, 아이템의 개수를 - 시키고
        //            // 아이템의 개수가 0 이라면 인벤토리에서 제거
        //            --BoItem.amount;
        //            if (BoItem.amount <= 0)
        //            { 
        //                boItems.Remove(BoItem);
        //                SetSlot();
        //            }
        //            else
        //                AmountUpdate();

        //                break;
        //        case Define.Item.ItemType.Quest:
        //            break;
        //        case Define.Item.ItemType.Etc:
        //            break;
        //    }

            

        //    // 변경된 정보를 db 에 새로 씀
        //    DummyServer.Instance.userData.dtoItem = new DtoItem(boItems);
        //    DummyServer.Instance.Save();
        //}
    }
}
