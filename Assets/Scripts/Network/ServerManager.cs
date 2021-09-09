using ProjectW.Util;


namespace ProjectW.Network
{
    /// <summary>
    /// 클라이언트 내 전체적인 서버 통신을 관리하는 매니저
    /// 상황에 따라 서버 모듈을 생성하여 통신을 처리한다.
    /// 여기서 말하는 상황?
    /// 지금처럼 더미서버를 사용할 경우 더미서버 모듈을 생성,
    /// 라이브서버 사용할 경우 라이브서버 모듈을 생성
    /// </summary>
    public class ServerManager : Singleton<ServerManager>
    {
        /// <summary>
        /// 서버 통신에 필요한 포로토콜 메서드를 갖는 인터페이스
        /// 상황에 따라 더미서버, 라이브서버 모듈 등이 올 수 있음..
        /// </summary>
        private INetworkClient netClient;
        public static INetworkClient Server => Instance.netClient;

        protected override void Awake()
        {
            base.Awake();

            if (gameObject != null)
                DontDestroyOnLoad(this);
        }

        /// <summary>
        /// 서버 매니저 초기화
        /// </summary>
        public void Initialize()
        {
            // 서버 모듈 팩토리를 통해 서버모듈을 생성
            // 서버 모듈 팩토리
            // 상황에 따라 더미 서버, 라이브 서버 모듈을 생성하는 역할
            netClient = ServerModuleFactory.NewNetworkClientModule(); 
        }
    }
}
