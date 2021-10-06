using System;

namespace ProjectW.DB
{
    [Serializable]
    public class BoDialogue
    {
        public int currentSpeech;
        public int[] quests;
        public string speaker;
        public string[] speeches;
    }
}
