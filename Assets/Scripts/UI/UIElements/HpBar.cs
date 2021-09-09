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

            // 오브젝트 풀로 사용하는 애들은 별도의 해당 풀의 홀더에 보관을 하고 있음..
            // 근데 풀에서 꺼내서 사용할 때 홀더에서 월드 캔버스로 하이라키 상의
            // 이동이 발생하므로 이 때 사용하고 있는 월드 캔버스를 기준으로 스케일링이 발생됌.
            // 따라서, 원래 기본 스케일 값으로 변경..
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
