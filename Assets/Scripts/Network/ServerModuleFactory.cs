using ProjectW.Dummy;

namespace ProjectW.Network
{
    /// <summary>
    /// 특정 조건에 따라 서버모듈을 생성할 팩토리
    /// </summary>
    public static class ServerModuleFactory
    {
        /// <summary>
        /// 조건에 따라 서버모듈을 생성 후 해당 모듈에 구현된 프로토콜을 갖는 인터페이스를 반환한다.
        /// </summary>
        /// <returns></returns>
        public static INetworkClient NewNetworkClientModule()
        {
            // 더미 서버를 사용하고, 더미서버 인스턴스가 존재한다면
            if (GameManager.Instance.useDummyServer && DummyServer.Instance != null)
            {
                // 더미서버 모듈을 반환
                return DummyServer.Instance.dummyModule;
            }
            // 더미서버가 아니라면 테스트 or 라이브 서버 등...
            else
            { 
            
            }

            return null;
        }
    }
}
