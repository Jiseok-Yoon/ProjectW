using ProjectW.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ProjectW.SD
{
    /// <summary>
    /// 모든 기획 데이터를 들고 있는 클래스
    /// 데이터를 로드하고 들고 있기만 할 것이므로 모노를 상속받을 필요가 없음
    /// </summary>
    // 모노를 갖지 않는 일반 C# 클래스를 인스펙터에 노출시키기 위해 직렬화
    [Serializable]
    public class StaticDataModule
    {
        public List<SDCharacter> sdCharacters;
        public List<SDStage> sdStages;
        public List<SDGrowthStat> sdGrowthStats;
        public List<SDMonster> sdMonsters;
        public List<SDItem> sdItems;
        public List<SDNpc> sdNpc;
        public List<SDQuest> sdQuest;
        public List<SDString> sdString;

        public void Initialize()
        {
            var loader = new StaticDataLoader();

            loader.Load(out sdCharacters);
            loader.Load(out sdStages);
            loader.Load(out sdGrowthStats);
            loader.Load(out sdMonsters);
            loader.Load(out sdItems);
            loader.Load(out sdNpc);
            loader.Load(out sdQuest);
            loader.Load(out sdString);
        }

        /// <summary>
        /// 기획데이터를 불러올 로더
        /// </summary>
        private class StaticDataLoader
        {
            private string path;

            public StaticDataLoader()
            {
                path = $"{Application.dataPath}/StaticData/Json";
            }

            public void Load<T>(out List<T> data) where T : StaticData
            {
                // 파일이름이 타입이름에서 SD만 제거하면 동일하다는 규칙이 있음..
                var fileName = typeof(T).Name.Remove(0, "SD".Length);

                var json = File.ReadAllText($"{path}/{fileName}.json");

                data = SerializationUtil.FromJson<T>(json);
            }
        }
    }
}
