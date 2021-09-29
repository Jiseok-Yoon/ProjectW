using System;

namespace ProjectW.SD
{
    [Serializable]
    public class SDQuest : StaticData
    {
        public string name;
        public Define.Quest.QuestType type;
        public int[] antecedentQuest;
        public int[] target;
        public int[] targetDetail;
        public int[] rewardItems;
        public int[] rewardItemsCount;
        public int[] speechRef;
        public int description;
    }
}
