using ProjectW.Network;
using System;
using System.Collections.Generic;

namespace ProjectW.DB
{
    [Serializable]
    public class DtoItem : DtoBase
    {
        public List<DtoItemElement> items;

        public DtoItem() { }
        /// <summary>
        /// db 저장 시 사용 용도
        /// </summary>
        /// <param name="boItems"></param>
        public DtoItem(List<BoItem> boItems)
        {
            items = new List<DtoItemElement>();

            for (int i = 0; i < boItems.Count; ++i)
            {
                items.Add(new DtoItemElement(boItems[i]));
            }
        }
    }

    [Serializable]
    public class DtoItemElement
    {
        public int slotIndex;
        public int index; // 실제로는 아이템의 고유한 인덱스, 저희는 기획 데이터 상의 인덱스
        public int amount;
        public int reinforceValue;
        public bool isEquip;

        public DtoItemElement() { }
        /// <summary>
        /// db 저장 시 사용 용도
        /// </summary>
        /// <param name="boItem"></param>
        public DtoItemElement(BoItem boItem) 
        {
            slotIndex = boItem.slotIndex;
            index = boItem.sdItem.index;
            amount = boItem.amount;

            if (boItem is BoEquipment)
            {
                var boEquipment = boItem as BoEquipment;
                reinforceValue = boEquipment.reinforceValue;
                isEquip = boEquipment.isEquip;
            }
        }
    }
}
