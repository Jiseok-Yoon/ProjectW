using ProjectW.Battle;
using ProjectW.DB;
using ProjectW.Define;
using ProjectW.Object;
using ProjectW.Resource;
using ProjectW.UI;
using ProjectW.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace ProjectW
{
    using Monster = Object.Monster;

    /// <summary>
    /// 스테이지 관련 기능들을 수행하는 클래스
    /// 스테이지 전환 시 처리작업 (해당 스테이지에 필요한 리소스 로드 및 인스턴스 생성)
    /// </summary>
    public class StageManager : Singleton<StageManager>
    {
        private bool isReady;

        /// <summary>
        /// 몬스터 스폰 시간 (현재 업데이트 시간)
        /// </summary>
        private float currentSpawnTime;
        /// <summary>
        /// 몬스터 스폰 시간 (체크 업데이트 시간)
        /// </summary>
        private float maxSpawnTime;

        private Transform monsterHolder;

        /// <summary>
        /// 현재 스테이지 인스턴스 객체
        /// </summary>
        private GameObject currentStage;
        /// <summary>
        /// 스테이지 내에서 몬스터 스폰영역에 대한 정보를 들고 있는 딕셔너리
        /// </summary>
        private Dictionary<int, Bounds> spawnAreaBounds = new Dictionary<int, Bounds>();

        private void Update()
        {
            if (!isReady)
                return;

            CheckSpawnTime();
        }

        /// <summary>
        /// 스테이지 전환 시 필요한 리소스를 불러오고 인스턴스 생성 및 데이터 바인딩 작업
        /// 이 메서드를 호출하는 시점은 로딩 씬이 활성화되어있는 상태..
        /// </summary>
        /// <returns></returns>
        public IEnumerator ChangeStage()
        {
            isReady = false;

            // 외부(서버)에서 새로 불러올 스테이지 정보를 받은 상태
            // 한마디로 아래의 sdStage는 새로 로드할 스테이지에 대한 기획 정보
            var sdStage = GameManager.User.boStage.sdStage;

            var resourceManager = ResourceManager.Instance;

            // 현재 스테이지 객체가 존재하는지
            if (currentStage != null)
                // 존재한다면 새로운 스테이지를 로드할 것이므로 파괴
                Destroy(currentStage);

            // 새로 불러올 스테이지 인스턴스를 생성하여 현재 스테이지 필드에 대입
            currentStage = Instantiate(resourceManager.LoadObject(sdStage.resourcePath));

            // 현재 ChangeStage 메서드가 호출되고 있는 시점은 씬이 2개인 상태 (로딩, 인게임)
            // 이 때 객체를 생성하면 활성화되어있는 씬에 객체가 생성됌 (로딩씬이 활성화, 인게임은 비활성화)
            // 그럼 현재 활성화 되어있는 로딩씬에 생성된 객체가 귀속되기 때문에 객체를 로딩씬에서 인게임씬으로 이동
            SceneManager.MoveGameObjectToScene(currentStage, SceneManager.GetSceneByName(SceneType.Ingame.ToString()));
            // 스테이지는 로딩씬에서 미리 만들어두고
            // 그 외 몬스터나 캐릭터 (액터)들은 씬이 완전히 전환된 후에 생성

            // 이전 스테이지에서 사용하던 정보들을 비우는 작업
            spawnAreaBounds.Clear(); 
            ObjectPoolManager.Instance.ClearPool<Monster>(PoolType.Monster);
            BattleManager.Instance.Monsters.Clear();

            // 현재 스테이지에서 사용될 리소스를 부르고 인스턴스를 생성하는 작업
            var sd = GameManager.SD;

            // 바뀐 스테이지에서 사용되는 몬스터의 종류만큼 반복
            for (int i = 0; i < sdStage.genMonsters.Length; ++i)
            {
                // 몬스터 기획 데이터를 하나씩 불러온다.
                var sdMonster = sd.sdMonsters.Where(_ => _.index == sdStage.genMonsters[i]).SingleOrDefault();

                if (sdMonster != null)
                {
                    resourceManager.LoadPoolableObject<Monster>(PoolType.Monster, sdMonster.resourcePath, 10);
                }
                else
                    continue;

                // 해당 몬스터의 스폰 구역에 대한 정보를 가져온다.
                var spawnAreaIndex = sdStage.spawnArea[i];
                if (spawnAreaIndex != -1)
                {
                    // 해당 스폰영역 인덱스가 딕셔너리에 존재하는지 체크 (중복되는 영역을 갖는 몬스터가 있을 수 있으므로)
                    if (!spawnAreaBounds.ContainsKey(spawnAreaIndex))
                    {
                        // 존재하지 않는다면 등록
                        var spawnArea = currentStage.transform.Find("SpawnPosHolder").GetChild(spawnAreaIndex);
                        spawnAreaBounds.Add(spawnAreaIndex, spawnArea.GetComponent<Collider>().bounds);
                    }
                }
            }

            yield return null;
        }

        /// <summary>
        /// 위의 ChangeStage 메서드가 씬 전환 도중에 실행되는 작업이라면
        /// OnChangeStageComplete은 씬 전환이 완료된 후에 실행될 작업
        /// </summary>
        public void OnChangeStageComplete()
        {
            UIWindowManager.Instance.GetWindow<UIBattle>().Clear();

            ClearSpawnTime();
            SpawnNPC();
            SpawnCharacter();
            SpawnMonster();

            isReady = true;
        }

        /// <summary>
        /// 플레이어의 캐릭터 생성 또는 스테이지 이동 시 플레이어 위치 설정
        /// </summary>
        private void SpawnCharacter()
        {
            var playerController = FindObjectOfType<PlayerController>();
            if (playerController == null)
                return;

            // 플레이어의 캐릭터 인스턴스가 있다면, 타이틀 -> 인게임 씬 변경이 아닌 스테이지 이동했다는 것
            if (playerController.PlayerCharacter != null)
            {
                // 새로 이동한 스테이지에 이전 스테이지와 연결된 워프의 EntryPos를 찾음
                var warpEntry = currentStage.transform.Find($"WarpPosHolder/{GameManager.User.boStage.prevStageIndex}/EntryPos").transform;

                // 플레이어를 해당 워프 위치로 변경
                playerController.PlayerCharacter.transform.position = warpEntry.position;
                playerController.PlayerCharacter.transform.forward = warpEntry.forward;
                // 플레이어가 워프로 인해 갑작스럽게 이동하였으므로 카메라도 동일하게 강제로 이동시켜준다.
                playerController.cameraController.SetForceStandardView();
                return;
            }

            // 캐릭터 인스턴스가 없다면, 타이틀 -> 인게임 씬 변경이 일어났다는 것, 이 때 캐릭터가 없으므로 생성
            var characterObj = Instantiate(ResourceManager.Instance.LoadObject(GameManager.User.boCharacter.sdCharacter.resourcePath));
            // 유저의 캐릭터가 전에 위치했던 곳의 값을 적용 
            characterObj.transform.position = GameManager.User.boStage.prevPos;

            var playerCharacter = characterObj.GetComponent<Character>();
            // 생성한 캐릭터 객체가 갖는 캐릭터 컴포넌트에 유저의 캐릭터 정보를 설정해준다.
            playerCharacter.Initialize(GameManager.User.boCharacter);

            // 설정이 끝난 캐릭터 객체를 유저가 제어할 수 있게 플레이어 컨트롤러에 등록
            playerController.Initialize(playerCharacter);

            // 모든 초기화가 끝난 캐릭터 객체를 정상적으로 업데이트할 수 있게 배틀매니저에 등록한다.
            BattleManager.Instance.AddActor(playerCharacter);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SpawnNPC()
        {
            // 현재 스테이지 정보를 참조하여
            // NPC 테이블에 접근해서 현재 스테이지에 존재하는 NPC 들의 정보를 받아온다.



            // 받아온 NPC 들 중에 유저 DB에서 해당 유저가 해금한 NPC인지 확인하여
            // 해금된 NPC라면 생성한다.
        }

        /// <summary>
        /// 몬스터 스폰 시간을 체크하는 기능
        /// </summary>
        private void CheckSpawnTime()
        {
            if (currentStage == null)
                return;

            currentSpawnTime += Time.deltaTime;
            if (currentSpawnTime >= maxSpawnTime)
            {
                ClearSpawnTime();
                SpawnMonster();
            }
        }

        /// <summary>
        /// 몬스터 스폰시간 초기화
        /// </summary>
        private void ClearSpawnTime()
        {
            currentSpawnTime = 0;
            maxSpawnTime = Random.Range(Spawn.MinMonsterSpawnTime, Spawn.MaxMonsterSpawnTime);
        }

        /// <summary>
        /// 몬스터를 생성하는 기능
        /// </summary>
        private void SpawnMonster()
        { 
            if (monsterHolder == null)
            {
                monsterHolder = new GameObject("MonsterHolder").transform;
                monsterHolder.position = Vector3.zero;
            }

            var sd = GameManager.SD;
            var sdStage = GameManager.User.boStage.sdStage;

            var monsterSpawnCnt = Random.Range(Spawn.MinMonsterSpawnCnt, Spawn.MaxMonsterSpawnCnt);
            var monsterPool = ObjectPoolManager.Instance.GetPool<Monster>(PoolType.Monster);

            var battleManager = BattleManager.Instance;

            for (int i = 0; i < monsterSpawnCnt; ++i)
            {
                // 현재 스테이지에서 생성할 수 있는 몬스터 중에 랜덤하게 생성
                // 생성할 수 있는 몬스터의 기획데이터 배열 상의 인덱스를 가져온다.
                var randIndex = Random.Range(0, sdStage.genMonsters.Length);
                // 랜덤하게 생성할 몬스터의 기획 데이터상의 인덱스
                var genMonsterIndex = sdStage.genMonsters[randIndex];
                if (genMonsterIndex == -1)
                    return;

                // 생성할 몬스터의 기획 데이터를 가져옴
                var sdMonster = sd.sdMonsters.Where(_ => _.index == genMonsterIndex).SingleOrDefault();
                // 몬스터 풀에는 여러 종류의 몬스터가 있기 때문에, 내가 생성할 몬스터 객체와 동일한 종류의 객체를 찾아야함
                // 반복문에서 새로운 문자열을 생성하는 방식이 좋지 않기 때문에, 나중에 바꾸는거 추천
                var monsterName = sdMonster.resourcePath.Remove(0, sdMonster.resourcePath.LastIndexOf('/') + 1);

                // 풀에서 몬스터를 가져옴
                var monster = monsterPool.GetPoolableObject(_ => _.name == monsterName);
                if (monster == null)
                    continue;

                // 몬스터의 위치를 설정 (스폰 영역 내에서)
                var bounds = spawnAreaBounds[sdStage.spawnArea[randIndex]];
                var spawnPosX = Random.Range(-bounds.size.x * .5f, bounds.size.x * .5f);
                var spawnPosZ = Random.Range(-bounds.size.z * .5f, bounds.size.z * .5f);

                monster.transform.position = bounds.center + new Vector3(spawnPosX, 0, spawnPosZ);
                monster.transform.parent = monsterHolder;
                monster.Initialize(new BoMonster(sdMonster));
                battleManager.AddActor(monster);
            }
        }

        /// <summary>
        /// 몬스터 인덱스르 받아 해당 몬스터의 스폰 구역을 찾아
        /// 해당 스폰 구역 내에서 랜덤한 위치를 반환
        /// </summary>
        /// <param name="monsterIndex"></param>
        /// <returns></returns>
        public Vector3 GetRandPosInArea(int monsterIndex)
        {
            // 현재 스테이지 정보
            var sdStage = GameManager.User.boStage.sdStage;

            // 현재 스테이지 정보에서 해당 스테이지가 스폰할 수 있는 몬스터 정보에 접근
            // 기획데이터 상에서 스폰할 수 있는 몬스터 정보는 배열형태로 되어있음..
            // 동일하게 스폰구역을 나타내는 정보도 몬스터 정보와 동일한 길이로 배열형태로 되어있음
            // 즉, 기획데이터 상의 배열 인덱스를 구하면 스폰 구역 데이터에 접근할 수 있다는 의미
            var arrayIndex = -1;

            for (int i = 0; i < sdStage.genMonsters.Length; ++i)
            {
                if (sdStage.genMonsters[i] == monsterIndex)
                {
                    arrayIndex = i;
                    break;
                }
            }

            var bounds = spawnAreaBounds[sdStage.spawnArea[arrayIndex]];
            var spawnPosX = Random.Range(-bounds.size.x * .5f, bounds.size.x * .5f);
            var spawnPosZ = Random.Range(-bounds.size.z * .5f, bounds.size.z * .5f);

            return bounds.center + new Vector3(spawnPosX, 0, spawnPosZ);
        }
    }
}
