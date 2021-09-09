using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectW.SD
{
    public class SDGrowthStat : StaticData
    {
        /// <summary>
        /// 레벨에 따른 최대 체력 계산 시 베이스값
        /// </summary>
        public float maxHp;
        /// <summary>
        /// 레벨에 따라 증가할 최대 체력 계수 (level * maxHp * maxHpFactor)
        /// </summary>
        public float maxHpFactor;
        public float maxMana;
        public float maxManaFactor;
        public float atk;
        public float atkFactor;
        public float def;
        public float defFactor;
        public float behaviour;
        public float behaviourFactor;
    }
}
