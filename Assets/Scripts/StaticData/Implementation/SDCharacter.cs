using ProjectW.Define;
using System;

namespace ProjectW.SD
{
    [Serializable]
    public class SDCharacter : StaticData
    {
        /// <summary>
        /// 캐릭터의 이름
        /// </summary>
        public string name;
        /// <summary>
        /// 캐릭터의 일반 공격 타입
        /// </summary>
        public Actor.AttackType atkType;
        /// <summary>
        /// 이동 속도
        /// </summary>
        public float moveSpeed;
        /// <summary>
        /// 점프시 지면에 가해지는 힘
        /// </summary>
        public float jumpForce;
        /// <summary>
        /// 일반 공격 범위
        /// </summary>
        public float atkRange;
        /// <summary>
        /// 일반 공격 간격 (쿨타임)
        /// </summary>
        public float atkInterval;
        /// <summary>
        /// 성장스텟 테이블 인덱스 참조
        /// </summary>
        public int growthStatRef;
        /// <summary>
        /// 리소스 경로
        /// </summary>
        public string resourcePath;
    }
}
