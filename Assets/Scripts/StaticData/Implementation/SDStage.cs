using System;

namespace ProjectW.SD
{
    [Serializable]
    public class SDStage : StaticData
    {
        public string name;
        public int[] genMonsters;
        public int[] spawnArea;
        public int[] warpStageRef;
        public string resourcePath;
    }
}
