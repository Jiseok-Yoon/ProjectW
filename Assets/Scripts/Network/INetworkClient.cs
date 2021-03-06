using ProjectW.DB;

namespace ProjectW.Network
{
    /// <summary>
    /// 서버와 통신하는 프로토콜 메서드를 갖는 인터페이스
    /// 프로토콜이란?
    /// 서버와 클라이언트 간에 통신에 사용되는 API
    /// </summary>
    public interface INetworkClient
    {
        /// <summary>
        /// 서버에 계정 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId">
        /// 서버에 계정 정보를 요청하면서 보내는 각 계정마다 부여된 고유 아이디
        /// 임의로 디바이스의 유니크 아이디
        /// </param>
        /// <param name="responsHandler"> 
        /// 서버에 요청한 데이터를 받아 처리하는 핸들러 
        /// </param>
        void Login(int uniqueId, ResponsHandler<DtoAccount> responsHandler);

        /// <summary>
        /// 서버에 캐릭터 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId">유저마다 갖는 유니크 아이디</param>
        /// <param name="responsHandler">서버에 요청한 데이터를 받아 처리하는 핸들러</param>
        void GetCharacter(int uniqueId, ResponsHandler<DtoCharacter> responsHandler);

        /// <summary>
        /// 서버에 스테이지 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="responsHandler"></param>
        void GetStage(int uniqueId, ResponsHandler<DtoStage> responsHandler);

        /// <summary>
        /// 서버에 유저의 전체 아이템 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="responsHandler"></param>
        void GetItem(int uniqueId, ResponsHandler<DtoItem> responsHandler);
        
        /// <summary>
        /// 서버에 유저의 퀘스트 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="responsHandler"></param>
        void GetQuest(int uniqueId, ResponsHandler<DtoQuest> responsHandler);

        /// <summary>
        /// 서버에 유저의 퀘스트 db에 새로운 퀘스트 정보 추가를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="questIndex">추가할 퀘스트의 인덱스</param>
        /// <param name="responsHandler">요청 성공 시 데이터를 반환받아 처리할 핸들러</param>
        void AddQuest(int uniqueId, int questIndex, ResponsHandler<DtoQuestProgress> responsHandler);
    }
}
