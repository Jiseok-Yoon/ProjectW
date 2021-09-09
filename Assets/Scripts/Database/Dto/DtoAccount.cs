using System;
using ProjectW.Network;

namespace ProjectW.DB
{
    [Serializable]
    public class DtoAccount : DtoBase
    {
        /// <summary>
        /// 유저 닉네임
        /// </summary>
        public string nickname;
        /// <summary>
        /// 보유한 골드
        /// </summary>
        public int gold;
    }
}
