using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectW.Define.Npc;

namespace ProjectW.SD
{
    public class SDNpc : StaticData
    {
        public string name;
        public NPCType npcType;
        public int[] questRef;
        public int[] needQusetRef;
        public int stageRef;
        public float[] stagePos;
        public string ResourcePath;
    }
}
