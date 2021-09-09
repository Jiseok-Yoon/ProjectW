using ProjectW.DB;
using ProjectW.Dummy;
using ProjectW.Resource;
using ProjectW.SD;
using ProjectW.UI;
using ProjectW.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectW.Object
{
    public class Item : MonoBehaviour, IPoolableObject
    {
        private SDItem sdItem;
        private Image image; 

        public bool CanRecycle { get; set; } = true;

        /// <summary>
        /// 몬스터를 죽인 후 아이템이 드랍되면 해당 아이템의 인덱스를
        /// 아이템 객체 생성 시 넘겨줌
        /// </summary>
        /// <param name="itemIndex"></param>
        public void Initialize(int itemIndex)
        {
            sdItem = GameManager.SD.sdItems.Where(_ => _.index == itemIndex).SingleOrDefault();
            var itemSprite = SpriteLoader.GetSprite(Define.Resource.AtlasType.ItemAtlas, sdItem.resourcePath);

            image ??= GetComponent<Image>();
            image.sprite = itemSprite;
            image.SetNativeSize();
        }

        private void OnTriggerEnter(Collider other)
        {
            // 아이템과 겹친 콜라이더를 갖는 객체가 플레이어가 아니라면 리턴
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;

            // 장비는 동일한 장비를 먹더라도, 서로 다른 아이템 슬롯을 사용할 거임
            var isEquip = sdItem.itemType == Define.Item.ItemType.Equipment;

            var userItems = GameManager.User.boItems;
            var uiInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

            // 장비가 아닌 아이템이라면 유저가 해당 아이템을 이미 가지고 있는지 확인해서
            if (!isEquip)
            {
                var sameItem = userItems.Where(_ => _.sdItem.index == sdItem.index).SingleOrDefault();

                // 이미 가지고 있는 아이템이라면 개수를 올려주고,
                if (sameItem != null)
                {
                    ++sameItem.amount;
                    uiInventory.IncreaseItem(sameItem);
                }
                // 가지고 있지 않다면 유저 아이템 정보에 추가
                else
                {
                    AddItem(new BoItem(sdItem));
                }
            }
            // 장비라면
            else
            {
                // 장비는 무조건 인벤토리 한 칸을 차지하므로, 바로 유저의 아이템 정보에 추가해준다.
                // (인벤토리 한 칸 차지 -> 장비의 amount는 무조건 1)
                AddItem(new BoEquipment(sdItem));
            }

            // 먹은 아이템의 정보를 인벤토리로 전부 넘겼으므로, 아이템 객체를 풀에 다시 반환한다.
            ObjectPoolManager.Instance.GetPool<Item>(Define.PoolType.Item).ReturnPoolableObject(this);

            // 유저의 전체 아이템 정보에 변동이 생겼으므로, 전체 아이템 정보를 다시 서버에 보내준다.
            // (베스트는 변동된 아이템 정보만을 보내는 것.. 요걸 처리하려면 좀 귀찮아지므로..  
            // 전체를 보내서 덮어쓰게함)
            DummyServer.Instance.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            DummyServer.Instance.Save();

            void AddItem(BoItem boItem)
            {
                uiInventory.AddItem(boItem);
                userItems.Add(boItem);
            }
        }
    }
}