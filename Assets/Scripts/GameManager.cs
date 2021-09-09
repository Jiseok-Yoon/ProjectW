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
    /// 게임에 사용하는 모든 데이터를 관리하는 클래스
    /// 추가로 게임의 씬 변경 등과 같은 큰 흐름등을 컨트롤하기도 함
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        /// <summary>
        /// 해당 필드가 true라면 더미서버를 사용
        /// </summary>
        public bool useDummyServer;

        public float loadProgress;

        /// <summary>
        /// 유저 데이터 (DB 데이터)
        /// </summary>
        [SerializeField]
        private BoUser boUser = new BoUser();
        public static BoUser User => Instance.boUser;

        /// <summary>
        /// 기획 데이터
        /// </summary>
        // private 필드를 인스펙터에 노출시키게
        [SerializeField]
        private StaticDataModule sd = new StaticDataModule();
        public static StaticDataModule SD => Instance.sd;

        protected override void Awake()
        {
            base.Awake();

            if (gameObject == null)
                return;

            // 씬이 변경되도 객체가 파괴되지 않게
            DontDestroyOnLoad(this);

            var titleController = FindObjectOfType<TitleController>();
            titleController?.Initialize();
            // if (titleController != null)
            // {
            //      titleController.Intialize();
            // }
        }

        /// <summary>
        /// 앱에 기본 설정
        /// </summary>
        public void OnApplicationSetting()
        {
            // 수직동기화 끄기
            QualitySettings.vSyncCount = 0;
            // 렌더 프레임을 60으로 설정
            Application.targetFrameRate = 60;
            // 앱 실행 중 장시간 대기 시에도 화면이 꺼지지 않게
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        /// <summary>
        /// 씬을 비동기로 로드하는 기능
        /// 다른 씬 간의 전환에 사용 (ex: Title -> InGame)
        /// </summary>
        /// <param name="sceneName">로드할 씬의 이름을 갖는 열거형</param>
        /// <param name="loadCoroutien">씬 전환 시 로딩 씬에서 미리 처리할 작업</param>
        /// <param name="loadComplete">씬 전환 완료 후 실행할 기능</param>
        public void LoadScene(SceneType sceneName, IEnumerator loadCoroutine = null, Action loadComplete = null)
        {
            StartCoroutine(WaitForLoad());

            // 씬을 전환할 때 ex) Title -> Ingame 전환을 할 때 한 번에 전환하는 것이 아니라
            // 중간에 로딩 씬을 이용, 최종적으로 Title -> Loading -> Ingame

            // 코루틴 -> 유니티에서 특정 작업을 비동기로 실행할 수 있게 해주는 기능
            //           (비동기처럼 실행이 되는데.. 실제로 비동기는 아님)

            // LoadScene 메서드에서만 사용가능한 로컬함수 선언
            IEnumerator WaitForLoad()
            {
                // 로딩 진행상태를 나타냄 (0~1)
                loadProgress = 0;

                // 비동기로 로딩 씬으로 전환 (비동기로 사용하는 이유는 씬 전환시 화면이 멈추지 않게 하기 위해서)
                yield return SceneManager.LoadSceneAsync(SceneType.Loading.ToString());

                // 로딩 씬으로 전환 완료 후에 아래 로직이 들어옴

                // 내가 변경하고자하는 씬을 추가
                var asyncOper = SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);
                // 방금 추가한 씬을 비활성화
                // 이유? 2개의 씬이 활성화되어있다면 따로 컨트롤하지 않는 이상 어떤 씬이 화면에 보일지 모름
                // 실제 변경하고자 하는 씬을 추가해둔 다음 사용자에게는 로딩 씬을 보여주고
                // 실제 변경하고자 하는 씬에 필요한 리소스등을 부르는 작업을 하기 위해서
                asyncOper.allowSceneActivation = false;

                // 변경하고자 하는 씬에 필요한 작업이 존재한다면 실행
                if (loadCoroutine != null)
                {
                    // 해당 작업이 완료될 때 까지 대기
                    yield return StartCoroutine(loadCoroutine);
                }

                // 위에 작업이 완료된 후에 아래 로직이 실행됌


                // 비동기로 로드한 씬이 활성화가 완료되지 않았다면 특정 작업을 반복
                while (!asyncOper.isDone)
                {
                    // loadProgress 값을 이용해서 사용자한테 로딩바를 통해 진행 상태를 알려줌

                    if (loadProgress >= .9f)
                    {
                        loadProgress = 1f;

                        // 로딩바가 마지막까지 차는 것을 확인하기 위해 1초 정도 대기
                        yield return new WaitForSeconds(1f);

                        // 변경하고자 하는 씬을 다시 활성화
                        // (isDone은 씬이 활성상태가 아니라면 progress가 1이 되어도 true가 안됌)
                        asyncOper.allowSceneActivation = true;
                    }
                    else
                        loadProgress = asyncOper.progress;

                    // 코루틴 내에서 반복문 사용 시 로직을 한 번 실행 후, 메인 로직을 실행 할 수 있게 yield return
                    yield return null;
                }

                // 위의 반복 작업이 다 끝난 후 아래 로직 실행

                // 로딩 씬에서 다음 씬에 필요한 작업을 전부 수행했으므로 로딩씬을 비활성화 시킴
                yield return SceneManager.UnloadSceneAsync(SceneType.Loading.ToString());

                // 모든 작업이 완료되었으므로 모든 작업 완료 후 실행시킬 로직이 있다면 실행
                loadComplete?.Invoke();
            }
        }

        /// <summary>
        /// 인게임 씬에서 스테이지 전환 시 사용 (실제 씬을 변경하는 것이 아닌, 로딩씬을 이용하여 씬 전환처럼 보이게 만듬)
        /// 로딩 씬을 이용하여 변경하고자 하는 스테이지에 필요한 리소스 로드나 초기화 작업등을 처리
        /// ex) 인게임씬(시작마을) -> 인게임씬(초보자사냥터)
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

                #region 로딩바 진행상태 처리
                while (!asyncOper.isDone)
                {
                    loadProgress = asyncOper.progress;
                    yield return null;
                }

                // uiLoading 인스턴스의 참조를 찾음
                UILoading uiLoading = null;
                while (uiLoading == null)
                {
                    // 로딩씬을 비동기로 로드하고 있기 떄문에 UILoading에 cam에 접근하시는 시점이
                    // uiLoading 초기화가 완료된 시점인지 알 수 없다.
                    // uiLoading의 인스턴스를 UWM을 통해 찾는다. 이 때 만약 UWM에서 반환하는
                    // UILoading이 null이 아니라면 uiLoading의 초기화가 완료된 상태라는 것
                    // 결론 -> uiLoading 초기화를 기다리는 작업
                    uiLoading = UIWindowManager.Instance.GetWindow<UILoading>();
                    yield return null;
                }

                // 로딩씬을 카메라를 비활성화
                // 이유? 인게임씬도 활성화된 상태로 카메라가 존재하기 때문에
                uiLoading.cam.enabled = false;

                loadProgress = 1f;
                #endregion

                // 확인용..
                yield return new WaitForSeconds(.5f);

                #region 스테이지 전환 시 필요한 작업
                if (loadCoroutine != null)
                    yield return StartCoroutine(loadCoroutine);
                #endregion

                // 확인용..
                yield return new WaitForSeconds(1f);

                #region 스테이지 전환 완료 후 실행할 작업
                yield return SceneManager.UnloadSceneAsync(SceneType.Loading.ToString());

                loadComplete?.Invoke();
                #endregion
            }

        }
    }
}