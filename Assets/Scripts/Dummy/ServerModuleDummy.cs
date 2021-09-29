using ProjectW.DB;
using ProjectW.Network;
using ProjectW.Util;

namespace ProjectW.Dummy
{
    /// <summary>
    /// 실제 더미서버에서의 통신 프로토콜 구현부를 갖는 클래스
    /// </summary>
    public class ServerModuleDummy : INetworkClient
    {
        private DummyServer serverData;

        public ServerModuleDummy(DummyServer serverData)
        {
            this.serverData = serverData;
        }

        public void Login(int uniqueId, ResponsHandler<DtoAccount> responsHandler)
        {
            // 더미서버에서는 계정정보를 요청해서 어떻게 처리할 것인가에 대해 작성
            // 더미서버이므로 실제로 클라이언트에서 클라이언트의 요청을 처리하는 것과 같음 (원맨쇼)
            // 한마디로 통신 요청에 대한 실패가 발생할 일이 일반적으로 없음..
            // 강제로 요청 성공 메서드를 실행시킴
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoAccount));
        }

        public void GetCharacter(int uniqueId, ResponsHandler<DtoCharacter> responsHandler)
        {
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoCharacter));
        }

        public void GetStage(int uniqueId, ResponsHandler<DtoStage> responsHandler)
        {
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoStage));
        }

        public void GetItem(int uniqueId, ResponsHandler<DtoItem> responsHandler)
        {
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoItem));
        }

        public void GetQuest(int uniqueId, ResponsHandler<DtoQuest> responsHandler)
        {
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoQuest));
        }
    }
}
