using ProjectW.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectW.DB
{
    [Serializable]
    public class BoNpc
    {
        public SDNpc sdNPC;

        public BoNpc(SDNpc sdNPC)
        {
            this.sdNPC = sdNPC;
        }
    }
}
