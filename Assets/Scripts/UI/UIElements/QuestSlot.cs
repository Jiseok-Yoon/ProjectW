using ProjectW.DB;
using ProjectW.SD;
using ProjectW.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectW.UI
{
    public class QuestSlot : MonoBehaviour, IPoolableObject
    {
        public bool CanRecycle { get; set; } = true;

        public Image icon;
        public TextMeshProUGUI title;


        public void SetQuest(SDQuest sdQuest)
        {
            title.text = sdQuest.name;
        }

        public void SetQuest(BoQuestProgress boQuestProgress)
        {
            SetQuest(boQuestProgress.sdQuest);
        }
    }
}
