using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectW.Define.Quest;

namespace ProjectW.SD
{
    public class SDQuest : StaticData
    {
        public string name;
        public QuestType questType;
        public int[] antecedentQuest;
        public int[] target;
        public int[] targetDetail;
        public int[] rewardItems;
        public int[] rewardItemsCount;
        public string description;
    }
}
