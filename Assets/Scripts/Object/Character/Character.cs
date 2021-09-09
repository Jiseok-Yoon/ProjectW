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
        /// 실제로 넘겨받는 데이터는 BoActor 가 아닌 BoActor에서 파생된 BoCharacter 형태의 데이터
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
            // 공통으로 갖는 상태는 베이스에서 처리
            base.SetState(state);

            // 캐릭터만 갖는 상태는 캐릭터 쪽에서 처리
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
            // 캐릭터 스텟 설정
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


            // 점프 상태에서 움직일 시 이동 모션이 아닌 점프 모션을 그대로 실행시키기 위해서
            if (State == ActorState.Jump )
                return;

            // 속도 벡터의 길이가 0과 같다면 안움직인다는 뜻..
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
        /// 캐릭터가 땅에 있는지 체크하는 기능
        /// </summary>
        private void CheckGround()
        {
            // 레이캐스팅을 이용해서 캐릭터가 땅에 닿았는지 확인
            boActor.isGround = Physics.Raycast(transform.position, Vector3.down, .1f, 1 << LayerMask.NameToLayer("Floor"));

            if (State != ActorState.Jump)
                return;

            // 이 아래가 들어왔다는 건 캐릭터의 상태가 점프 상태라는 것
            // 이 때 isGround가 true라면 착지했다는 뜻
            if (boActor.isGround)
                // 따라서 상태를 대기 상태로 변경
                SetState(ActorState.Idle);
        }

        /// <summary>
        /// 점프 연산 실행 (점프 키를 눌렀을 때 한번 호출)
        /// </summary>
        public void OnJump()
        {
            // 이미 공중이라면 점프할 수 없게 리턴
            if (!boActor.isGround)
                return;

            anim.SetBool(ActorAnim.isJump.ToString(), true);

            rig.AddForce(Vector3.up * boCharacter.sdCharacter.jumpForce, ForceMode.Impulse);
        }
    }
}