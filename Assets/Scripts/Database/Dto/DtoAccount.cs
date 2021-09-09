using System;
using ProjectW.Network;

namespace ProjectW.DB
{
    [Serializable]
    public class DtoAccount : DtoBase
    {
        /// <summary>
        /// ���� �г���
        /// </summary>
        public string nickname;
        /// <summary>
        /// ������ ���
        /// </summary>
        public int gold;
    }
}
