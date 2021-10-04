using ProjectW.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectW.DB
{
    [Serializable]
    public class BoQuest
    {
        public List<BoQuestProgress> progressQuests;
        public List<SDQuest> completedQuests;

        public BoQuest(DtoQuest dtoQuest)
        {
            progressQuests = new List<BoQuestProgress>();
            completedQuests = new List<SDQuest>();

            for (int i = 0; i < dtoQuest.progressQuests.Length; ++i)
            {
                progressQuests.Add(new BoQuestProgress(dtoQuest.progressQuests[i]));
            }
            
            for (int i = 0; i < dtoQuest.completedQuests.Count(); ++i)
            {
                completedQuests.Add(GameManager.SD.sdQuests.Where(_ => _.index == dtoQuest.completedQuests[i]).SingleOrDefault());
            }

        }
    }

    [Serializable]
    public class BoQuestProgress
    {
        public int[] details;
        public SDQuest sdQuest;
        public BoQuestProgress(DtoQuestProgress dtoQuestProgress)
        {
            details = (int[])dtoQuestProgress.details.Clone();
            sdQuest = GameManager.SD.sdQuests.Where(_ => _.index == dtoQuestProgress.index).SingleOrDefault();
        }
    }
}
