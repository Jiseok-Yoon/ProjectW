using UnityEngine;
using TMPro;
using ProjectW.DB;

namespace ProjectW.UI
{
    public class UIDialogue : UIWindow
    {
        public TextMeshProUGUI speakerName;
        public TextMeshProUGUI dialogue;

        public Transform buttonHolder;
        public BoDialogue boDialogue;

        public void SetDialogue(BoDialogue boDialogue)
        {
            this.boDialogue = boDialogue;

            speakerName.text = boDialogue.speaker;
            dialogue.text = boDialogue.speeches[0];

            Open();
        }

        /// <summary>
        /// 캐릭터가 상호작용 키를 눌러 대화를 다음으로 진행시켰을 경우 실행될 기능
        /// </summary>
        public void NextDialogue()
        {
            if (boDialogue.currentSpeech + 1 >= boDialogue.speeches.Length)
            {
                EndDialogue();
            }
            else
            {
                ++boDialogue.currentSpeech;
                dialogue.text = boDialogue.speeches[boDialogue.currentSpeech];
            }
        }

        /// <summary>
        /// 대화가 끝났을 경우 실행될 기능
        /// </summary>
        public void EndDialogue()
        {
            Close();
            boDialogue = null;
        }
    }
}
