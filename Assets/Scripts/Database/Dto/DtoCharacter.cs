using ProjectW.Network;
using System;

namespace ProjectW.DB
{
    [Serializable]
    public class DtoCharacter : DtoBase
    {
        /// <summary>
        /// 기획 테이블 상의 캐릭터 인덱스
        /// </summary>
        public int index;
        /// <summary>
        /// 유저 캐릭터의 레벨
        /// </summary>
        public int level;
    }
}
