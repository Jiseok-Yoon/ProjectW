using ProjectW.SD;
using System;
using System.Linq;


namespace ProjectW.DB
{
    [Serializable]
    public class BoCharacter : BoActor
    {
        /// <summary>
        /// 캐릭터 기본 정보 및 레벨에 영향을 받지 않는 스텟
        /// </summary>
        public SDCharacter sdCharacter;

        /// <summary>
        /// 레벨에 영향을 받는 스텟
        /// </summary>
        public SDGrowthStat sdGrowthStat;

        public BoCharacter(DtoCharacter dtoCharacter)
        {
            // 지금 캐릭터의 기획 데이터는 어디있는가?
            // GameManager의 SD에 가지고 있음
            var sd = GameManager.SD;

            // 캐릭터 테이블에서 유저의 캐릭터 인덱스에 해당하는 캐릭터 정보를 찾아
            // 참조를 가져온다
            //for (int i = 0; i < characterTable.Count; ++i)
            //{
            //    if (characterTable[i].index == dtoCharacter.index)
            //    {
            //        sdCharacter = characterTable[i];
            //        break;
            //    }
            //}

            // Linq 를 이용해서 코드 생산성을 높일 수 있다.. Linq 나중에 따로 한번 쭉 검색해서 읽어보시길..
            sdCharacter = sd.sdCharacters.Where(_ => _.index == dtoCharacter.index).SingleOrDefault();

            // 성장스텟 테이블에서 내 캐릭터가 참조하는 성장스텟 인덱스 값과 동일한 인덱스를 가진 원소가 있다면 가져온다
            sdGrowthStat = sd.sdGrowthStats.Where(_ => _.index == sdCharacter.growthStatRef).SingleOrDefault();

            level = dtoCharacter.level;
        }
    }
}
