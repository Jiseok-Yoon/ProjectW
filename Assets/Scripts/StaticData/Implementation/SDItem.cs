using System;

namespace ProjectW.SD
{
    [Serializable]
    public class SDItem : StaticData
    {
        public string name;
        public Define.Item.ItemType itemType;
        /// <summary>
        /// 아이템 사용 시 영향을 끼치는 스텟
        /// (이 때 스텟의 이름은 실제로 캐릭터가 사용하고 있는 스텟 필드 변수명으로 들어온다..)
        /// </summary>
        public string[] affectingStats;
        /// <summary>
        /// 위쪽에서 영향을 끼치는 스텟에 적용되는 값
        /// </summary>
        public float[] affectingStatsValue;
        public string description;
        public string resourcePath;
    }
}
