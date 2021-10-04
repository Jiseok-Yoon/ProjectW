using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ProjectW.Define.Quest;

namespace ProjectW.UI
{
    public class UIQuest : UIWindow
    {
        public QuestWindow currentWindow;

        [Header("Quest List Window Ref")]
        public Button progressTab; // 진행탭
        public Button completedTab; // 완료탭
        public Transform listContentHolder; // 스크롤뷰의 콘텐츠 홀더
        public Transform listWindow; // 리스트 창 홀더(UIQuest -> QuestList 객체)

        [Header("Quest Order Window Ref")]
        public Button refuse; // 거절 버튼
        public Button accept; // 수락 버튼
        public TextMeshProUGUI orderTitle; // 수주받은 퀘스트 타이틀
        public TextMeshProUGUI orderDescription; // 수주받은 퀘스트 내용
        public Transform orderWindow; // 오더 창 홀더 (UIQuest -> QuestOrder 객체)

        public void Open(QuestWindow questWindow)
        {
            if (isOpen)
                return;

            // 베이스의 오픈 호출
            Open();

            currentWindow = questWindow;
            var isListWindow = currentWindow == QuestWindow.List;

            // 창 타입에 따라 각종 창을 활성/비활성화
            listWindow.gameObject.SetActive(isListWindow);
            orderWindow.gameObject.SetActive(!isListWindow);

        }

        private void Initialize()
        {

        }
    }
}

