using ProjectW.Object;
using ProjectW.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectW.UI
{
    public class HpBar : MonoBehaviour, IPoolableObject
    {
        public Actor target;
        public Image hpGauge;

        public bool CanRecycle { get; set; } = true;

        public void Initialize(Actor target)
        {
            this.target = target;
            hpGauge.fillAmount = 1f;

            // ������Ʈ Ǯ�� ����ϴ� �ֵ��� ������ �ش� Ǯ�� Ȧ���� ������ �ϰ� ����..
            // �ٵ� Ǯ���� ������ ����� �� Ȧ������ ���� ĵ������ ���̶�Ű ����
            // �̵��� �߻��ϹǷ� �� �� ����ϰ� �ִ� ���� ĵ������ �������� �����ϸ��� �߻���.
            // ����, ���� �⺻ ������ ������ ����..
            transform.localScale = Vector3.one;
        }

        public void HpBarUpdate()
        {
            if (target == null || target.Coll == null)
                return;

            if (target.State == Define.Actor.ActorState.Dead)
            {
                ObjectPoolManager.Instance.GetPool<HpBar>(Define.PoolType.HpBar).ReturnPoolableObject(this);
                return;
            }

            transform.position = target.transform.position + Vector3.up * target.Coll.bounds.size.y * 1.2f;
            hpGauge.fillAmount = target.boActor.currentHp / target.boActor.maxHp;
        }
    }
}
