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
        public Button progressTab; // ������
        public Button completedTab; // �Ϸ���
        public Transform listContentHolder; // ��ũ�Ѻ��� ������ Ȧ��
        public Transform listWindow; // ����Ʈ â Ȧ�� (UIQuest -> QuestList ��ü)

        [Header("Quest Order Window Ref")]
        public Button refuse; // ���� ��ư 
        public Button accept; // ���� ��ư
        public TextMeshProUGUI orderTitle; // ���ֹ��� ����Ʈ Ÿ��Ʋ
        public TextMeshProUGUI orderDescription; // ���ֹ��� ����Ʈ ����
        public Transform orderWindow; // ���� â Ȧ�� (UIQuest -> QuestOrder ��ü)

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
        /// ����Ʈâ�� �����ϴ� ���
        /// </summary>
        /// <param name="questWindow">�������ϴ� ����Ʈ â�� Ÿ��</param>
        public void Open(QuestWindow questWindow, SDQuest orderQuest = null)
        {
            if (isOpen)
                return;

            // ���̽��� �ִ� ���� ȣ��
            Open(); 

            currentWindow = questWindow;
            var isListWindow = currentWindow == QuestWindow.List;

            // â Ÿ�Կ� ���� ���� â�� Ȱ��/��Ȱ��ȭ
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
