using ProjectW.Resource;
using UnityEngine;

namespace ProjectW.Object
{
    using CamView = Define.Camera.CamView;

    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// 현재 카메라의 뷰 상태를 나타낼 필드
        /// </summary>
        public CamView camView;
        /// <summary>
        /// 카메라 이동 시 선형보간을 이용한 이동을 사용할 것임. 그 때 선형 보간 이동에 사용할 스무스 값
        /// </summary>
        public float smooth = 3f;

        /// <summary>
        /// 뒤쪽에서 3인칭으로 캐릭터를 찍을 때의 트랜스폼
        /// </summary>
        private Transform standardPos;
        /// <summary>
        /// 앞쪾에서 3인칭으로 캐릭터를 찍을 때의 트랜스폼
        /// </summary>
        private Transform frontPos;

        /// <summary>
        /// 카메라가 추적할 타겟의 트랜스폼 (플레이어의 트랜스폼)
        /// </summary>
        private Transform target;

        /// <summary>
        /// 카메라 컴포넌트를 이용해 서로 다른 좌표계에서 좌표변환을 이용해 연산을 해야할 경우가 빈번하게 발생되므로
        /// 이 때 처음에 카메라 컴포넌트의 참조를 한 번 담아둔 다음에 편리하게 접근하기 위해 
        /// </summary>
        public static Camera Cam { get; private set; }

        private void Start()
        {
            Cam = GetComponent<Camera>();
        }

        /// <summary>
        /// 카메라의 추적 타겟을 설정하는 기능
        /// </summary>
        /// <param name="newTarget">추적하고자 하는 타겟의 트랜스폼</param>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;

            // CamPos 프리팹 객체를 미리 생성해둠
            // CamPos 객체에는 프론트뷰와 스탠다드 뷰의 트랜스폼을 갖는 자식 객체가 존재함
            var camPos = Instantiate(ResourceManager.Instance.LoadObject(Define.Camera.CamPosPath)).transform;
            // CamPos의 부모를 타겟으로 설정함
            camPos.parent = target.transform;
            camPos.localPosition = Vector3.zero;

            // 위의 과정을 통해 결과적으로 스탠다드뷰와 프론트뷰의 트랜스폼이 타겟을 기준으로 트랜스폼을 갖게됌
            standardPos = camPos.Find("StandardPos");
            frontPos = camPos.Find("FrontPos");

            // 기본으로 카메라의 위치와 방향을 스탠타드 뷰의 위치와 방향으로 설정
            transform.position = standardPos.position;
            transform.forward = standardPos.forward;
        }

        private void FixedUpdate()
        {
            if (target == null)
                return;

            switch (camView)
            {
                case CamView.Standard:
                    SetPosition(false, standardPos);
                    break;
                case CamView.Front:
                    SetPosition(false, frontPos);
                    break;
            }
        }

        public void SetForceStandardView()
        {
            SetPosition(false, standardPos);
        }

        /// <summary>
        /// 카메라의 이동 및 회전 연산
        /// </summary>
        /// <param name="isLerp">이동 시 보간할 것인지? 보간하지 않을 시 한 번에 이동</param>
        /// <param name="target">스탠다드뷰 또는 프론트 뷰 둘 중 하나</param>
        private void SetPosition(bool isLerp, Transform target)
        {
            // isLerp라면 타겟 위치로 보간해서 이동
            if (isLerp)
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.fixedDeltaTime * smooth);
                transform.forward = Vector3.Lerp(transform.forward, target.forward, Time.fixedDeltaTime * smooth);
            }
            // 타겟 위치로 한 번에 이동
            else
            {
                transform.position = target.position;
                transform.forward = target.forward;
            }
        }
    }
}
