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
        /// 현재 활성화된 다이얼로그 버튼들의 참조를 갖는 리스트
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
        /// 캐릭터가 상호작용 키를 눌러 대화를 다음으로 진행시켰을 경우 실행될 기능
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
        /// 대화가 끝났을 경우 실행될 기능
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
            // 다이얼로그 버튼 오브젝트 풀을 가져옴
            var pool = ObjectPoolManager.Instance.GetPool<DialogueButton>(Define.PoolType.DialogueButton);

            // 발행할 수 있는 퀘스트 수만큼 다이얼로그 버튼 활성화
            for (int i = 0; i < boDialogue.quests.Length; ++i)
            {
                var button = pool.GetPoolableObject();

                // 부모를 버튼 홀더로 설정 -> 결과: 버튼 홀더의 수직 레이아웃 그룹에 의해 버튼의 위치가 자동 정렬 됌
                button.transform.SetParent(buttonHolder);
                // 버튼을 초기화 -> 필수매개변수 : 어떤 타입의 버튼인지(ex: 퀘스트, 상점), 옵셔널 매개변수 : 타입에 따른 추가 데이터 
                button.Initialize(DialogueButtonType.Quest, boDialogue.quests[i]);
                
                dialogueButtons.Add(button);

                button.gameObject.SetActive(true);
            }

            // 아래부터는 NPC에 따라 버튼의 종류가 달라짐
            //  -> 다이얼로그 UI를 설정할 때 BoDialogue에 추가적으로 NPCType 을 받아와야 함

            // NPC 종류에 따라 추가적인 다이얼로그 버튼 활성
            // ex) NPC가 상인이라면, 상점 버튼 등을 추가 활성
        }
    }
}
