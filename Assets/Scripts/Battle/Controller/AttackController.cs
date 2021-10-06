using ProjectW.Object;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectW.Battle
{
    using ActorType = Define.Actor.ActorType;
    using ActorState = Define.Actor.ActorState;
    using AttackType = Define.Actor.AttackType;

    /// <summary>
    /// 액터의 공격 기능을 담당할 컨트롤러
    /// </summary>
    public class AttackController : MonoBehaviour
    {
        /// <summary>
        /// 공격 대상이 있는지
        /// </summary>
        public bool hasTarget;

        /// <summary>
        /// 공격 쿨타임을 체크할 수 있는지? (공격 모션이 끝나기 전에는 쿨타임체크 막는다.)
        /// </summary>
        public bool canCheckCoolTime;

        /// <summary>
        /// 공격 쿨타임인지?
        /// </summary>
        public bool isCoolTime;

        /// <summary>
        /// 공격 가능 상태인지?
        /// </summary>
        public bool canAtk;

        /// <summary>
        /// 현재 공격쿨타임을 체크하는 값
        /// </summary>
        private float currentAtkInterval;

        /// <summary>
        /// 공격자 (해당 어택 컨트롤러 인스턴스를 갖는 액터)
        /// </summary>
        private Actor attacker;

        /// <summary>
        /// 피격 대상 액터들을 갖는 리스트
        /// </summary>
        private List<Actor> targets = new List<Actor>();

        public void Initialize(Actor attacker)
        {
            this.attacker = attacker;
        }

        /// <summary>
        /// 공격 가능 상태라면 공격자의 상태를 공격상태로 변경
        /// </summary>
        public void CheckAttack()
        {
            // 타겟이 없다면 리턴
            if (!hasTarget)
                return;

            // 공격 쿨타임이라면 리턴
            if (isCoolTime)
                return;

            // 공격 불가능이라면 리턴
            if (!canAtk)
                return;


            
            attacker.SetState(ActorState.Attack);
        }

        /// <summary>
        /// 액터의 공격모션이 타격점에 도달했을 때 호출
        /// 근접 공격이라면 공격 범위 연산 후 데미지 연산..
        /// 발사체를 이용한 공격이라면 발사체를 생성
        /// </summary>
        public virtual void OnAttack()
        {
            switch (attacker.boActor.atkType)
            {
                case AttackType.Normal:
                    CalculateAttackRange();

                    var damage = attacker.boActor.atk;

                    for (int i = 0; i < targets.Count; ++i)
                        CalculateDamage(damage, targets[i]);
                    break;
                case AttackType.Projectile:
                    OnFire();
                    break;
            }
        }

        /// <summary>
        /// 공격 범위에 적이 있는지 연산
        /// </summary>
        public virtual void CalculateAttackRange()
        {
            var targetLayer = attacker.boActor.actorType == ActorType.Character ?
                LayerMask.NameToLayer("Monster") : LayerMask.NameToLayer("Player");

            var hits = Physics.SphereCastAll(attacker.transform.position, .5f, attacker.transform.forward,
                attacker.boActor.atkRange, 1 << targetLayer);

            // 타겟에 대한 정보를 새로 구했으니 이전 타겟에 대한 정보를 지운다.
            targets.Clear();

            // 새로운 타겟 정보를 타겟 목록에 넣는다.
            for (int i = 0; i < hits.Length; ++i)
            {
                targets.Add(hits[i].transform.GetComponent<Actor>());
            }
        }

        /// <summary>
        /// 원거리타입 발사체 생성
        /// </summary>
        public virtual void OnFire()
        { 
        
        }

        /// <summary>
        /// 데미지를 공식에 따라 연산하여 타겟에 적용
        /// </summary>
        /// <param name="damage">공격자가 가한 데미지</param>
        /// <param name="target">피격 대상</param>
        public virtual void CalculateDamage(float damage, Actor target)
        {
            // 데미지 계산
            var calDamage = Mathf.Max(damage - target.boActor.def, 0);

            // 계산된 데미지를 타겟의 현재 체력에 적용
            target.boActor.currentHp = Mathf.Max(target.boActor.currentHp - calDamage, 0);

            // 타겟의 체력이 0이라면 죽은 상태로 변경
            if (target.boActor.currentHp <= 0)
                target.SetState(ActorState.Dead);
        }

        /// <summary>
        /// 공격 쿨타임을 업데이트 하는 기능
        /// FixedUpdate 에서 호출
        /// </summary>
        public void AttackIntervalUpdate()
        {
            // 쿨타임을 체크할 수 없다면 리턴
            if (!canCheckCoolTime)
                return;

            // 공격 쿨타임이 아니라면 리턴
            if (!isCoolTime)
                return;

            currentAtkInterval += Time.fixedDeltaTime;
            if (currentAtkInterval >= attacker.boActor.atkInterval)
            {
                InitAttackInterval();
            }
        }

        /// <summary>
        /// 공격 쿨타임 초기화
        /// </summary>
        public void InitAttackInterval()
        {
            currentAtkInterval = 0;
            isCoolTime = false;
        }
    }
}
