using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectW.UI
{
    // UIWindow는 캔버스 그룹을 강제로 갖게됌
    [RequireComponent(typeof(CanvasGroup))]
    /// <summary>
    /// 조그마한 팝업, UI 내의 UIElement를 제외한 모든 UI의 베이스 클래스
    /// </summary>
    public class UIWindow : MonoBehaviour
    {
        /// <summary>
        /// 캔버스그룹을 통해 UI를 활성/비활성화 하는 효과를 줌 (알파값 0~1을 사용)
        /// 비활성화 시 실제로 객체가 비활성화되는 것이 아니므로 UI 입력을 차단 (블록 레이캐스트)
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
        /// 해당 UI를 esc 키로 닫을 수 있게 설정할건지
        /// </summary>
        public bool canCloseESC;

        /// <summary>
        /// UI의 활성화 상태
        /// </summary>
        public bool isOpen;

        public virtual void Start()
        {
            InitWindow();
        }

        /// <summary>
        /// UI 초기화 기능
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
        /// UI를 활성화하는 기능
        /// </summary>
        /// <param name="force">강제로 활성화 시킬건지</param>
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
        /// UI를 비활성화하는 기능
        /// </summary>
        /// <param name="force">강제로 비활성화 시킬건지</param>
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
        /// 활성화 상태에 따라 캔버스 그룹 내의 필드를 설정
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