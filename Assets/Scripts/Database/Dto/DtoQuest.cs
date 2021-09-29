using ProjectW.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectW.DB
{
    [Serializable]
    public class DtoQuest : DtoBase
    {
        /// <summary>
        /// 진행중인 퀘스트에 대한 인덱스 목록
        /// </summary>
        public int[] progressQuests;

        /// <summary>
        /// 완료한 퀘스트에 대한 인덱스 목록
        /// </summary>
        public int[] completedQuests;
    }
}
