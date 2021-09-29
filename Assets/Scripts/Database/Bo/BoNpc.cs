using System;
using ProjectW.SD;

namespace ProjectW.DB
{
    [Serializable]
    public class BoNPC 
    {
        // 현재 NPC가 플레이어와 상호작용 중인지를 나타내는 필드
        public bool isInteraction;

        public SDNPC sdNPC;

        public BoNPC(SDNPC sdNPC)
        {
            this.sdNPC = sdNPC;
        }
    }
}
