using ProjectW.Object;
using ProjectW.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectW.UI
{
    public class UIBattle : UIWindow
    {
        /// <summary>
        /// 마우스로 타겟을 타겟팅 하고 있는지에 대한 값을 해당 참조에서 받아옴
        /// </summary>
        public PlayerController playerController;

        public Texture2D defaultCursorTex; // 기본 커서 이미지
        public Texture2D targetPointCursorTex; // 몬스터에 마우스 포인팅 시 커서 이미지

        public Canvas worldUICanvas;

        public List<BubbleGauge> playerHpBubbles; // 플레이어의 hp 버블 이미지가 버블 2개가 붙어있음..
        public List<BubbleGauge> playerManaBubbles;
        public List<HpBar> allHpBar;

        private void Update()
        {
            CursorUpdate();
            BubbleGaugeUpdate();
            HpBarUpdate();
            BillboardUpdate();
        }

        private void BillboardUpdate()
        {
            var camTrans = CameraController.Cam.transform;

            for (int i = 0; i < worldUICanvas.transform.childCount; ++i)
            {
                var child = worldUICanvas.transform.GetChild(i);

                // 월드 ui를 사용하는 ui 객체가 카메라를 바라보게 (카메라를 따라 회전)
                child.LookAt(camTrans, Vector3.up);
                var newRot = child.eulerAngles;
                newRot.x = 0;
                newRot.z = 0;
                child.eulerAngles = newRot;
            }
        }

        private void CursorUpdate()
        {
            Cursor.SetCursor(playerController.HasPointTarget ? 
                targetPointCursorTex : defaultCursorTex, Vector2.zero, CursorMode.Auto);
        }

        private void BubbleGaugeUpdate()
        {
            var boActor = playerController.PlayerCharacter?.boActor;
            if (boActor == null)
                return;

            var hpGauge = boActor.currentHp / boActor.maxHp;
            var manaGauge = boActor.currentMana / boActor.maxMana;

            for (int i = 0; i < playerHpBubbles.Count; ++i)
            {
                playerHpBubbles[i].SetGauge(hpGauge);
            }

            for (int i = 0; i < playerManaBubbles.Count; ++i)
            {
                playerManaBubbles[i].SetGauge(manaGauge);
            }
        }

        private void HpBarUpdate()
        {
            for (int i = 0; i < allHpBar.Count; ++i)
            {
                allHpBar[i].HpBarUpdate();
            }
        }

        /// <summary>
        /// 매개변수로 받은 액터의 정보를 기준으로 체력바를 생성하여 allHpBar 리스트에 추가
        /// </summary>
        /// <param name="target"></param>
        public void AddHpBar(Actor target)
        {
            var hpBar = ObjectPoolManager.Instance.GetPool<HpBar>(Define.PoolType.HpBar).GetPoolableObject();

            hpBar.transform.SetParent(worldUICanvas.transform);
            hpBar.Initialize(target);
            hpBar.gameObject.SetActive(true);

            allHpBar.Add(hpBar);
        }

        /// <summary>
        /// 스테이지 전환 시 현재 스테이지에 있는 hpBar 객체를 전부 풀에 반환
        /// </summary>
        public void Clear()
        {
            var hpBarPool = ObjectPoolManager.Instance.GetPool<HpBar>(Define.PoolType.HpBar);

            for (int i = 0; i < allHpBar.Count; ++i)
            {
                hpBarPool.ReturnPoolableObject(allHpBar[i]);
            }
            allHpBar.Clear();
        }
    }
}
