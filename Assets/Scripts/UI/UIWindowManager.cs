using ProjectW.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectW.UI
{
    /// <summary>
    /// 모든 UIWindow 클래스를 관리하는 매니저 
    /// </summary>
    public class UIWindowManager : Singleton<UIWindowManager>
    {
        // UIWindowManager = UWM, UIWindow = UW

        /// <summary>
        /// UWM에 등록된 활성화 되어있는 모든 UW를 갖는 리스트
        /// </summary>
        private List<UIWindow> totalOpenWindows = new List<UIWindow>();
        /// <summary>
        /// UWM에 등록된 모든 UW를 갖는 리스트
        /// </summary>
        private List<UIWindow> totalUIWindows = new List<UIWindow>();
        /// <summary>
        /// UWM에 UW 등록 시 캐싱하여 담아둘 딕셔너리
        /// </summary>
        private Dictionary<string, UIWindow> cachedTotalUIWindowDic = new Dictionary<string, UIWindow>();
        /// <summary>
        /// UWM에 등록된 UW의 인스턴스 접근 시 (UWM를 이용해서 특정 인스턴스 접근 메서드를 사용할 시에)
        /// 해당 인스턴스들을 캐싱하여 담아둘 딕셔너리 (내가 코드로 직접 접근한 UW들만 담김)
        /// </summary>
        private Dictionary<string, object> cachedInstanceDic = new Dictionary<string, object>();

        public void Initialize()
        {
            InitAllWindow();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var targetWindow = GetTopWindow();

                if (targetWindow != null && targetWindow.canCloseESC)
                {
                    targetWindow.Close();
                }
            }
        }

        /// <summary>
        /// totalUIWindows 필드에 UW를 등록하는 기능
        /// </summary>
        /// <param name="uiWindow">등록하고자 하는 UW</param>
        public void AddTotalWindow(UIWindow uiWindow)
        {
            var key = uiWindow.GetType().Name;
            bool hasKey = false;

            // 전체 UW 리스트에 등록하고자 하는 인스턴스가 있는지
            // 또는 전체 UW 딕셔너리에 해당 인스턴스 타입 이름의 키 값이 존재하는지
            if (totalUIWindows.Contains(uiWindow) || cachedTotalUIWindowDic.ContainsKey(key))
            {
                // 딕셔너리에 키값으로 밸류에 접근 시, 해당 밸류가 null이 아니라면 리턴
                if (cachedTotalUIWindowDic[key] != null)
                    return;
                // 키 값은 있는데 밸류가 null이라면 참조하고 있는 UW의 인스턴스가 없다는 것
                else
                {
                    hasKey = true;
                    // 예를 들어 로비씬에서만 사용하던 UI가 UWM에 등록되어있는데
                    // 인겜으로 씬전환하면서 해당 UI의 인스턴스가 파괴되서 밸류가 null로 박혀있는 경우 등..

                    // 인스턴스가 없다면 해당 원소를 리스트에서 지운다
                    for (int i = 0; i < totalUIWindows.Count; ++i)
                    {
                        if (totalUIWindows[i] == null)
                            totalUIWindows.RemoveAt(i);
                    }
                }
            }

            totalUIWindows.Add(uiWindow);

            if (hasKey)
                cachedTotalUIWindowDic[key] = uiWindow;
            else
                cachedTotalUIWindowDic.Add(key, uiWindow);
        }

        /// <summary>
        /// totalOpenWindows 리스트에 활성화된 UW를 등록하는 기능
        /// </summary>
        /// <param name="uiWindow"/> 등록하고자하는 UW 인스턴스</param> 
        public void AddOpenWindow(UIWindow uiWindow)
        {
            // totalOpenWindows 리스트에 이미 존재하는지 확인
            if (!totalOpenWindows.Contains(uiWindow))
                totalOpenWindows.Add(uiWindow);
        }

        /// <summary>
        /// totalOpenWindow 리스트에서 비활성화된 UW를 제거하는 기능
        /// </summary>
        /// <param name="uiWindow">제거하고자하는 UW 인스턴스</param>
        public void RemoveOpendWindow(UIWindow uiWindow)
        {
            if (totalOpenWindows.Contains(uiWindow))
                totalOpenWindows.Remove(uiWindow);
        }

        /// <summary>
        /// UWM에 등록된 T타입 유형의 UW를 반환하는 기능
        /// </summary>
        /// <typeparam name="T">반환받고자하는 UW의 타입</typeparam>
        /// <returns>T타입의 UW 인스턴스</returns>
        public T GetWindow<T>() where T : UIWindow
        {
            string key = typeof(T).Name;

            // 토탈 UW 딕셔너리에 없으면 null 반환
            if (!cachedTotalUIWindowDic.ContainsKey(key))
                return null;

            // 현재 메서드를 통해 접근하는 UW는 모두 최종적으로 인스턴스 딕셔너리에 등록된다.

            // 인스턴스 딕셔너리에 키가 없다면 전체 딕셔너리에서 UW 인스턴스를 가져와 
            // T타입으로 캐스팅 후 등록
            if (!cachedInstanceDic.ContainsKey(key))
            {
                cachedInstanceDic.Add(key, (T)Convert.ChangeType(cachedTotalUIWindowDic[key], typeof(T)));
            }
            // 인스턴스 딕셔너리에 키는 존재하는데 밸류가 null이라면 UW 인스턴스를 T타입으로 캐스팅 후 등록
            else if (cachedInstanceDic[key].Equals(null))
            {
                cachedInstanceDic[key] = (T)Convert.ChangeType(cachedTotalUIWindowDic[key], typeof(T));
            }

            // 인스턴스 딕셔너리를 통해서 UW 베이스 형태가 아닌 최종 파생 클래스형태(T타입)로
            // 원하는 UW의 파생 인스턴스를 가져올 수 있음
            return (T)cachedInstanceDic[key];
        }

        /// <summary>
        /// UWM에 등록된 모든 UW 를 닫는 기능
        /// </summary>
        public void CloseAll()
        {
            for (int i = 0; i < totalUIWindows.Count; ++i)
            {
                if (totalUIWindows[i] != null)
                    totalUIWindows[i].Close(true);
            }
        }

        /// <summary>
        /// UWM에 등록된 모든 UW를 초기화하는 기능
        /// </summary>
        public void InitAllWindow()
        {
            for (int i = 0; i < totalUIWindows.Count; ++i)
            {
                if (totalUIWindows[i] != null)
                    totalUIWindows[i].InitWindow();
            }
        }

        /// <summary>
        /// 현재 열려있는 UW 중 가장 최상위(가장 마지막에 열린) UW 인스턴스를 반환하는 기능
        /// </summary>
        /// <returns></returns>
        public UIWindow GetTopWindow()
        {
            for (int i = totalUIWindows.Count - 1; i > 0; --i)
            {
                if (totalUIWindows[i] != null)
                    return totalUIWindows[i];
            }

            return null;
        }
    }
}
