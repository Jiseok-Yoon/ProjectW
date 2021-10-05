using ProjectW.SD;
using ProjectW.Util;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ProjectW.UI
{
    public class DialogueButton : MonoBehaviour, IPoolableObject
    {
        public bool CanRecycle { get; set; }
        public SDQuest sdQuest;
        public TextMeshProUGUI tmp;

        public void Start()
        {
            tmp = GetComponent<TextMeshProUGUI>();
        }
        public void SetDialogueButton(SDQuest sdQuest)
        {
            this.sdQuest = sdQuest;
            tmp.text = sdQuest.name;
        }
    }

}
