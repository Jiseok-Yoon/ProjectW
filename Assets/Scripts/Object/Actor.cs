using ProjectW.Battle;
using ProjectW.DB;
using UnityEngine;

namespace ProjectW.Object
{
    using ActorState = Define.Actor.ActorState;
    using ActorAnim = Define.Actor.ActorAnim;

    /// <summary>
    /// 인게임 내에 다이나믹하게 행동하는 객체들의 추상화된 베이스 클래스
    /// 캐릭터, 몬스터 등 Actor의 파생클래스에서 공통되는 기능은 최대한 Actor에 정의
    /// 파생 클래스에 따라 다른 기능은 해당 파생클래스에서 별도로 정의
    /// </summary>
    public abstract class Actor : MonoBehaviour
    {
        /// <summary>
        /// 액터의 상태
        /// </summary>
        public ActorState State { get; private set; }

        /// <summary>
        /// 액터의 bo 데이터
        /// </summary>
        public BoActor boActor;

        /// <summary>
        /// 액터의 콜라이더 컴포넌트 참조
        /// </summary>
        public Collider Coll { get; private set; }
        protected Rigidbody rig;
        protected Animator anim;

        protected AttackController attackController;

        /// <summary>
        /// 액터 초기화, 초기화 시 외부에서 boActor 데이터를 주입받는다. (의존성 주입)
        /// </summary>
        /// <param name="boActor"></param>
        public virtual void Initialize(BoActor boActor)
        {
            this.boActor = boActor;

            attackController ??= gameObject.AddComponent<AttackController>();
            attackController.Initialize(this);
        }

        protected virtual void Start()
        {
            Coll = GetComponent<Collider>();
            rig = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
        }

        /// <summary>
        /// 액터 스텟 설정 추상 메서드
        /// </summary>
        public virtual void SetStats() { }

        /// <summary>
        /// 액터 인스턴스의 모든 업데이트를 담당
        /// </summary>
        public virtual void ActorUpdate()
        {
            attackController.AttackIntervalUpdate();
            attackController.CheckAttack();

            if (State == ActorState.Attack)
                return;

            MoveUpdate();
        }

        /// <summary>
        /// 이동 업데이트 추상 메서드
        /// </summary>
        public virtual void MoveUpdate() { }

        // 정석
        // 캐릭터나 몬스터 같은 클래스에서는 해당 객체의 데이터 및 객체가 사용하는 기능의 참조만 존재하는 것이 가장 베스트
        // 기능 별로 클래스를 분류하여, 객체 생성 시 필요한 기능들을 인스턴스하고, 인스턴스된 기능의 참조만 갖는 형태가 베스트
        // 현재 모든 기능을 분류하여 정의하기는 힘드므로, 대표적으로 공격 관련 기능을 별도의 컨트롤러로 작성하여 사용해볼게요


        #region State Controll
        /// <summary>
        /// 액터의 상태 변경 기능
        /// </summary>
        /// <param name="state">변경하고자 하는 상태</param>
        public virtual void SetState(ActorState state)
        {
            var prevState = State;
            State = state;

            // 액터의 파생 객체들의 공통적으로 갖는 상태만을 베이스에서 처리한다.
            // 그 후 파생 객체에 따라 추가적으로 갖는 상태는 해당 파생클래스에서 별도로 처리한다.
            switch (state)
            {
                case ActorState.Idle:
                    OnIdle();
                    break;
                case ActorState.Walk:
                    OnWalk();
                    break;
                case ActorState.Attack:
                    if (attackController.isCoolTime)
                    {
                        State = prevState;
                        return;
                    }
                    OnAttack();
                    break;
                case ActorState.Dead:
                    OnDead();
                    break;
            }
        }

        /// <summary>
        /// 대기 상태로 변경 시 한 번 호출
        /// </summary>
        protected virtual void OnIdle()
        {
            // 자주 호출되는 메서드에서 ToString 빈번하게 호출하는 것 좋지않음..
            anim.SetBool(ActorAnim.isWalk.ToString(), false);
            anim.SetBool(ActorAnim.isJump.ToString(), false);
        }

        /// <summary>
        /// 걷는 상태로 변경 시 한 번 호출
        /// </summary>
        protected virtual void OnWalk()
        {
            anim.SetBool(ActorAnim.isWalk.ToString(), true);
        }

        /// <summary>
        /// 공격 상태로 변경 시 한 번 호출
        /// </summary>
        protected virtual void OnAttack()
        {
            attackController.canCheckCoolTime = false;
            attackController.isCoolTime = true;
            anim.SetBool(ActorAnim.isAttack.ToString(), true);
        }

        /// <summary>
        /// 죽은 상태로 변경 시 한 번 호출
        /// </summary>
        protected virtual void OnDead()
        {
            anim.SetBool(ActorAnim.isDead.ToString(), true);
        }

        #endregion

        // 애니메이션의 특정 프레임에 내가 호출하고자 하는 메서드를 등록하여 해당 프레임에 도달했을 때,
        // 등록한 메서드가 실행되도록 하는 기능 (등록할 수 있는 메서드는 접근제한자가 public인 메서드만)
        #region Animation Event
        /// <summary>
        /// 공격 모션 중에 타점 또는 발사체를 발사하는 시점에 호출될 이벤트
        /// </summary>
        public virtual void OnAttackHit()
        {
            attackController.OnAttack();
        }

        /// <summary>
        /// 공격 모션 중에 모션의 마지막에 호출될 이벤트
        /// </summary>
        public virtual void OnAttackEnd()
        {
            attackController.canCheckCoolTime = true;
            anim.SetBool(ActorAnim.isAttack.ToString(), false);
            SetState(ActorState.Idle);
        }

        /// <summary>
        /// 죽는 모션 중에 모션의 마지막에 호출될 이벤트
        /// </summary>
        public virtual void OnDeadEnd()
        { 
        
        }
        #endregion
    }
}