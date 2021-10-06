using UnityEngine;
using TMPro;
using ProjectW.DB;
using ProjectW.Util;
using static ProjectW.Define.Dialogue;
using System.Collections.Generic;

namespace ProjectW.UI
{
    public class UIDialogue : UIWindow
    {
        public TextMeshProUGUI speakerName;
        public TextMeshProUGUI dialogue;

        public Transform buttonHolder;
        public BoDialogue boDialogue;

        /// <summary>
        /// ���� Ȱ��ȭ�� ���̾�α� ��ư���� ������ ���� ����Ʈ
        /// </summary>
        private List<DialogueButton> dialogueButtons = new List<DialogueButton>();

        public void SetDialogue(BoDialogue boDialogue)
        {
            this.boDialogue = boDialogue;

            speakerName.text = boDialogue.speaker;
            dialogue.text = boDialogue.speeches[0];

            OnDialogueButtons();

            Open();
        }

        /// <summary>
        /// ĳ���Ͱ� ��ȣ�ۿ� Ű�� ���� ��ȭ�� �������� ��������� ��� ����� ���
        /// </summary>
        public void NextDialogue()
        {
            if (boDialogue.currentSpeech + 1 >= boDialogue.speeches.Length)
            {
                Close();
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
        public override void Close(bool force = false)
        {
            base.Close(force);

            boDialogue = null;

            var pool = ObjectPoolManager.Instance.GetPool<DialogueButton>(Define.PoolType.DialogueButton);

            for (int i = 0; i < dialogueButtons.Count; ++i)
            {
                pool.ReturnPoolableObject(dialogueButtons[i]);
            }
            dialogueButtons.Clear();
        }

        private void OnDialogueButtons()
        {
            // ���̾�α� ��ư ������Ʈ Ǯ�� ������
            var pool = ObjectPoolManager.Instance.GetPool<DialogueButton>(Define.PoolType.DialogueButton);

            // ������ �� �ִ� ����Ʈ ����ŭ ���̾�α� ��ư Ȱ��ȭ
            for (int i = 0; i < boDialogue.quests.Length; ++i)
            {
                var button = pool.GetPoolableObject();

                // �θ� ��ư Ȧ���� ���� -> ���: ��ư Ȧ���� ���� ���̾ƿ� �׷쿡 ���� ��ư�� ��ġ�� �ڵ� ���� ��
                button.transform.SetParent(buttonHolder);
                // ��ư�� �ʱ�ȭ -> �ʼ��Ű����� : � Ÿ���� ��ư����(ex: ����Ʈ, ����), �ɼų� �Ű����� : Ÿ�Կ� ���� �߰� ������ 
                button.Initialize(DialogueButtonType.Quest, boDialogue.quests[i]);
                
                dialogueButtons.Add(button);

                button.gameObject.SetActive(true);
            }

            // �Ʒ����ʹ� NPC�� ���� ��ư�� ������ �޶���
            //  -> ���̾�α� UI�� ������ �� BoDialogue�� �߰������� NPCType �� �޾ƿ;� ��

            // NPC ������ ���� �߰����� ���̾�α� ��ư Ȱ��
            // ex) NPC�� �����̶��, ���� ��ư ���� �߰� Ȱ��
        }
    }
}
