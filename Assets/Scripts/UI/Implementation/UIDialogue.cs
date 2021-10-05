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
        /// ĳ���Ͱ� ��ȣ�ۿ� Ű�� ���� ��ȭ�� �������� ��������� ��� ����� ���
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
        /// ��ȭ�� ������ ��� ����� ���
        /// </summary>
        public void EndDialogue()
        {
            Close();
            boDialogue = null;
        }
    }
}
