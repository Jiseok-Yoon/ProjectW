using System;

namespace ProjectW.SD
{
    [Serializable]
    public class SDNPC : StaticData
    {
        public string name;
        public Define.NPC.NPCType type;
        public int[] questRef;
        public int[] needQuestRef;
        public int stageRef;
        public float[] stagePos;
        public int[] speechRef;
        public string resourcePath;
    }
}
