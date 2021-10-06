/// <summary>
/// 프로젝트에서 사용되는 상수나 열거형 등을 정의
/// </summary>
namespace ProjectW.Define
{
    /// <summary>
    /// 게임에 사용되는 씬 종류
    /// </summary>
    public enum SceneType { Title, Ingame, Loading }

    /// <summary>
    /// 타이틀 씬에서 순차적으로 수행할 작업을 열거
    /// </summary>
    public enum IntroPhase { Start, ApplicationSetting, Server, StaticData,
    UserData, Resource, UI, Complete }

    public enum PoolType { Character, Monster, Projectile, Item, HpBar, QuestSlot, DialogueButton }

    public class Camera
    {
        public enum CamView { Standard, Front }
        public const float CamRotSpeed = 3f;
        public const string CamPosPath = "Prefabs/CamPos";
    }

    public class Input
    {
        public const string AxisX = "Horizontal";
        public const string AxisZ = "Vertical";
        public const string MouseX = "Mouse X";
        public const string MouseY = "Mouse Y";
        public const string FrontCam = "Fire3";
        public const string Jump = "Jump";
        public const string MouseLeft = "Fire1";
        public const string MouseRight = "Fire2";
    }

    public class Spawn
    {
        public const int MinMonsterSpawnCnt = 1;
        public const int MaxMonsterSpawnCnt = 5;
        public const float MinMonsterSpawnTime = 10f;
        public const float MaxMonsterSpawnTime = 20f;
    }

    public class Actor
    {
        /// <summary>
        /// 액터의 타입
        /// </summary>
        public enum ActorType { Character, Monster }
        /// <summary>
        /// 액터의 상태
        /// </summary>
        public enum ActorState { Idle, Walk, Sit, Rise, Jump, Attack , Dead }
        /// <summary>
        /// 액터가 사용하는 애니메이터 파라미터
        /// </summary>
        public enum ActorAnim { isWalk, isJump, isSit, isRise, isAttack, isDead }
        /// <summary>
        /// 액터의 일반 공격 타입
        /// </summary>
        public enum AttackType { Normal, Projectile }
    }

    public class Monster
    {
        public const float MinPatrolWaitTime = 1f;
        public const float MaxPatrolWaitTime = 3f;
    }

    public class StaticData
    {
        public const string SDPath = "Assets/StaticData";
        public const string SDExcelPath = "Assets/StaticData/Excel";
        public const string SDJsonPath = "Assets/StaticData/Json";
    }

    public class Resource
    {
        public enum AtlasType { ItemAtlas, IconAtlas }
    }

    public class Item
    {
        public enum ItemType { Equipment, Expendables, Quest, Etc }
    }

    public class NPC
    {
        public enum NPCType { None,  }
    }

    public class Dialogue
    {
        public enum DialogueButtonType { Shop, Quest }
    }

    public class Quest
    {
        public enum QuestType { Hunt, Collect, Adventure, }
        public enum QuestWindow { List, Order }
    }
}
