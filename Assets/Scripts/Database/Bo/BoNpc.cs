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
        SDNpc sdNpc;

        BoNpc(SDNpc sdNpc)
        {
            this.sdNpc = sdNpc;
        }
    }
}
