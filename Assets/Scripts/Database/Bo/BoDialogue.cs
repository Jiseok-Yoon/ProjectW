﻿using System;

namespace ProjectW.DB
{
    [Serializable]
    public class BoDialogue
    {
        public int currentSpeech;
        public string speaker;
        public string[] speeches;
        public int[] orderableQuests;
    }
}
