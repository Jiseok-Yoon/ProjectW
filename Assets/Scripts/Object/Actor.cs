using ProjectW.Battle;
using ProjectW.DB;
using UnityEngine;

namespace ProjectW.Object
{
    using ActorState = Define.Actor.ActorState;
    using ActorAnim = Define.Actor.ActorAnim;

    /// <summary>
    /// �ΰ��� ���� ���̳����ϰ� �ൿ�ϴ� ��ü���� �߻�ȭ�� ���̽� Ŭ����
    /// ĳ����, ���� �� Actor�� �Ļ�Ŭ�������� ����Ǵ� ����� �ִ��� Actor�� ����
    /// �Ļ� Ŭ������ ���� �ٸ� ����� �ش� �Ļ�Ŭ�������� ������ ����
    /// </summary>
    public abstract class Actor : MonoBehaviour
    {
        /// <summary>
        /// ������ ����
        /// </summary>
        public ActorState State { get; private set; }

        /// <summary>
        /// ������ bo ������
        /// </summary>
        public BoActor boActor;

        /// <summary>
        /// ������ �ݶ��̴� ������Ʈ ����
        /// </summary>
        public Collider Coll { get; private set; }
        protected Rigidbody rig;
        protected Animator anim;

        protected AttackController attackController;

        /// <summary>
        /// ���� �ʱ�ȭ, �ʱ�ȭ �� �ܺο��� boActor �����͸� ���Թ޴´�. (������ ����)
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
        /// ���� ���� ���� �߻� �޼���
        /// </summary>
        public virtual void SetStats() { }

        /// <summary>
        /// ���� �ν��Ͻ��� ��� ������Ʈ�� ���
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
        /// �̵� ������Ʈ �߻� �޼���
        /// </summary>
        public virtual void MoveUpdate() { }

        // ����
        // ĳ���ͳ� ���� ���� Ŭ���������� �ش� ��ü�� ������ �� ��ü�� ����ϴ� ����� ������ �����ϴ� ���� ���� ����Ʈ
        // ��� ���� Ŭ������ �з��Ͽ�, ��ü ���� �� �ʿ��� ��ɵ��� �ν��Ͻ��ϰ�, �ν��Ͻ��� ����� ������ ���� ���°� ����Ʈ
        // ���� ��� ����� �з��Ͽ� �����ϱ�� ����Ƿ�, ��ǥ������ ���� ���� ����� ������ ��Ʈ�ѷ��� �ۼ��Ͽ� ����غ��Կ�


        #region State Controll
        /// <summary>
        /// ������ ���� ���� ���
        /// </summary>
        /// <param name="state">�����ϰ��� �ϴ� ����</param>
        public virtual void SetState(ActorState state)
        {
            var prevState = State;
            State = state;

            // ������ �Ļ� ��ü���� ���������� ���� ���¸��� ���̽����� ó���Ѵ�.
            // �� �� �Ļ� ��ü�� ���� �߰������� ���� ���´� �ش� �Ļ�Ŭ�������� ������ ó���Ѵ�.
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
        /// ��� ���·� ���� �� �� �� ȣ��
        /// </summary>
        protected virtual void OnIdle()
        {
            // ���� ȣ��Ǵ� �޼��忡�� ToString ����ϰ� ȣ���ϴ� �� ��������..
            anim.SetBool(ActorAnim.isWalk.ToString(), false);
            anim.SetBool(ActorAnim.isJump.ToString(), false);
        }

        /// <summary>
        /// �ȴ� ���·� ���� �� �� �� ȣ��
        /// </summary>
        protected virtual void OnWalk()
        {
            anim.SetBool(ActorAnim.isWalk.ToString(), true);
        }

        /// <summary>
        /// ���� ���·� ���� �� �� �� ȣ��
        /// </summary>
        protected virtual void OnAttack()
        {
            attackController.canCheckCoolTime = false;
            attackController.isCoolTime = true;
            anim.SetBool(ActorAnim.isAttack.ToString(), true);
        }

        /// <summary>
        /// ���� ���·� ���� �� �� �� ȣ��
        /// </summary>
        protected virtual void OnDead()
        {
            anim.SetBool(ActorAnim.isDead.ToString(), true);
        }

        #endregion

        // �ִϸ��̼��� Ư�� �����ӿ� ���� ȣ���ϰ��� �ϴ� �޼��带 ����Ͽ� �ش� �����ӿ� �������� ��,
        // ����� �޼��尡 ����ǵ��� �ϴ� ��� (����� �� �ִ� �޼���� ���������ڰ� public�� �޼��常)
        #region Animation Event
        /// <summary>
        /// ���� ��� �߿� Ÿ�� �Ǵ� �߻�ü�� �߻��ϴ� ������ ȣ��� �̺�Ʈ
        /// </summary>
        public virtual void OnAttackHit()
        {
            attackController.OnAttack();
        }

        /// <summary>
        /// ���� ��� �߿� ����� �������� ȣ��� �̺�Ʈ
        /// </summary>
        public virtual void OnAttackEnd()
        {
            attackController.canCheckCoolTime = true;
            anim.SetBool(ActorAnim.isAttack.ToString(), false);
            SetState(ActorState.Idle);
        }

        /// <summary>
        /// �״� ��� �߿� ����� �������� ȣ��� �̺�Ʈ
        /// </summary>
        public virtual void OnDeadEnd()
        { 
        
        }
        #endregion
    }
}