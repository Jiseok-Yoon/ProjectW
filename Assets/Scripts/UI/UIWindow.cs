using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectW.UI
{
    // UIWindow�� ĵ���� �׷��� ������ ���ԉ�
    [RequireComponent(typeof(CanvasGroup))]
    /// <summary>
    /// ���׸��� �˾�, UI ���� UIElement�� ������ ��� UI�� ���̽� Ŭ����
    /// </summary>
    public class UIWindow : MonoBehaviour
    {
        /// <summary>
        /// ĵ�����׷��� ���� UI�� Ȱ��/��Ȱ��ȭ �ϴ� ȿ���� �� (���İ� 0~1�� ���)
        /// ��Ȱ��ȭ �� ������ ��ü�� ��Ȱ��ȭ�Ǵ� ���� �ƴϹǷ� UI �Է��� ���� (��� ����ĳ��Ʈ)
        /// </summary>
        private CanvasGroup cachedCanvasGroup;
        public CanvasGroup CachedCanvasGroup
        {
            get
            {
                if (cachedCanvasGroup == null)
                    cachedCanvasGroup = GetComponent<CanvasGroup>();

                return cachedCanvasGroup;
            }
        }

        /// <summary>
        /// �ش� UI�� esc Ű�� ���� �� �ְ� �����Ұ���
        /// </summary>
        public bool canCloseESC;

        /// <summary>
        /// UI�� Ȱ��ȭ ����
        /// </summary>
        public bool isOpen;

        public virtual void Start()
        {
            InitWindow();
        }

        /// <summary>
        /// UI �ʱ�ȭ ���
        /// </summary>
        public virtual void InitWindow()
        {
            UIWindowManager.Instance.AddTotalWindow(this);

            if (isOpen)
                Open(true);
            else
                Close(true);
        }

        /// <summary>
        /// UI�� Ȱ��ȭ�ϴ� ���
        /// </summary>
        /// <param name="force">������ Ȱ��ȭ ��ų����</param>
        public virtual void Open(bool force = false)
        {
            if (!isOpen || force)
            {
                isOpen = true;
                UIWindowManager.Instance.AddOpenWindow(this);
                SetCanvasGroup(true);
            }
        }

        /// <summary>
        /// UI�� ��Ȱ��ȭ�ϴ� ���
        /// </summary>
        /// <param name="force">������ ��Ȱ��ȭ ��ų����</param>
        public virtual void Close(bool force = false)
        {
            if (isOpen || force)
            {
                isOpen = false;
                UIWindowManager.Instance.RemoveOpendWindow(this);
                SetCanvasGroup(false);
            }
        }

        /// <summary>
        /// Ȱ��ȭ ���¿� ���� ĵ���� �׷� ���� �ʵ带 ����
        /// </summary>
        /// <param name="isActive"></param>
        private void SetCanvasGroup(bool isActive)
        {
            CachedCanvasGroup.alpha = Convert.ToInt32(isActive);
            cachedCanvasGroup.interactable = isActive;
            cachedCanvasGroup.blocksRaycasts = isActive;
        }
    }
}