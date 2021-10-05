using UnityEngine;
using TMPro;
using ProjectW.DB;
using ProjectW.Util;
using ProjectW.Define;

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
            SetDialogueButton();
            Open();
        }

        public void SetDialogueButton()
        {
            var dialogueButtonPool = ObjectPoolManager.Instance.GetPool<DialogueButton>(PoolType.DialogueButton);
            var sdQuests = GameManager.SD.sdQuests;

            for (int i = 0; i < boDialogue.orderableQuests.Length; ++i)
            {
                var button = dialogueButtonPool.GetPoolableObject();
                var sdQuest = sdQuests.Find(_ => _.index == boDialogue.orderableQuests[i]);
                button.SetDialogueButton(sdQuest);
                button.transform.SetParent(buttonHolder);
            }
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
