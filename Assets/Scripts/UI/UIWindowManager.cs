using ProjectW.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectW.UI
{
    /// <summary>
    /// ��� UIWindow Ŭ������ �����ϴ� �Ŵ��� 
    /// </summary>
    public class UIWindowManager : Singleton<UIWindowManager>
    {
        // UIWindowManager = UWM, UIWindow = UW

        /// <summary>
        /// UWM�� ��ϵ� Ȱ��ȭ �Ǿ��ִ� ��� UW�� ���� ����Ʈ
        /// </summary>
        private List<UIWindow> totalOpenWindows = new List<UIWindow>();
        /// <summary>
        /// UWM�� ��ϵ� ��� UW�� ���� ����Ʈ
        /// </summary>
        private List<UIWindow> totalUIWindows = new List<UIWindow>();
        /// <summary>
        /// UWM�� UW ��� �� ĳ���Ͽ� ��Ƶ� ��ųʸ�
        /// </summary>
        private Dictionary<string, UIWindow> cachedTotalUIWindowDic = new Dictionary<string, UIWindow>();
        /// <summary>
        /// UWM�� ��ϵ� UW�� �ν��Ͻ� ���� �� (UWM�� �̿��ؼ� Ư�� �ν��Ͻ� ���� �޼��带 ����� �ÿ�)
        /// �ش� �ν��Ͻ����� ĳ���Ͽ� ��Ƶ� ��ųʸ� (���� �ڵ�� ���� ������ UW�鸸 ���)
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
        /// totalUIWindows �ʵ忡 UW�� ����ϴ� ���
        /// </summary>
        /// <param name="uiWindow">����ϰ��� �ϴ� UW</param>
        public void AddTotalWindow(UIWindow uiWindow)
        {
            var key = uiWindow.GetType().Name;
            bool hasKey = false;

            // ��ü UW ����Ʈ�� ����ϰ��� �ϴ� �ν��Ͻ��� �ִ���
            // �Ǵ� ��ü UW ��ųʸ��� �ش� �ν��Ͻ� Ÿ�� �̸��� Ű ���� �����ϴ���
            if (totalUIWindows.Contains(uiWindow) || cachedTotalUIWindowDic.ContainsKey(key))
            {
                // ��ųʸ��� Ű������ ����� ���� ��, �ش� ����� null�� �ƴ϶�� ����
                if (cachedTotalUIWindowDic[key] != null)
                    return;
                // Ű ���� �ִµ� ����� null�̶�� �����ϰ� �ִ� UW�� �ν��Ͻ��� ���ٴ� ��
                else
                {
                    hasKey = true;
                    // ���� ��� �κ�������� ����ϴ� UI�� UWM�� ��ϵǾ��ִµ�
                    // �ΰ����� ����ȯ�ϸ鼭 �ش� UI�� �ν��Ͻ��� �ı��Ǽ� ����� null�� �����ִ� ��� ��..

                    // �ν��Ͻ��� ���ٸ� �ش� ���Ҹ� ����Ʈ���� �����
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
        /// totalOpenWindows ����Ʈ�� Ȱ��ȭ�� UW�� ����ϴ� ���
        /// </summary>
        /// <param name="uiWindow"/> ����ϰ����ϴ� UW �ν��Ͻ�</param> 
        public void AddOpenWindow(UIWindow uiWindow)
        {
            // totalOpenWindows ����Ʈ�� �̹� �����ϴ��� Ȯ��
            if (!totalOpenWindows.Contains(uiWindow))
                totalOpenWindows.Add(uiWindow);
        }

        /// <summary>
        /// totalOpenWindow ����Ʈ���� ��Ȱ��ȭ�� UW�� �����ϴ� ���
        /// </summary>
        /// <param name="uiWindow">�����ϰ����ϴ� UW �ν��Ͻ�</param>
        public void RemoveOpendWindow(UIWindow uiWindow)
        {
            if (totalOpenWindows.Contains(uiWindow))
                totalOpenWindows.Remove(uiWindow);
        }

        /// <summary>
        /// UWM�� ��ϵ� TŸ�� ������ UW�� ��ȯ�ϴ� ���
        /// </summary>
        /// <typeparam name="T">��ȯ�ް����ϴ� UW�� Ÿ��</typeparam>
        /// <returns>TŸ���� UW �ν��Ͻ�</returns>
        public T GetWindow<T>() where T : UIWindow
        {
            string key = typeof(T).Name;

            // ��Ż UW ��ųʸ��� ������ null ��ȯ
            if (!cachedTotalUIWindowDic.ContainsKey(key))
                return null;

            // ���� �޼��带 ���� �����ϴ� UW�� ��� ���������� �ν��Ͻ� ��ųʸ��� ��ϵȴ�.

            // �ν��Ͻ� ��ųʸ��� Ű�� ���ٸ� ��ü ��ųʸ����� UW �ν��Ͻ��� ������ 
            // TŸ������ ĳ���� �� ���
            if (!cachedInstanceDic.ContainsKey(key))
            {
                cachedInstanceDic.Add(key, (T)Convert.ChangeType(cachedTotalUIWindowDic[key], typeof(T)));
            }
            // �ν��Ͻ� ��ųʸ��� Ű�� �����ϴµ� ����� null�̶�� UW �ν��Ͻ��� TŸ������ ĳ���� �� ���
            else if (cachedInstanceDic[key].Equals(null))
            {
                cachedInstanceDic[key] = (T)Convert.ChangeType(cachedTotalUIWindowDic[key], typeof(T));
            }

            // �ν��Ͻ� ��ųʸ��� ���ؼ� UW ���̽� ���°� �ƴ� ���� �Ļ� Ŭ��������(TŸ��)��
            // ���ϴ� UW�� �Ļ� �ν��Ͻ��� ������ �� ����
            return (T)cachedInstanceDic[key];
        }

        /// <summary>
        /// UWM�� ��ϵ� ��� UW �� �ݴ� ���
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
        /// UWM�� ��ϵ� ��� UW�� �ʱ�ȭ�ϴ� ���
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
        /// ���� �����ִ� UW �� ���� �ֻ���(���� �������� ����) UW �ν��Ͻ��� ��ȯ�ϴ� ���
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
