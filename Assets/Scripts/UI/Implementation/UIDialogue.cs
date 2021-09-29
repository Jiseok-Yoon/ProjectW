using UnityEngine;
using TMPro;
using ProjectW.DB;

namespace ProjectW.UI
{
    public class UIDialogue : UIWindow
    {
        public TextMeshProUGUI speakerName;
        public TextMeshProUGUI dialogue;

        public BoDialogue boDialogue;

        public void SetDialogue(BoDialogue boDialogue)
        {
            this.boDialogue = boDialogue;

            speakerName.text = boDialogue.speaker;
            dialogue.text = boDialogue.speeches[0];

            Open();
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
