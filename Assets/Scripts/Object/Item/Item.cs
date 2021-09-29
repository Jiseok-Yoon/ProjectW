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
        /// ���͸� ���� �� �������� ����Ǹ� �ش� �������� �ε�����
        /// ������ ��ü ���� �� �Ѱ���
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
            // �����۰� ��ģ �ݶ��̴��� ���� ��ü�� �÷��̾ �ƴ϶�� ����
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;

            // ���� ������ ��� �Դ���, ���� �ٸ� ������ ������ ����� ����
            var isEquip = sdItem.itemType == Define.Item.ItemType.Equipment;

            var userItems = GameManager.User.boItems;
            var uiInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

            // ��� �ƴ� �������̶�� ������ �ش� �������� �̹� ������ �ִ��� Ȯ���ؼ�
            if (!isEquip)
            {
                var sameItem = userItems.Where(_ => _.sdItem.index == sdItem.index).SingleOrDefault();

                // �̹� ������ �ִ� �������̶�� ������ �÷��ְ�,
                if (sameItem != null)
                {
                    ++sameItem.amount;
                    uiInventory.IncreaseItem(sameItem);
                }
                // ������ ���� �ʴٸ� ���� ������ ������ �߰�
                else
                {
                    AddItem(new BoItem(sdItem));
                }
            }
            // �����
            else
            {
                // ���� ������ �κ��丮 �� ĭ�� �����ϹǷ�, �ٷ� ������ ������ ������ �߰����ش�.
                // (�κ��丮 �� ĭ ���� -> ����� amount�� ������ 1)
                AddItem(new BoEquipment(sdItem));
            }

            // ���� �������� ������ �κ��丮�� ���� �Ѱ����Ƿ�, ������ ��ü�� Ǯ�� �ٽ� ��ȯ�Ѵ�.
            ObjectPoolManager.Instance.GetPool<Item>(Define.PoolType.Item).ReturnPoolableObject(this);

            // ������ ��ü ������ ������ ������ �������Ƿ�, ��ü ������ ������ �ٽ� ������ �����ش�.
            // (����Ʈ�� ������ ������ �������� ������ ��.. ��� ó���Ϸ��� �� ���������Ƿ�..  
            //  ��ü�� ������ �������)
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