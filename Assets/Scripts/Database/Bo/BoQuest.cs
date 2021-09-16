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
        SDQuest sdQuest;

        public BoQuest(SDQuest sdQuest)
        {
            this.sdQuest = sdQuest;
        }
    }
}
