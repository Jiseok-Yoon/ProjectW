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
        public List<SDQuest> progressQuests;
        public List<SDQuest> completedQuests;

        public BoQuest(DtoQuest dtoQuest)
        {
            progressQuests = new List<SDQuest>();
            completedQuests = new List<SDQuest>();

            for (int i = 0; i < dtoQuest.progressQuests.Count(); ++i)
            {
                progressQuests.Add(GameManager.SD.sdQuests.Where(_ => _.index == dtoQuest.progressQuests[i]).SingleOrDefault());
            }
            
            for (int i = 0; i < dtoQuest.completedQuests.Count(); ++i)
            {
                completedQuests.Add(GameManager.SD.sdQuests.Where(_ => _.index == dtoQuest.completedQuests[i]).SingleOrDefault());
            }

        }
    }
}
