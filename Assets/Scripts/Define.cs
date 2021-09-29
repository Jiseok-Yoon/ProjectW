/// <summary>
/// ������Ʈ���� ���Ǵ� ����� ������ ���� ����
/// </summary>
namespace ProjectW.Define
{
    /// <summary>
    /// ���ӿ� ���Ǵ� �� ����
    /// </summary>
    public enum SceneType { Title, Ingame, Loading }

    /// <summary>
    /// Ÿ��Ʋ ������ ���������� ������ �۾��� ����
    /// </summary>
    public enum IntroPhase { Start, ApplicationSetting, Server, StaticData,
    UserData, Resource, UI, Complete }

    public enum PoolType { Character, Monster, Projectile, Item, HpBar }

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
        /// ������ Ÿ��
        /// </summary>
        public enum ActorType { Character, Monster }
        /// <summary>
        /// ������ ����
        /// </summary>
        public enum ActorState { Idle, Walk, Sit, Rise, Jump, Attack , Dead }
        /// <summary>
        /// ���Ͱ� ����ϴ� �ִϸ����� �Ķ����
        /// </summary>
        public enum ActorAnim { isWalk, isJump, isSit, isRise, isAttack, isDead }
        /// <summary>
        /// ������ �Ϲ� ���� Ÿ��
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
        public enum AtlasType { ItemAtlas }
    }

    public class Item
    {
        public enum ItemType { Equipment, Expendables, Quest, Etc }
    }

    public class NPC
    {
        public enum NPCType { None, }
    }

    public class Quest
    {
        public enum QuestType { Hunt, Collect, Adventure, }
    }
}
