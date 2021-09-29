using ProjectW.SD;
using System;

namespace ProjectW.DB
{
    [Serializable]
    public class BoItem
    {
        public int slotIndex; // 아이템이 위치하고 있는 아이템 슬롯의 인덱스
        public int amount; // 아이템의 개수
        public SDItem sdItem; // 아이템의 기획 데이터

        // 아이템 드롭 후 루팅 시 사용, 서버에서 받은 아이템 정보로 데이터셋 생성 시 사용
        public BoItem(SDItem sdItem)
        {
            slotIndex = -1;
            amount = 1;
            this.sdItem = sdItem;
        }

        public BoItem DeepCopy()
        {
            // MemberwiseClone을 통해 값 타입의 필드를 전부 복사함.
            BoItem clone = (BoItem)this.MemberwiseClone();
            // 그 후 참조 타입만 별도로 처리
            // SDItem은 참조타입이지만, SD Module 쪽에서 전부 참조하여 사용하므로
            // 따로 객체를 복사할 필요가 없음.. 한마디로 같은 공간을 가리켜도 상관없음

            return clone;
        }
    }

    [Serializable]
    public class BoEquipment : BoItem
    {
        public bool isEquip; // 장비를 착용중인지
        public int reinforceValue; // 강화 수치

        public BoEquipment(SDItem sdItem) : base(sdItem) 
        {
            isEquip = false;
            reinforceValue = 0;
        }
    }
}
