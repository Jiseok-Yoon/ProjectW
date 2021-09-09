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
        private float currentPatrolWaitTime; // ���� �������ð�
        private float patrolWaitTime; // �������ð� üũ ����
        private Vector3 destPos; // ������ ��ġ (���� ����, Ÿ�� ����)

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

            // �������� ������ ���� ��ġ�� �����Ͽ� �ٷ� ������ ���� ���·� �ν��ϰ� �Ͽ�
            // ������ ���ο� �������� ������ �� �ְ� �ϱ� ���ؼ�
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

                // �ӷ� ����
                agent.speed = boMonster.moveSpeed;
                // ������ ����
                agent.SetDestination(destPos);
            }
            else
            {
                SetState(Define.Actor.ActorState.Idle);
            }
        }

        /// <summary>
        /// ��Ȳ�� ���� �������� �����ϰ� ������ ���θ� ��ȯ
        /// </summary>
        /// <returns></returns>
        private bool GetMovement()
        {
            // Ÿ�ٿ� ���� ������ ������ �ִ���?
            if (attackController.hasTarget)
            {
                // ���� ���� ���� ĳ���Ͱ� ������ true, �ƴ϶�� false
                // !canAtk�� ������ �����ϸ� ������ ���θ� false, ���� �Ұ����� ���¶�� true
                return !attackController.canAtk;
            }

            // ���°� �����¶��
            if (State == Define.Actor.ActorState.Idle)
            {
                currentPatrolWaitTime += Time.deltaTime;
                // ��� �ð��� �����ٸ�
                if (currentPatrolWaitTime >= patrolWaitTime)
                {
                    // ���ð� �ʱ�ȭ
                    InitPatrolWaitTime();
                    // �̵��� �� �ְ� true ��ȯ
                    return true;
                }

                // ���ð��� ������ �ʾ����Ƿ� �̵��� �� ���� false ��ȯ
                return false;
            }

            var distance = (destPos - transform.position).magnitude;

            // ���� ��ġ�� �����ߴٸ� ���� ��ġ�� ����
            // false ��ȯ���Ѽ� �ٷ� �̵��ϴ� ���� �ƴ� �� �ʰ� ��� �� �̵��ϰ�
            if (distance < agent.stoppingDistance)
            {
                ChangeDestPos();

                return false;
            }

            return true;
        }

        /// <summary>
        /// ���� ��ġ�� �����ϴ� ���
        /// </summary>
        private void ChangeDestPos()
        {
            // ���͸��� ���� ������ �ٸ��Ƿ� ������ �ε��� ���� �Ѱ� �ش� ������
            // ���� ���� ������ ������ ��ġ�� ��ȯ�޴´�.
            destPos = StageManager.Instance.GetRandPosInArea(boMonster.sdMonster.index);

            var isExist = agent.CalculatePath(destPos, path);

            // �ش� ��ġ�� �׺�޽��� �����ϴ��� Ȯ��
            if (!isExist)
            {
                // ��� ȣ���� ���� �������� ���� �����ϰ� �ٽ� �˻�.
                ChangeDestPos();
            }
            // �ش� ��ġ�� �׺�޽��� ����������, �������� ������ �� ���� ���
            else if (path.status == NavMeshPathStatus.PathPartial)
            {
                ChangeDestPos();
            }
        }

        /// <summary>
        /// ���� ��� �ð� �ʱ�ȭ
        /// </summary>
        private void InitPatrolWaitTime()
        {
            currentPatrolWaitTime = 0;
            patrolWaitTime = Random.Range(Define.Monster.MinPatrolWaitTime, Define.Monster.MaxPatrolWaitTime);
        }

        /// <summary>
        /// ���� ���� �� ��(�÷��̾�)�� �ִ��� üũ
        /// </summary>
        private void CheckDetection()
        {
            var extentsValue = boMonster.sdMonster.detectionRange;
            var halfExtents = new Vector3(extentsValue, extentsValue, extentsValue);

            var colls = Physics.OverlapBox(transform.position, halfExtents, transform.rotation,
                1 << LayerMask.NameToLayer("Player"));

            // ���� ���� �� �÷��̾ ���ٸ�
            if (colls == null || colls.Length == 0)
            {
                attackController.hasTarget = false;
                return;
            }

            // ���� ���� �� �÷��̾ �߰�
            attackController.hasTarget = true;
            // �÷��̾ ������ �� �ֵ��� �������� �÷��̾� ��ġ�� ����
            destPos = colls[0].transform.position;

            // �÷��̾���� �Ÿ��� ���� ���� ���̶�� ���ݰ��� ���·� ����
            var distance = destPos - transform.position;
            // �÷��̾�� �Ÿ��� ���ݹ��� ���̶��
            if (distance.sqrMagnitude <= boActor.atkRange)
            {
                attackController.canAtk = true;
                // ���� ���ɻ��°� �Ǹ� �̵����� ������ �켱�� �Ǿ� �̵��̳� ȸ���� ���� �ʴ´�.
                // �� ��, ĳ���Ͱ� �̵� �� ���Ͱ� ĳ���͸� �ٶ� �� �ְ� ȸ������ ����
                transform.rotation = Quaternion.LookRotation(distance.normalized);
            }
            else
            {
                attackController.canAtk = false;
            }
        }

        /// <summary>
        /// ���Ͱ� �״� �ִϸ��̼��� ������� ��, Ǯ�� ��ȯ
        /// </summary>
        public override void OnDeadEnd()
        {
            // ������ Ǯ�� �ϴ� �޾ƿ�
            var itemPool = ObjectPoolManager.Instance.GetPool<Item>(Define.PoolType.Item);
            // ���� UI ĵ������ �ϴ� �޾ƿ�
            var worldUICanvas = UIWindowManager.Instance.GetWindow<UIBattle>().worldUICanvas;

            // ���� ��ȹ �������� dropItemPer �ݺ������� �����ؼ�
            // ���Ȯ�� ��� �Ŀ�, ����̶���ش� �ε����� �������� ���� (������ Ǯ���� ������ ��ü ������ �����͸� ä��)

            // ���Ͱ� ����� �� �ִ� ������ ������ŭ �ݺ�
            for (int i = 0; i < boMonster.sdMonster.dropItemRef.Length; ++i)
            {
                // ������ ��� Ȯ�� ���
                var isDrop = boMonster.sdMonster.dropItemPer[i] <= Random.Range(0, 1f);

                // ����� ���� �ʾҴٸ� ���� ������ ��� Ȯ�� ������� �Ѿ
                if (!isDrop)
                    continue;

                // ��������� ������ Ǯ���� ������ ��ü�� �ϳ� ������
                var itemObj = itemPool.GetPoolableObject();

                // ������ ��ü�� �θ� ���� ĵ������ ����
                itemObj.transform.SetParent(worldUICanvas.transform);
                // ���� ĵ������ �θ� ����Ǹ鼭 �����ϸ��� �߻��ϹǷ� �������� �����Ϸ� �ǵ���
                itemObj.transform.localScale = Vector3.one * .5f;
                // ��ġ�� ������ ��ġ�� ����
                itemObj.transform.position = transform.position + Vector3.up * .25f;
                itemObj.gameObject.SetActive(true);

                // �⺻���� ������ ���� ������ ��ü�� �ʱ�ȭ (�� �� ������ �ε����� �ѱ�)
                itemObj.Initialize(boMonster.sdMonster.dropItemRef[i]);
            }

            ObjectPoolManager.Instance.GetPool<Monster>(Define.PoolType.Monster).ReturnPoolableObject(this);
        }
    }
}
