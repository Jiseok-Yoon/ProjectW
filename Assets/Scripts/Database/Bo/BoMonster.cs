using ProjectW.SD;
using System;

namespace ProjectW.DB
{
    [Serializable]
    public class BoMonster : BoActor
    {
        public SDMonster sdMonster;

        public BoMonster(SDMonster sdMonster)
        {
            this.sdMonster = sdMonster;
        }
    }
}
