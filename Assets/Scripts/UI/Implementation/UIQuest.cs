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
        public Button progressTab; // ������
        public Button completedTab; // �Ϸ���
        public Transform listContentHolder; // ��ũ�Ѻ��� ������ Ȧ��
        public Transform listWindow; // ����Ʈ â Ȧ�� (UIQuest -> QuestList ��ü)
        public QuestTabType currentTab;
        private List<QuestSlot> questSlots = new List<QuestSlot>();

        [Header("Quest Order Window Ref")]
        public Button refuse; // ���� ��ư 
        public Button accept; // ���� ��ư
        public TextMeshProUGUI orderTitle; // ���ֹ��� ����Ʈ Ÿ��Ʋ
        public TextMeshProUGUI orderDescription; // ���ֹ��� ����Ʈ ����
        public Transform orderWindow; // ���� â Ȧ�� (UIQuest -> QuestOrder ��ü)

        public override void Start()
        {
            base.Start();

            progressTab.onClick.AddListener(TabOnClicked);
            completedTab.onClick.AddListener(TabOnClicked);

            // ����â���� ���� ��ư Ŭ�� �� �̺�Ʈ ���ε�
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
        /// ����Ʈâ�� �����ϴ� ���
        /// </summary>
        /// <param name="questWindow">�������ϴ� ����Ʈ â�� Ÿ��</param>
        public void Open(QuestWindow questWindow, SDQuest orderQuest = null)
        {
            if (isOpen)
            {
                Close();
                return;
            }
                

            currentWindow = questWindow;
            var isListWindow = currentWindow == QuestWindow.List;

            if (isListWindow)
            {
                SetListWindow();
            }
            else
                SetOrderWindow(orderQuest);

            // â Ÿ�Կ� ���� ���� â�� Ȱ��/��Ȱ��ȭ
            listWindow.gameObject.SetActive(isListWindow);
            orderWindow.gameObject.SetActive(!isListWindow);

            // ���̽��� �ִ� ���� ȣ��
            Open();
        }

        public override void Close(bool force = false)
        {
            ListWindowClear();

            base.Close(force);
        }

        private void SetOrderWindow(SDQuest sdQuest)
        {
            // ���� ����Ʈ �̸� ����
            orderTitle.text = sdQuest.name;
            // ���� ����Ʈ ���� ����
            orderDescription.text = GameManager.SD.sdString.Where(_ => _.index == sdQuest.description).SingleOrDefault().kr;

            // ���� ��ư �̺�Ʈ ���ε�
            // ���� ��ư�� ���� �� ���� ���� ��ų ��� ��ü�� �Ȱ�����, ���޵Ǵ� �����Ͱ� �޶�������
            // ������ư�� ������ �� ������ ����Ʈ�� ���� DB�� ����Ʈ�� �߰��ϴ� ������ ������
            // � ����Ʈ���� <- �����Ͱ� �޶���
            
            // ���� ��ư�� ���ε� �� �̺�Ʈ�� ���� �����.
            accept.onClick.RemoveAllListeners();
            // ���� ����Ʈ�� ���� ������ ������ �ѱ�鼭 db�� �߰��ش޶�� ��û�� �Ѵ�.
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
                        questSlots.Add(progressQuest);
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
                        questSlots.Add(completedQuest);
                    }
                    break;
            }

        }
        private void ListWindowClear()
        {
            var pool = ObjectPoolManager.Instance.GetPool<QuestSlot>(Define.PoolType.QuestSlot);

            for (int i = 0; i < questSlots.Count; ++i)
            {
                questSlots[i].Clear();
                pool.ReturnPoolableObject(questSlots[i]);
            }
            questSlots.Clear();
        }

        private void TabOnClicked()
        {
            ListWindowClear();
            switch (currentTab)
            {
                case QuestTabType.Progress:
                    currentTab = QuestTabType.Complete;
                    break;
                case QuestTabType.Complete:
                    currentTab = QuestTabType.Progress;
                    break;
            }
            SetListWindow();
        }
    }
}
