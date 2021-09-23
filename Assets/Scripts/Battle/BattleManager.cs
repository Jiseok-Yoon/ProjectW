using ProjectW.Object;
using ProjectW.UI;
using ProjectW.Util;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectW.Battle
{
    using ActorType = Define.Actor.ActorType;
    /// <summary>
    /// 인게임에 활성화된 액터 객체들을 관리하는 클래스
    /// </summary>
    public class BattleManager : Singleton<BattleManager>
    {
        public List<Actor> Characters { get; private set; } = new List<Actor>();
        public List<Actor> Monsters { get; private set; } = new List<Actor>();
        public List<NPC> NPCS { get; private set; } = new List<NPC>();

        /// <summary>
        /// 활성화된 액터를 BM에 등록하는 기능
        /// 이 곳에 등록된 액터만 업데이트가 된다.
        /// </summary>
        /// <param name="actor">등록하고자 하는 활성화된 액터의 인스턴스</param>
        public void AddActor(Actor actor)
        {
            switch (actor)
            {
                // actor의 타입이 몬스터와 같다면 임시로 actor를 monster라는 변수로 선언해서 사용
                case var monster when actor.boActor.actorType == ActorType.Monster:
                    Monsters.Add(monster);
                    UIWindowManager.Instance.GetWindow<UIBattle>().AddHpBar(monster);
                    break;
                case var character when actor.boActor.actorType == ActorType.Character:
                    Characters.Add(character);
                    break;
            }

            // 몬스터 같은 경우 오브젝트 풀에서 가져와서 사용하기 때문에
            // 객체가 풀에 있을 경우(비활성화된 상태), 따라서 사용을 위해 활성화
            actor.gameObject.SetActive(true);
        }

        private void FixedUpdate()
        {
            // 특정 객체의 클래스 안에 Update 콜백을 직접적으로 갖는 것보다..
            // 특정 객체들을 담을 컨테이너를 만들고 해당 컨테이너에 객체들을
            // 하나의 업데이트 콜백에서 처리할 시 일반적으로 훨씬 성능면에서 우수하다..
            ActorUpdate(Characters);
            ActorUpdate(Monsters);
        }

        private void ActorUpdate(List<Actor> actors)
        {
            for (int i = 0; i < actors.Count; ++i)
            {
                // 액터가 죽지않았다면 업데이트
                if (actors[i].State != Define.Actor.ActorState.Dead)
                    actors[i].ActorUpdate();
                else
                {
                    // 죽었다면 액터 컨테이너에서 제거
                    actors.RemoveAt(i);
                    // 반복되는 곳에서 리스트 안에 원소를 지울 때
                    // 증감연산자 ++로 사용할 현재 인덱스에서 -- 해줘야함.
                    --i;
                }
            }
        }
    }
}
