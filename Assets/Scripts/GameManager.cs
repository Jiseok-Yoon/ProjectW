using ProjectW.DB;
using ProjectW.Define;
using ProjectW.SD;
using ProjectW.UI;
using ProjectW.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectW
{
    /// <summary>
    /// ���ӿ� ����ϴ� ��� �����͸� �����ϴ� Ŭ����
    /// �߰��� ������ �� ���� ��� ���� ū �帧���� ��Ʈ���ϱ⵵ ��
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        /// <summary>
        /// �ش� �ʵ尡 true��� ���̼����� ���
        /// </summary>
        public bool useDummyServer;

        public float loadProgress;

        /// <summary>
        /// ���� ������ (DB ������)
        /// </summary>
        [SerializeField]
        private BoUser boUser = new BoUser();
        public static BoUser User => Instance.boUser;

        /// <summary>
        /// ��ȹ ������
        /// </summary>
        // private �ʵ带 �ν����Ϳ� �����Ű��
        [SerializeField]
        private StaticDataModule sd = new StaticDataModule();
        public static StaticDataModule SD => Instance.sd;

        protected override void Awake()
        {
            base.Awake();

            if (gameObject == null)
                return;

            // ���� ����ǵ� ��ü�� �ı����� �ʰ�
            DontDestroyOnLoad(this);

            var titleController = FindObjectOfType<TitleController>();
            titleController?.Initialize();
            // if (titleController != null)
            // {
            //      titleController.Intialize();
            // }
        }

        /// <summary>
        /// �ۿ� �⺻ ����
        /// </summary>
        public void OnApplicationSetting()
        {
            // ��������ȭ ����
            QualitySettings.vSyncCount = 0;
            // ���� �������� 60���� ����
            Application.targetFrameRate = 60;
            // �� ���� �� ��ð� ��� �ÿ��� ȭ���� ������ �ʰ�
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        /// <summary>
        /// ���� �񵿱�� �ε��ϴ� ���
        /// �ٸ� �� ���� ��ȯ�� ��� (ex: Title -> InGame)
        /// </summary>
        /// <param name="sceneName">�ε��� ���� �̸��� ���� ������</param>
        /// <param name="loadCoroutien">�� ��ȯ �� �ε� ������ �̸� ó���� �۾�</param>
        /// <param name="loadComplete">�� ��ȯ �Ϸ� �� ������ ���</param>
        public void LoadScene(SceneType sceneName, IEnumerator loadCoroutine = null, Action loadComplete = null)
        {
            StartCoroutine(WaitForLoad());

            // ���� ��ȯ�� �� ex) Title -> Ingame ��ȯ�� �� �� �� ���� ��ȯ�ϴ� ���� �ƴ϶�
            // �߰��� �ε� ���� �̿�, ���������� Title -> Loading -> Ingame

            // �ڷ�ƾ -> ����Ƽ���� Ư�� �۾��� �񵿱�� ������ �� �ְ� ���ִ� ���
            //           (�񵿱�ó�� ������ �Ǵµ�.. ������ �񵿱�� �ƴ�)

            // LoadScene �޼��忡���� ��밡���� �����Լ� ����
            IEnumerator WaitForLoad()
            {
                // �ε� ������¸� ��Ÿ�� (0~1)
                loadProgress = 0;

                // �񵿱�� �ε� ������ ��ȯ (�񵿱�� ����ϴ� ������ �� ��ȯ�� ȭ���� ������ �ʰ� �ϱ� ���ؼ�)
                yield return SceneManager.LoadSceneAsync(SceneType.Loading.ToString());

                // �ε� ������ ��ȯ �Ϸ� �Ŀ� �Ʒ� ������ ����

                // ���� �����ϰ����ϴ� ���� �߰�
                var asyncOper = SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);
                // ��� �߰��� ���� ��Ȱ��ȭ
                // ����? 2���� ���� Ȱ��ȭ�Ǿ��ִٸ� ���� ��Ʈ������ �ʴ� �̻� � ���� ȭ�鿡 ������ ��
                // ���� �����ϰ��� �ϴ� ���� �߰��ص� ���� ����ڿ��Դ� �ε� ���� �����ְ�
                // ���� �����ϰ��� �ϴ� ���� �ʿ��� ���ҽ����� �θ��� �۾��� �ϱ� ���ؼ�
                asyncOper.allowSceneActivation = false;

                // �����ϰ��� �ϴ� ���� �ʿ��� �۾��� �����Ѵٸ� ����
                if (loadCoroutine != null)
                {
                    // �ش� �۾��� �Ϸ�� �� ���� ���
                    yield return StartCoroutine(loadCoroutine);
                }

                // ���� �۾��� �Ϸ�� �Ŀ� �Ʒ� ������ ������


                // �񵿱�� �ε��� ���� Ȱ��ȭ�� �Ϸ���� �ʾҴٸ� Ư�� �۾��� �ݺ�
                while (!asyncOper.isDone)
                {
                    // loadProgress ���� �̿��ؼ� ��������� �ε��ٸ� ���� ���� ���¸� �˷���

                    if (loadProgress >= .9f)
                    {
                        loadProgress = 1f;

                        // �ε��ٰ� ���������� ���� ���� Ȯ���ϱ� ���� 1�� ���� ���
                        yield return new WaitForSeconds(1f);

                        // �����ϰ��� �ϴ� ���� �ٽ� Ȱ��ȭ
                        // (isDone�� ���� Ȱ�����°� �ƴ϶�� progress�� 1�� �Ǿ true�� �ȉ�)
                        asyncOper.allowSceneActivation = true;
                    }
                    else
                        loadProgress = asyncOper.progress;

                    // �ڷ�ƾ ������ �ݺ��� ��� �� ������ �� �� ���� ��, ���� ������ ���� �� �� �ְ� yield return
                    yield return null;
                }

                // ���� �ݺ� �۾��� �� ���� �� �Ʒ� ���� ����

                // �ε� ������ ���� ���� �ʿ��� �۾��� ���� ���������Ƿ� �ε����� ��Ȱ��ȭ ��Ŵ
                yield return SceneManager.UnloadSceneAsync(SceneType.Loading.ToString());

                // ��� �۾��� �Ϸ�Ǿ����Ƿ� ��� �۾� �Ϸ� �� �����ų ������ �ִٸ� ����
                loadComplete?.Invoke();
            }
        }

        /// <summary>
        /// �ΰ��� ������ �������� ��ȯ �� ��� (���� ���� �����ϴ� ���� �ƴ�, �ε����� �̿��Ͽ� �� ��ȯó�� ���̰� ����)
        /// �ε� ���� �̿��Ͽ� �����ϰ��� �ϴ� ���������� �ʿ��� ���ҽ� �ε峪 �ʱ�ȭ �۾����� ó��
        /// ex) �ΰ��Ӿ�(���۸���) -> �ΰ��Ӿ�(�ʺ��ڻ����)
        /// </summary>
        /// <param name="loadCoroutine"></param>
        /// <param name="loadComplete"></param>
        public void OnAddtiveLoadingScene(IEnumerator loadCoroutine = null, Action loadComplete = null)
        {
            StartCoroutine(WaitForLoad());

            IEnumerator WaitForLoad()
            {
                loadProgress = 0;

                var asyncOper = SceneManager.LoadSceneAsync(SceneType.Loading.ToString(), LoadSceneMode.Additive);

                #region �ε��� ������� ó��
                while (!asyncOper.isDone)
                {
                    loadProgress = asyncOper.progress;
                    yield return null;
                }

                // uiLoading �ν��Ͻ��� ������ ã��
                UILoading uiLoading = null;
                while (uiLoading == null)
                {
                    // �ε����� �񵿱�� �ε��ϰ� �ֱ� ������ UILoading�� cam�� �����Ͻô� ������
                    // uiLoading �ʱ�ȭ�� �Ϸ�� �������� �� �� ����.
                    // uiLoading�� �ν��Ͻ��� UWM�� ���� ã�´�. �� �� ���� UWM���� ��ȯ�ϴ�
                    // UILoading�� null�� �ƴ϶�� uiLoading�� �ʱ�ȭ�� �Ϸ�� ���¶�� ��
                    // ��� -> uiLoading �ʱ�ȭ�� ��ٸ��� �۾�
                    uiLoading = UIWindowManager.Instance.GetWindow<UILoading>();
                    yield return null;
                }

                // �ε����� ī�޶� ��Ȱ��ȭ
                // ����? �ΰ��Ӿ��� Ȱ��ȭ�� ���·� ī�޶� �����ϱ� ������
                uiLoading.cam.enabled = false;

                loadProgress = 1f;
                #endregion

                // Ȯ�ο�..
                yield return new WaitForSeconds(.5f);

                #region �������� ��ȯ �� �ʿ��� �۾�
                if (loadCoroutine != null)
                    yield return StartCoroutine(loadCoroutine);
                #endregion

                // Ȯ�ο�..
                yield return new WaitForSeconds(1f);

                #region �������� ��ȯ �Ϸ� �� ������ �۾�
                yield return SceneManager.UnloadSceneAsync(SceneType.Loading.ToString());

                loadComplete?.Invoke();
                #endregion
            }

        }
    }
}