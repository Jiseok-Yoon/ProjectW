using ProjectW.Define;
using System;
using UnityEngine;

namespace ProjectW.DB
{
    /// <summary>
    /// BoActor는 캐릭터와 몬스터의 공통된 데이터를 정의
    /// BO 는 통신으로 받은(서버) 데이터, 기획에서 정의한 데이터, 인게임에서만 사용하는 임시데이터(휘발성)
    /// </summary>
    [Serializable]
    public class BoActor
    {
        public float level;
        public Actor.ActorType actorType;
        public Actor.AttackType atkType;
        public float moveSpeed;
        public Vector3 moveDir;
        public Vector3 rotDir;
        public float currentHp;
        public float maxHp;
        public float currentMana;
        public float maxMana;
        public float atk;
        public float def;
        public float atkRange;
        public bool isGround;
        public float atkInterval;
    }
}
