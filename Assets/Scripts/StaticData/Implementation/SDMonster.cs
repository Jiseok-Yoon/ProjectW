using System;

namespace ProjectW.SD
{
    [Serializable]
    public class SDMonster : StaticData
    {
        public string name;
        public Define.Actor.AttackType atkType;
        public float moveSpeed;
        public float detectionRange;
        public float atkRange;
        public float atkInterval;
        public float maxHp;
        public float maxMana;
        public float atk;
        public float def;
        /// <summary>
        /// 몬스터가 드랍하는 아이템들의 인덱스
        /// </summary>
        public int[] dropItemRef;
        /// <summary>
        /// 몬스터가 드랍하는 아이템들의 드랍률
        /// </summary>
        public float[] dropItemPer;
        public string resourcePath;
    }
}
