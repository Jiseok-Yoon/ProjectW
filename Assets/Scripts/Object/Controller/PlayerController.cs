using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectW.Object
{
    using CamView = Define.Camera.CamView;
    using Input = Define.Input;

    /// <summary>
    /// 플레이어 캐릭터의 입력 처리
    /// 캐릭터 클레스에서 처리 안하는 이유?
    /// 캐릭터와 플레이어의 입력을 분리함으로써
    /// 캐릭터 클래스를 더 다양하게 사용할 수 있기 때문에 (ex : 멀티 환경에서의 캐릭터, NPC 등)
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private bool canRot;
        public bool HasPointTarget { get; private set; }
        private Transform pointingTarget;

        private Dictionary<string, InputHandler.AxisHandler> inputAxisDic;
        private Dictionary<string, InputHandler.ButtonHandler> inputButtonDic;

        public Character PlayerCharacter { get; private set; }
        public CameraController cameraController;

        public void Initialize(Character character)
        {
            character.transform.parent = transform;
            character.gameObject.layer = LayerMask.NameToLayer("Player");

            PlayerCharacter = character;
            cameraController.SetTarget(PlayerCharacter.transform);

            // 사용할 축 타입의 키의 키값과 사용 시 실행시킬 메서드를 딕셔너리에 등록
            inputAxisDic = new Dictionary<string, InputHandler.AxisHandler>();
            inputAxisDic.Add(Input.AxisX, new InputHandler.AxisHandler(GetAxisX)); // 캐릭터 좌우 회전 -> 마우스 회전 (사용안함)
            inputAxisDic.Add(Input.AxisZ, new InputHandler.AxisHandler(GetAxisZ)); // 캐릭터 앞, 뒤 이동
            inputAxisDic.Add(Input.MouseX, new InputHandler.AxisHandler(GetMouseX)); // 마우스 x축 값으로 캐릭터 좌우 회전
            inputAxisDic.Add(Input.MouseY, new InputHandler.AxisHandler(GetMouseY)); // 사용안함

            inputButtonDic = new Dictionary<string, InputHandler.ButtonHandler>();
            inputButtonDic.Add(Input.MouseLeft, new InputHandler.ButtonHandler(OnPressMouseLeft, null));
            inputButtonDic.Add(Input.MouseRight, new InputHandler.ButtonHandler(OnPressMouseRight, OnNotPressMouseRight));
            inputButtonDic.Add(Input.Jump, new InputHandler.ButtonHandler(OnPressJump, null));
            inputButtonDic.Add(Input.FrontCam, new InputHandler.ButtonHandler(OnPressFrontCam, OnNotPressFrontCam));
        }

        private void FixedUpdate()
        {
            // 플레이어 캐릭터가 세팅되기 전에는 입력할 수 없게 예외처리
            if (PlayerCharacter == null)
                return;

            // 죽었을 경우 입력할 수 없게 예외처리
            if (PlayerCharacter.State == Define.Actor.ActorState.Dead)
            { 
                return;
            }

            InputUpdate();
            CheckMousePointTarget();
        }

        private void CheckMousePointTarget()
        {
            // 현재 씬에서 사용하는 카메라에서 스크린 좌표계의 마우스 위치로의 레이 생성
            var ray = CameraController.Cam.ScreenPointToRay(UnityEngine.Input.mousePosition);

            // 생성한 레이를 통해 해당 레이 방향에 몬스터가 존재하는지 체크
            var hits = Physics.RaycastAll(ray, 1000f, 1 << LayerMask.NameToLayer("Monster"));

            // 레이캐스팅의 결과가 담긴 배열의 길이가 0이 아니라면 타겟 존재
            HasPointTarget = hits.Length != 0;
            // 캐릭터를 타겟 쪽으로 회전시키기 위해 타겟의 트랜스폼을 받는 것이므로
            // 그냥 맨 앞에 있는 타겟의 트랜스폼을 넣어줌..
            pointingTarget = HasPointTarget ? hits[0].transform : null;
        }

        private void InputUpdate()
        {
            foreach (var input in inputAxisDic)
            {
                var value = UnityEngine.Input.GetAxisRaw(input.Key);
                input.Value.GetAxisValue(value);
            }

            foreach (var input in inputButtonDic)
            {
                // 키를 누르고 있을 때 들어옴
                if (UnityEngine.Input.GetButton(input.Key))
                    input.Value.OnPress();
                else
                    // 키를 떼고 있다면 계속 호출 
                    // 1. 키를 떼고 있을 때 계속적인 작업이 필요한 경우
                    // 2. 키를 뗀 순간에 한 번만 실행하면 되는 작업
                    input.Value.OnNotPress();
            }
        }

        #region Input Implementation 입력 구현부
        private void GetAxisX(float value)
        { 
        
        }

        private void GetAxisZ(float value)
        {
            var newDir = PlayerCharacter.boActor.moveDir;
            newDir.z = value;
            PlayerCharacter.boActor.moveDir = newDir;
        }

        private void GetMouseX(float value)
        {
            var newDir = PlayerCharacter.boActor.rotDir;
            newDir.y = canRot ? value : 0;
            PlayerCharacter.boActor.rotDir = newDir;
        }

        private void GetMouseY(float value)
        { 
        
        }

        private void OnPressFrontCam()
        {
            cameraController.camView = CamView.Front;
        }

        private void OnNotPressFrontCam()
        {
            cameraController.camView = CamView.Standard;
        }

        private void OnPressJump()
        {
            PlayerCharacter.SetState(Define.Actor.ActorState.Jump);
        }

        private void OnPressMouseLeft()
        {
            // 마우스가 가르키는 객체(몬스터)의 정보가 존재한다면
            if (pointingTarget != null)
            {
                // y축 회전만 실행하고, 나머지 축은 기존 회전값을 보존한채로 몬스터쪽으로 회전
                var originRot = PlayerCharacter.transform.eulerAngles;
                // 플레이어가 타겟을 바라보게
                PlayerCharacter.transform.LookAt(pointingTarget);
                // 변경된 x, z 축 회전 값을 원래 회전값으로 변경
                var newRot = PlayerCharacter.transform.eulerAngles;
                newRot.x = originRot.x;
                newRot.z = originRot.z;
                PlayerCharacter.transform.eulerAngles = newRot;
            }

            PlayerCharacter.SetState(Define.Actor.ActorState.Attack);
        }

        private void OnPressMouseRight()
        {
            canRot = true;
        }

        private void OnNotPressMouseRight()
        {
            canRot = false;
        }
        #endregion

        private void OnApplicationQuit()
        {
            // 종료 직전에 플레이어가 위치한 곳의 정보를 db에 저장

            var dtoStage = Dummy.DummyServer.Instance.userData.dtoStage;

            dtoStage.index = GameManager.User.boStage.sdStage.index;

            var playerPos = PlayerCharacter.transform.position;

            dtoStage.posX = playerPos.x;
            dtoStage.posY = playerPos.y;
            dtoStage.posZ = playerPos.z;

            Dummy.DummyServer.Instance.Save();
        }

        private class InputHandler
        {
            /// <summary>
            /// 버튼 타입의 키 입력 시 실행할 메서드를 대리할 델리게이트
            /// </summary>
            public delegate void InputButtonDel();
            /// <summary>
            /// 축 타입의 키 입력시 실행할 메서드를 대리할 델리게이트
            /// </summary>
            /// <param name="value">축 값</param>
            public delegate void InputAxisDel(float value);

            public class AxisHandler
            {
                private InputAxisDel axisDel;

                public AxisHandler(InputAxisDel axisDel)
                {
                    this.axisDel = axisDel;
                }

                public void GetAxisValue(float value)
                {
                    axisDel.Invoke(value);
                }
            }

            public class ButtonHandler
            {
                private InputButtonDel pressDel;
                private InputButtonDel notPressDel;

                public ButtonHandler(InputButtonDel pressDel, InputButtonDel notPressDel)
                {
                    this.pressDel = pressDel;
                    this.notPressDel = notPressDel;
                }

                public void OnPress()
                {
                    pressDel?.Invoke();
                }

                public void OnNotPress()
                {
                    notPressDel?.Invoke();
                }
            }
        }
    }
}
