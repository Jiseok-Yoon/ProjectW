using ProjectW.SD;
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
        public Transform listWindow; // 리스트 창 홀더 (UIQuest -> QuestList 객체)

        [Header("Quest Order Window Ref")]
        public Button refuse; // 거절 버튼 
        public Button accept; // 수락 버튼
        public TextMeshProUGUI orderTitle; // 수주받은 퀘스트 타이틀
        public TextMeshProUGUI orderDescription; // 수주받은 퀘스트 내용
        public Transform orderWindow; // 오더 창 홀더 (UIQuest -> QuestOrder 객체)

        private void Start()
        {
            refuse.onClick.AddListener(OnClickedRefuse());
            accept.onClick.AddListener(OnClickedAccept());
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                Open(QuestWindow.List);
            }
        }

        /// <summary>
        /// 퀘스트창을 오픈하는 기능
        /// </summary>
        /// <param name="questWindow">열고자하는 퀘스트 창의 타입</param>
        public void Open(QuestWindow questWindow, SDQuest orderQuest = null)
        {
            if (isOpen)
                return;

            // 베이스에 있던 오픈 호출
            Open(); 

            currentWindow = questWindow;
            var isListWindow = currentWindow == QuestWindow.List;

            // 창 타입에 따라 각종 창을 활성/비활성화
            listWindow.gameObject.SetActive(isListWindow);
            orderWindow.gameObject.SetActive(!isListWindow);
        }

        public void OnClickedRefuse()
        {

        }
        public void OnClickedAccept()
        {

        }
    }
}
