using ProjectW.DB;
using ProjectW.Network;
using ProjectW.SD;
using ProjectW.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public QuestTabType currentTab;
        private List<QuestSlot> questSlots = new List<QuestSlot>();

        [Header("Quest Order Window Ref")]
        public Button refuse; // 거절 버튼 
        public Button accept; // 수락 버튼
        public TextMeshProUGUI orderTitle; // 수주받은 퀘스트 타이틀
        public TextMeshProUGUI orderDescription; // 수주받은 퀘스트 내용
        public Transform orderWindow; // 오더 창 홀더 (UIQuest -> QuestOrder 객체)

        public override void Start()
        {
            base.Start();

            // 수주창에서 거절 버튼 클릭 시 이벤트 바인딩
            refuse.onClick.AddListener(() => { Close(); });
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

            currentWindow = questWindow;
            var isListWindow = currentWindow == QuestWindow.List;

            if (isListWindow)
            {
                SetListWindow();
            }
            else
                SetOrderWindow(orderQuest);

            // 창 타입에 따라 각종 창을 활성/비활성화
            listWindow.gameObject.SetActive(isListWindow);
            orderWindow.gameObject.SetActive(!isListWindow);

            // 베이스에 있던 오픈 호출
            Open();
        }

        public override void Close(bool force = false)
        {
            base.Close(force);
        }

        private void SetOrderWindow(SDQuest sdQuest)
        {
            // 수주 퀘스트 이름 설정
            orderTitle.text = sdQuest.name;
            // 수주 퀘스트 내용 설정
            orderDescription.text = GameManager.SD.sdString.Where(_ => _.index == sdQuest.description).SingleOrDefault().kr;

            // 수락 버튼 이벤트 바인딩
            // 수락 버튼을 누를 때 마다 실행 시킬 기능 자체는 똑같은데, 전달되는 데이터가 달라져야함
            // 수락버튼을 눌렀을 때 수락한 퀘스트를 유저 DB의 퀘스트에 추가하는 행위는 같은데
            // 어떤 퀘스트인지 <- 데이터가 달라짐
            
            // 수락 버튼에 바인딩 된 이벤트를 전부 지운다.
            accept.onClick.RemoveAllListeners();
            // 수주 퀘스트에 대한 정보를 서버에 넘기면서 db에 추가해달라는 요청을 한다.
            accept.onClick.AddListener(() => {

                ServerManager.Server.AddQuest(0, sdQuest.index, 
                    new ResponsHandler<DtoQuestProgress>(dtoQuestProgress => {

                        var boQuestProgress = new BoQuestProgress(dtoQuestProgress);
                        GameManager.User.boQuest.progressQuests.Add(boQuestProgress);
                        Close();
                    }, 
                    failed => {  
                    
                    }));
            });
        }

        private void SetListWindow()
        {
            var progressQuests = GameManager.User.boQuest.progressQuests;
            var completedQuests = GameManager.User.boQuest.completedQuests;

            var pool = ObjectPoolManager.Instance.GetPool<QuestSlot>(Define.PoolType.QuestSlot);

            switch (currentTab)
            {
                case QuestTabType.Progress:
                    for (int i = 0; i < progressQuests.Count; ++i)
                    {
                        var progressQuest = pool.GetPoolableObject();
                        progressQuest.transform.SetParent(listContentHolder);
                        progressQuest.transform.localScale = Vector3.one;
                        progressQuest.SetQuest(progressQuests[i]);
                        progressQuest.gameObject.SetActive(true);
                    }
                    break;
                case QuestTabType.Complete:
                    for (int i = 0; i < completedQuests.Count; ++i)
                    {
                        var completedQuest = pool.GetPoolableObject();
                        completedQuest.transform.SetParent(listContentHolder);
                        completedQuest.transform.localScale = Vector3.one;
                        completedQuest.SetQuest(completedQuests[i]);
                        completedQuest.gameObject.SetActive(true);
                    }
                    break;
            }

        }
    }
}
