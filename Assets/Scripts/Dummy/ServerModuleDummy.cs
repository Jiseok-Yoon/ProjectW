using ProjectW.DB;
using ProjectW.Network;
using ProjectW.Util;
using System;
using System.Linq;
using static ProjectW.Define.Quest;

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

        public void AddQuest(int uniqueId, int questIndex, ResponsHandler<DtoQuestProgress> responsHandler)
        {
            // 서버에서는 이런식으로 처리를 하고 있음

            // 퀘스트 인덱스를 통해 기획 퀘스트 테이블에서 해당 퀘스트 정보를 받아옴
            var sdQuest = GameManager.SD.sdQuests.Where(_ => _.index == questIndex).SingleOrDefault();

            // 클라이언트 측에 반환할 데이터를 생성
            var dtoQuestProgress = new DtoQuestProgress();

            dtoQuestProgress.index = sdQuest.index;

            // 퀘스트의 디테일정보는 현재 퀘스트의 종류에 따라 달라짐
            switch (sdQuest.type)
            {
                // 사냥, 수집 종류의 퀘스트는
                //  사냥 -> 몇 종류의 몬스터를 몇마리 잡아라
                //  수집 -> 몇 종류의 아이템을 몇개 가져와라
                 // -> 결과적으로 종류 만큼 디테일 배열의 길이를 설정
                case QuestType.Hunt:
                case QuestType.Collect:
                    Array.Resize(ref dtoQuestProgress.details, sdQuest.target.Length);
                    Array.ForEach(dtoQuestProgress.details, _ => _ = 0);
                    break;
                // 특정 지역을 탐사해라 

                // 특정 지역 도착 판단방법
                //  -> 특정 지역에 클라쪽에서 이벤트 트리거가 존재해서 해당 트리거 들어갔을 때, 판단
                //  -> or 디테일 배열에서 특정지역의 위치를 x,y,z로 갖고, 실제 유저 위치와 비교해서 클리어를 판단

                // 퀘스트 기획 테이블
                //   특정 지역 탐사의 타겟 값은 스테이지 인덱스,
                //   디테일 값은 해당 스테이지에서의 x,y,z 위치값

                // 유저 db에서의 디테일 값
                //   0이면 미도착, 1이면 도착
                case QuestType.Adventure:
                    dtoQuestProgress.details = new int[] { 0 };
                    break;
            }

            // db에 dtoQuest.progressQuests에 위에 생성한 새로운 진행 퀘스트에 대한 정보를 추가
            // dtoQuest.progressQuests가 배열로 되어있으니까 배열을 리사이징해서
            // 방금 생성한 녀석을 배열의 마지막 인덱스 공간에 넣고 있음
            var length = serverData.userData.dtoQuest.progressQuests.Length + 1;
            Array.Resize(ref serverData.userData.dtoQuest.progressQuests, length);
            serverData.userData.dtoQuest.progressQuests[length - 1] = dtoQuestProgress;

            // db 데이터를 수정했으니까 저장
            DummyServer.Instance.Save();

            // 추가된 진행 퀘스트 정보를 클라에 보내준다.
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoQuest.progressQuests[length - 1]));
        }
    }
}
