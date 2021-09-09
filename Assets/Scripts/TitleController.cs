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
    /// 타이틀 씬에서 게임 시작 전에 필요한 전반적인 초기화 및
    /// 데이터 로드 등을 수행하는 클래스
    /// </summary>
    public class TitleController : MonoBehaviour
    {
        /// <summary>
        /// 현재 페이즈의 완료 상태
        /// </summary>
        private bool loadComplete;

        /// <summary>
        /// 외부에서 loadComplete 접근하기 위한 프로퍼티
        /// 추가로 현재 페이즈 완료 시 조건에 따라 다음 페이즈로 변경
        /// </summary>
        public bool LoadComplete
        {
            get => loadComplete;
            set
            {
                loadComplete = value;

                // 현재 페이즈가 완료되었고, 모든 페이즈가 완료되지 않았다면
                if (loadComplete && !allLoaded)
                {
                    // 다음 페이즈로 변경
                    NextPhase();
                }
            }
        }

        /// <summary>
        /// 모든 페이즈의 완료 상태
        /// </summary>
        private bool allLoaded;

        /// <summary>
        /// 현재 페이즈를 나타냄
        /// </summary>
        private IntroPhase introPhase = IntroPhase.Start;

        /// <summary>
        /// 로드 게이지 애니메이션 처리에 사용될 코루틴
        /// </summary>
        private Coroutine loadGaugeUpdateCoroutine;
        public UITitle uiTitle;

        /// <summary>
        /// 타이틀 컨트롤러 초기화
        /// </summary>
        public void Initialize()
        {
            OnPhase(introPhase);
        }

        /// <summary>
        /// 현재 페이즈에 대한 로직 실행
        /// </summary>
        /// <param name="phase">진행시키고자 하는 현재 페이즈</param>
        private void OnPhase(IntroPhase phase)
        {
            uiTitle.SetLoadStateDescription(phase.ToString());

            // 로딩게이지 ui의 fillAmount가 아직 실제 로딩 게이지 퍼센트로 값이 끝까지 보간이 안됐다면
            // 아직 코루틴이 실행중임..
            // 이미 실행중인 코루틴을 또 시작시키면 오류가 발생하므로
            // 코루틴이 존재한다면 멈춘 후에 새로 변경된 로딩 게이지 퍼센트를 넘겨 코루틴을 다시 시작하게 한다.
            if (loadGaugeUpdateCoroutine != null)
            {
                StopCoroutine(loadGaugeUpdateCoroutine);
                loadGaugeUpdateCoroutine = null;
            }

            // 변경된 페이즈가 전체 페이즈 완료가 아니라면
            if (phase != IntroPhase.Complete)
            {
                // 현재 로드 퍼센테이지를 구한다.
                var loadPer = (float)phase / (float)IntroPhase.Complete;
                // 구한 퍼센테이지를 로딩바에 적용
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
                    // 서버에 요청해서 DB에서 게임 시작 시 필요한 유저 정보를 받아옴
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
        /// 페이즈를 다음 페이즈로 변경
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