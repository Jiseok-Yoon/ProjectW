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
        public DtoQuestProgress[] progressQuests;

        /// <summary>
        /// 완료한 퀘스트에 대한 인덱스 목록
        /// </summary>
        public int[] completedQuests;
    }

    [Serializable]
    public class DtoQuestProgress
    {
        public int index; // 진행중인 퀘스트 인덱스
        public int[] details; // 진행중인 퀘스트에 대한 세부정보 (ex: 사냥이라면 현재 몇마리?)
    }
}
