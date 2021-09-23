using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectW.DB
{
    [Serializable]
    public class BoDialogue
    {
        public int currentSpeech;
        public string speaker;
        public string[] speeches;
    }
}
