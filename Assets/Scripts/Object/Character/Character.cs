using ProjectW.DB;
using UnityEngine;

namespace ProjectW.Object
{
    using ActorState = Define.Actor.ActorState;
    using ActorAnim = Define.Actor.ActorAnim;

    public class Character : Actor
    {
        public BoCharacter boCharacter;

        /// <summary>
        /// ������ �Ѱܹ޴� �����ʹ� BoActor �� �ƴ� BoActor���� �Ļ��� BoCharacter ������ ������
        /// </summary>
        /// <param name="boActor"></param>
        public override void Initialize(BoActor boActor)
        {
            base.Initialize(boActor);

            this.boCharacter = boActor as BoCharacter;

            SetStats();
        }

        public override void SetState(ActorState state)
        {
            // �������� ���� ���´� ���̽����� ó��
            base.SetState(state);

            // ĳ���͸� ���� ���´� ĳ���� �ʿ��� ó��
            switch (state)
            {
                case ActorState.Sit:
                    break;
                case ActorState.Rise:
                    break;
                case ActorState.Jump:
                    OnJump();
                    break;
            }
        }

        public override void SetStats()
        {
            // ĳ���� ���� ����
            boCharacter.actorType = Define.Actor.ActorType.Character;
            boCharacter.atkType = boCharacter.sdCharacter.atkType;
            boCharacter.moveSpeed = boCharacter.sdCharacter.moveSpeed;
            boCharacter.currentHp = boCharacter.maxHp = boCharacter.level * boCharacter.sdGrowthStat.maxHp * boCharacter.sdGrowthStat.maxHpFactor;
            boCharacter.currentMana = boCharacter.maxMana = boCharacter.level * boCharacter.sdGrowthStat.maxMana * boCharacter.sdGrowthStat.maxManaFactor;
            boCharacter.atk = boCharacter.level * boCharacter.sdGrowthStat.atk * boCharacter.sdGrowthStat.atkFactor;
            boCharacter.def = boCharacter.level * boCharacter.sdGrowthStat.def * boCharacter.sdGrowthStat.defFactor;
            boCharacter.atkRange = boCharacter.sdCharacter.atkRange;
            boCharacter.atkInterval = boCharacter.sdCharacter.atkInterval;
        }

        public override void ActorUpdate()
        {
            CheckGround();

            base.ActorUpdate();
        }

        public override void MoveUpdate()
        {
            var velocity = boActor.moveSpeed * boActor.moveDir;
            velocity = transform.TransformDirection(velocity);

            transform.localPosition += velocity * Time.fixedDeltaTime;
            transform.Rotate(boActor.rotDir * Define.Camera.CamRotSpeed);


            // ���� ���¿��� ������ �� �̵� ����� �ƴ� ���� ����� �״�� �����Ű�� ���ؼ�
            if (State == ActorState.Jump )
                return;

            // �ӵ� ������ ���̰� 0�� ���ٸ� �ȿ����δٴ� ��..
            if (Mathf.Approximately(velocity.magnitude, 0))
            {
                SetState(ActorState.Idle);
            }
            else
            {
                SetState(ActorState.Walk);
            }
        }

        /// <summary>
        /// ĳ���Ͱ� ���� �ִ��� üũ�ϴ� ���
        /// </summary>
        private void CheckGround()
        {
            // ����ĳ������ �̿��ؼ� ĳ���Ͱ� ���� ��Ҵ��� Ȯ��
            boActor.isGround = Physics.Raycast(transform.position, Vector3.down, .1f, 1 << LayerMask.NameToLayer("Floor"));

            if (State != ActorState.Jump)
                return;

            // �� �Ʒ��� ���Դٴ� �� ĳ������ ���°� ���� ���¶�� ��
            // �� �� isGround�� true��� �����ߴٴ� ��
            if (boActor.isGround)
                // ���� ���¸� ��� ���·� ����
                SetState(ActorState.Idle);
        }

        /// <summary>
        /// ���� ���� ���� (���� Ű�� ������ �� �ѹ� ȣ��)
        /// </summary>
        public void OnJump()
        {
            // �̹� �����̶�� ������ �� ���� ����
            if (!boActor.isGround)
                return;

            anim.SetBool(ActorAnim.isJump.ToString(), true);

            rig.AddForce(Vector3.up * boCharacter.sdCharacter.jumpForce, ForceMode.Impulse);
        }
    }
}