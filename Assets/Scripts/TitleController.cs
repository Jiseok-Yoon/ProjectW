using ProjectW.Define;
using ProjectW.Dummy;
using ProjectW.Network;
using ProjectW.Resource;
using ProjectW.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectW
{
    /// <summary>
    /// Ÿ��Ʋ ������ ���� ���� ���� �ʿ��� �������� �ʱ�ȭ ��
    /// ������ �ε� ���� �����ϴ� Ŭ����
    /// </summary>
    public class TitleController : MonoBehaviour
    {
        /// <summary>
        /// ���� �������� �Ϸ� ����
        /// </summary>
        private bool loadComplete;

        /// <summary>
        /// �ܺο��� loadComplete �����ϱ� ���� ������Ƽ
        /// �߰��� ���� ������ �Ϸ� �� ���ǿ� ���� ���� ������� ����
        /// </summary>
        public bool LoadComplete
        {
            get => loadComplete;
            set
            {
                loadComplete = value;

                // ���� ����� �Ϸ�Ǿ���, ��� ����� �Ϸ���� �ʾҴٸ�
                if (loadComplete && !allLoaded)
                {
                    // ���� ������� ����
                    NextPhase();
                }
            }
        }

        /// <summary>
        /// ��� �������� �Ϸ� ����
        /// </summary>
        private bool allLoaded;

        /// <summary>
        /// ���� ����� ��Ÿ��
        /// </summary>
        private IntroPhase introPhase = IntroPhase.Start;

        /// <summary>
        /// �ε� ������ �ִϸ��̼� ó���� ���� �ڷ�ƾ
        /// </summary>
        private Coroutine loadGaugeUpdateCoroutine;
        public UITitle uiTitle;

        /// <summary>
        /// Ÿ��Ʋ ��Ʈ�ѷ� �ʱ�ȭ
        /// </summary>
        public void Initialize()
        {
            OnPhase(introPhase);
        }

        /// <summary>
        /// ���� ����� ���� ���� ����
        /// </summary>
        /// <param name="phase">�����Ű���� �ϴ� ���� ������</param>
        private void OnPhase(IntroPhase phase)
        {
            uiTitle.SetLoadStateDescription(phase.ToString());

            // �ε������� ui�� fillAmount�� ���� ���� �ε� ������ �ۼ�Ʈ�� ���� ������ ������ �ȵƴٸ�
            // ���� �ڷ�ƾ�� ��������..
            // �̹� �������� �ڷ�ƾ�� �� ���۽�Ű�� ������ �߻��ϹǷ�
            // �ڷ�ƾ�� �����Ѵٸ� ���� �Ŀ� ���� ����� �ε� ������ �ۼ�Ʈ�� �Ѱ� �ڷ�ƾ�� �ٽ� �����ϰ� �Ѵ�.
            if (loadGaugeUpdateCoroutine != null)
            {
                StopCoroutine(loadGaugeUpdateCoroutine);
                loadGaugeUpdateCoroutine = null;
            }

            // ����� ����� ��ü ������ �Ϸᰡ �ƴ϶��
            if (phase != IntroPhase.Complete)
            {
                // ���� �ε� �ۼ��������� ���Ѵ�.
                var loadPer = (float)phase / (float)IntroPhase.Complete;
                // ���� �ۼ��������� �ε��ٿ� ����
                loadGaugeUpdateCoroutine = StartCoroutine(uiTitle.LoadGaugeUpdate(loadPer));
            }
            else
                uiTitle.loadFillGauge.fillAmount = 1f;

            switch (phase)
            {
                case IntroPhase.Start:
                    LoadComplete = true;
                    break;
                case IntroPhase.ApplicationSetting:
                    GameManager.Instance.OnApplicationSetting();
                    LoadComplete = true;
                    break;
                case IntroPhase.Server:
                    DummyServer.Instance.Initialize();
                    ServerManager.Instance.Initialize();
                    LoadComplete = true;
                    break;
                case IntroPhase.StaticData:
                    GameManager.SD.Initialize();
                    LoadComplete = true;
                    break;
                case IntroPhase.UserData:
                    // ������ ��û�ؼ� DB���� ���� ���� �� �ʿ��� ���� ������ �޾ƿ�
                    new LoginHandler().Connect();
                    break;
                case IntroPhase.Resource:
                    ResourceManager.Instance.Initialize();
                    LoadComplete = true;
                    break;
                case IntroPhase.UI:
                    UIWindowManager.Instance.Initialize();
                    LoadComplete = true;
                    break;
                case IntroPhase.Complete:
                    var stageManager = StageManager.Instance;
                    GameManager.Instance.LoadScene(SceneType.Ingame, stageManager.ChangeStage(), stageManager.OnChangeStageComplete);
                    allLoaded = true;
                    LoadComplete = true;
                    break;
            }
        }

        /// <summary>
        /// ����� ���� ������� ����
        /// </summary>
        private void NextPhase()
        {
            StartCoroutine(WaitForSeconds());

            IEnumerator WaitForSeconds()
            {
                yield return new WaitForSeconds(.5f);

                LoadComplete = false;
                OnPhase(++introPhase);
            }
        }
    }
}