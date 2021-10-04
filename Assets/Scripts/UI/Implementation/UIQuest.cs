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
        public Transform listWindow; // ����Ʈ â Ȧ��(UIQuest -> QuestList ��ü)

        [Header("Quest Order Window Ref")]
        public Button refuse; // ���� ��ư
        public Button accept; // ���� ��ư
        public TextMeshProUGUI orderTitle; // ���ֹ��� ����Ʈ Ÿ��Ʋ
        public TextMeshProUGUI orderDescription; // ���ֹ��� ����Ʈ ����
        public Transform orderWindow; // ���� â Ȧ�� (UIQuest -> QuestOrder ��ü)

        public void Open(QuestWindow questWindow)
        {
            if (isOpen)
                return;

            // ���̽��� ���� ȣ��
            Open();

            currentWindow = questWindow;
            var isListWindow = currentWindow == QuestWindow.List;

            // â Ÿ�Կ� ���� ���� â�� Ȱ��/��Ȱ��ȭ
            listWindow.gameObject.SetActive(isListWindow);
            orderWindow.gameObject.SetActive(!isListWindow);

        }

        private void Initialize()
        {

        }
    }
}

