using ProjectW.DB;
using UnityEngine;

namespace ProjectW.Dummy
{
    // ScriptableObject란?
    // 유니티에서 지원하는 데이터 또는 정적 메서드(툴 같은 기능)만을 갖는 클래스
    // 인스턴스가 불가능함
    // 현재 DB가 없으므로 해당 클래스가 DB라고 생각하고 사용하면 됌

    [CreateAssetMenu(menuName = "ProjectW/UserData")]
    public class UserDataSo : ScriptableObject
    {
        public DtoAccount dtoAccount;
        public DtoStage dtoStage;
        public DtoCharacter dtoCharacter;
        public DtoItem dtoItem;
        public DtoQuest dtoQuest;
    }
}
