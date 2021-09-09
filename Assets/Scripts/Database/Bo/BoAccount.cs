using System;

namespace ProjectW.DB
{
    // 일반적으로 서버에서 받은 Dto를 Bo로 변환하여 사용
    // Dto를 직접적으로 인게임 로직에서 사용할 일은 없음
    // Bo는 인게임 로직에서만 사용되고, 통신을 하지 않으므로 직렬화할 필요가 없음
    // 하지만, 작업과정에서 데이터를 확인하기 위해 최종적인 빌드 전에는 직렬화하여 
    // 데이터를 인스펙터에 노출
#if UNITY_EDITOR   // #으로 시작하는 문법은 전부 전처리기..
                   // 전처리기는 컴파일러에 특정한 명령을 지시하기 위한 문법..
                   // 현재 작성한 코드는 #if ~ #endif 사이에 있는 코드가 유니티 에디터에서만
                   // 동작하도록하는 코드 (빌드 시에 빌드 타겟이 에디터가 아니라면 코드 자체가 제외됌)
    [Serializable]
#endif
    public class BoAccount
    {
        public string nickname;
        public int gold;

        public BoAccount(DtoAccount dtoAccount)
        {
            nickname = dtoAccount.nickname;
            gold = dtoAccount.gold;
        }
    }
}
