using ProjectW.Object;
using ProjectW.UI;
using ProjectW.Util;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectW.Battle
{
    using ActorType = Define.Actor.ActorType;
    /// <summary>
    /// �ΰ��ӿ� Ȱ��ȭ�� ���� ��ü���� �����ϴ� Ŭ����
    /// </summary>
    public class BattleManager : Singleton<BattleManager>
    {
        public List<Actor> Characters { get; private set; } = new List<Actor>();
        public List<Actor> Monsters { get; private set; } = new List<Actor>();
        public List<NPC> NPCS { get; private set; } = new List<NPC>();

        /// <summary>
        /// Ȱ��ȭ�� ���͸� BM�� ����ϴ� ���
        /// �� ���� ��ϵ� ���͸� ������Ʈ�� �ȴ�.
        /// </summary>
        /// <param name="actor">����ϰ��� �ϴ� Ȱ��ȭ�� ������ �ν��Ͻ�</param>
        public void AddActor(Actor actor)
        {
            switch (actor)
            {
                // actor�� Ÿ���� ���Ϳ� ���ٸ� �ӽ÷� actor�� monster��� ������ �����ؼ� ���
                case var monster when actor.boActor.actorType == ActorType.Monster:
                    Monsters.Add(monster);
                    UIWindowManager.Instance.GetWindow<UIBattle>().AddHpBar(monster);
                    break;
                case var character when actor.boActor.actorType == ActorType.Character:
                    Characters.Add(character);
                    break;
            }

            // ���� ���� ��� ������Ʈ Ǯ���� �����ͼ� ����ϱ� ������
            // ��ü�� Ǯ�� ���� ���(��Ȱ��ȭ�� ����), ���� ����� ���� Ȱ��ȭ
            actor.gameObject.SetActive(true);
        }

        private void FixedUpdate()
        {
            // Ư�� ��ü�� Ŭ���� �ȿ� Update �ݹ��� ���������� ���� �ͺ���..
            // Ư�� ��ü���� ���� �����̳ʸ� ����� �ش� �����̳ʿ� ��ü����
            // �ϳ��� ������Ʈ �ݹ鿡�� ó���� �� �Ϲ������� �ξ� ���ɸ鿡�� ����ϴ�..
            ActorUpdate(Characters);
            ActorUpdate(Monsters);
        }

        private void ActorUpdate(List<Actor> actors)
        {
            for (int i = 0; i < actors.Count; ++i)
            {
                // ���Ͱ� �����ʾҴٸ� ������Ʈ
                if (actors[i].State != Define.Actor.ActorState.Dead)
                    actors[i].ActorUpdate();
                else
                {
                    // �׾��ٸ� ���� �����̳ʿ��� ����
                    actors.RemoveAt(i);
                    // �ݺ��Ǵ� ������ ����Ʈ �ȿ� ���Ҹ� ���� ��
                    // ���������� ++�� ����� ���� �ε������� -- �������.
                    --i;
                }
            }
        }
    }
}
