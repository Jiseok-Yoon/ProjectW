using ProjectW.DB;
using ProjectW.UI;
using ProjectW.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectW.Object
{
    public class Monster : Actor, IPoolableObject
    {
        private float currentPatrolWaitTime; // 현재 정찰대기시간
        private float patrolWaitTime; // 정찰대기시간 체크 지점
        private Vector3 destPos; // 목적지 위치 (정찰 지점, 타겟 지점)

        public BoMonster boMonster;

        private NavMeshAgent agent;
        private NavMeshPath path;

        public bool CanRecycle { get; set; } = true;

        public override void Initialize(BoActor boActor)
        {
            base.Initialize(boActor);

            boMonster = boActor as BoMonster;

            SetStats();
            InitPatrolWaitTime();

            // 목적지를 몬스터의 현재 위치로 설정하여 바로 목적지 도착 상태로 인식하게 하여
            // 몬스터의 새로운 목적지를 설정할 수 있게 하기 위해서
            destPos = transform.position;
        }

        protected override void Start()
        {
            base.Start();

            agent = GetComponent<NavMeshAgent>();
            path = new NavMeshPath();
        }

        public override void SetStats()
        {
            if (boMonster == null)
                return;

            boMonster.level = 1;
            boMonster.actorType = Define.Actor.ActorType.Monster;
            boMonster.atkType = boMonster.sdMonster.atkType;
            boMonster.moveSpeed = boMonster.sdMonster.moveSpeed;
            boMonster.currentHp = boMonster.maxHp = boMonster.sdMonster.maxHp;
            boMonster.currentMana = boMonster.maxMana = boMonster.sdMonster.maxMana;
            boMonster.atkRange = boMonster.sdMonster.atkRange;
            boMonster.atkInterval = boMonster.sdMonster.atkInterval;
            boMonster.atk = boMonster.sdMonster.atk;
            boMonster.def = boMonster.sdMonster.def;
        }

        public override void ActorUpdate()
        {
            CheckDetection();

            base.ActorUpdate();
        }

        public override void MoveUpdate()
        {
            var isMove = GetMovement();

            if (isMove)
            {
                SetState(Define.Actor.ActorState.Walk);

                // 속력 설정
                agent.speed = boMonster.moveSpeed;
                // 목적지 설정
                agent.SetDestination(destPos);
            }
            else
            {
                SetState(Define.Actor.ActorState.Idle);
            }
        }

        /// <summary>
        /// 상황에 따라 움직임을 설정하고 움직임 여부를 반환
        /// </summary>
        /// <returns></returns>
        private bool GetMovement()
        {
            // 타겟에 대한 정보를 가지고 있는지?
            if (attackController.hasTarget)
            {
                // 공격 범위 내에 캐릭터가 있으면 true, 아니라면 false
                // !canAtk은 공격이 가능하면 움직임 여부를 false, 공격 불가능한 상태라면 true
                return !attackController.canAtk;
            }

            // 상태가 대기상태라면
            if (State == Define.Actor.ActorState.Idle)
            {
                currentPatrolWaitTime += Time.deltaTime;
                // 대기 시간이 끝났다면
                if (currentPatrolWaitTime >= patrolWaitTime)
                {
                    // 대기시간 초기화
                    InitPatrolWaitTime();
                    // 이동할 수 있게 true 반환
                    return true;
                }

                // 대기시간이 지나지 않았으므로 이동할 수 없게 false 반환
                return false;
            }

            var distance = (destPos - transform.position).magnitude;

            // 정찰 위치에 도착했다면 정찰 위치를 변경
            // false 반환시켜서 바로 이동하는 것이 아닌 몇 초간 대기 후 이동하게
            if (distance < agent.stoppingDistance)
            {
                ChangeDestPos();

                return false;
            }

            return true;
        }

        /// <summary>
        /// 정찰 위치를 변경하는 기능
        /// </summary>
        private void ChangeDestPos()
        {
            // 몬스터마다 스폰 구역이 다르므로 몬스터의 인덱스 값을 넘겨 해당 몬스터의
            // 스폰 구역 내에서 랜덤한 위치를 반환받는다.
            destPos = StageManager.Instance.GetRandPosInArea(boMonster.sdMonster.index);

            var isExist = agent.CalculatePath(destPos, path);

            // 해당 위치가 네비메쉬에 존재하는지 확인
            if (!isExist)
            {
                // 재귀 호출을 통해 목적지를 새로 지정하고 다시 검사.
                ChangeDestPos();
            }
            // 해당 위치가 네비메쉬에 존재하지만, 목적지에 도착할 수 없는 경우
            else if (path.status == NavMeshPathStatus.PathPartial)
            {
                ChangeDestPos();
            }
        }

        /// <summary>
        /// 정찰 대기 시간 초기화
        /// </summary>
        private void InitPatrolWaitTime()
        {
            currentPatrolWaitTime = 0;
            patrolWaitTime = Random.Range(Define.Monster.MinPatrolWaitTime, Define.Monster.MaxPatrolWaitTime);
        }

        /// <summary>
        /// 감지 범위 내 적(플레이어)이 있는지 체크
        /// </summary>
        private void CheckDetection()
        {
            var extentsValue = boMonster.sdMonster.detectionRange;
            var halfExtents = new Vector3(extentsValue, extentsValue, extentsValue);

            var colls = Physics.OverlapBox(transform.position, halfExtents, transform.rotation,
                1 << LayerMask.NameToLayer("Player"));

            // 감지 범위 내 플레이어가 없다면
            if (colls == null || colls.Length == 0)
            {
                attackController.hasTarget = false;
                return;
            }

            // 감지 범위 내 플레이어를 발견
            attackController.hasTarget = true;
            // 플레이어를 추적할 수 있도록 목적지를 플레이어 위치로 변경
            destPos = colls[0].transform.position;

            // 플레이어와의 거리가 공격 범위 안이라면 공격가능 상태로 변경
            var distance = destPos - transform.position;
            // 플레이어와 거리가 공격범위 안이라면
            if (distance.sqrMagnitude <= boActor.atkRange)
            {
                attackController.canAtk = true;
                // 공격 가능상태가 되면 이동보다 공격이 우선시 되어 이동이나 회전을 하지 않는다.
                // 이 때, 캐릭터가 이동 시 몬스터가 캐릭터를 바라볼 수 있게 회전값을 변경
                transform.rotation = Quaternion.LookRotation(distance.normalized);
            }
            else
            {
                attackController.canAtk = false;
            }
        }

        /// <summary>
        /// 몬스터가 죽는 애니메이션이 종료됐을 때, 풀에 반환
        /// </summary>
        public override void OnDeadEnd()
        {
            // 아이템 풀을 일단 받아옴
            var itemPool = ObjectPoolManager.Instance.GetPool<Item>(Define.PoolType.Item);
            // 월드 UI 캔버스도 일단 받아옴
            var worldUICanvas = UIWindowManager.Instance.GetWindow<UIBattle>().worldUICanvas;

            // 몬스터 기획 데이터의 dropItemPer 반복문으로 접근해서
            // 드롭확률 계산 후에, 드롭이라면해당 인덱스의 아이템을 생성 (아이템 풀에서 아이템 객체 꺼내서 데이터를 채움)

            // 몬스터가 드롭할 수 있는 아이템 종류만큼 반복
            for (int i = 0; i < boMonster.sdMonster.dropItemRef.Length; ++i)
            {
                // 아이템 드롭 확률 계산
                var isDrop = boMonster.sdMonster.dropItemPer[i] <= Random.Range(0, 1f);

                // 드롭이 되지 않았다면 다음 아이템 드롭 확률 계산으로 넘어감
                if (!isDrop)
                    continue;

                // 드롭했으니 아이템 풀에서 아이템 객체를 하나 가져옴
                var itemObj = itemPool.GetPoolableObject();

                // 아이템 객체의 부모를 월드 캔버스로 설정
                itemObj.transform.SetParent(worldUICanvas.transform);
                // 월드 캔버스로 부모가 변경되면서 스케일링이 발생하므로 정상적인 스케일로 되돌림
                itemObj.transform.localScale = Vector3.one * .5f;
                // 위치를 몬스터의 위치로 설정
                itemObj.transform.position = transform.position + Vector3.up * .25f;
                itemObj.gameObject.SetActive(true);

                // 기본적인 설정이 끝난 아이템 객체를 초기화 (이 때 아이템 인덱스를 넘김)
                itemObj.Initialize(boMonster.sdMonster.dropItemRef[i]);
            }

            ObjectPoolManager.Instance.GetPool<Monster>(Define.PoolType.Monster).ReturnPoolableObject(this);
        }
    }
}
