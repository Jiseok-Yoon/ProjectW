using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectW.Object
{
    using CamView = Define.Camera.CamView;
    using Input = Define.Input;

    /// <summary>
    /// �÷��̾� ĳ������ �Է� ó��
    /// ĳ���� Ŭ�������� ó�� ���ϴ� ����?
    /// ĳ���Ϳ� �÷��̾��� �Է��� �и������ν�
    /// ĳ���� Ŭ������ �� �پ��ϰ� ����� �� �ֱ� ������ (ex : ��Ƽ ȯ�濡���� ĳ����, NPC ��)
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

            // ����� �� Ÿ���� Ű�� Ű���� ��� �� �����ų �޼��带 ��ųʸ��� ���
            inputAxisDic = new Dictionary<string, InputHandler.AxisHandler>();
            inputAxisDic.Add(Input.AxisX, new InputHandler.AxisHandler(GetAxisX)); // ĳ���� �¿� ȸ�� -> ���콺 ȸ�� (������)
            inputAxisDic.Add(Input.AxisZ, new InputHandler.AxisHandler(GetAxisZ)); // ĳ���� ��, �� �̵�
            inputAxisDic.Add(Input.MouseX, new InputHandler.AxisHandler(GetMouseX)); // ���콺 x�� ������ ĳ���� �¿� ȸ��
            inputAxisDic.Add(Input.MouseY, new InputHandler.AxisHandler(GetMouseY)); // ������

            inputButtonDic = new Dictionary<string, InputHandler.ButtonHandler>();
            inputButtonDic.Add(Input.MouseLeft, new InputHandler.ButtonHandler(OnPressMouseLeft, null));
            inputButtonDic.Add(Input.MouseRight, new InputHandler.ButtonHandler(OnPressMouseRight, OnNotPressMouseRight));
            inputButtonDic.Add(Input.Jump, new InputHandler.ButtonHandler(OnPressJump, null));
            inputButtonDic.Add(Input.FrontCam, new InputHandler.ButtonHandler(OnPressFrontCam, OnNotPressFrontCam));
        }

        private void FixedUpdate()
        {
            // �÷��̾� ĳ���Ͱ� ���õǱ� ������ �Է��� �� ���� ����ó��
            if (PlayerCharacter == null)
                return;

            // �׾��� ��� �Է��� �� ���� ����ó��
            if (PlayerCharacter.State == Define.Actor.ActorState.Dead)
            { 
                return;
            }

            InputUpdate();
            CheckMousePointTarget();
        }

        private void CheckMousePointTarget()
        {
            // ���� ������ ����ϴ� ī�޶󿡼� ��ũ�� ��ǥ���� ���콺 ��ġ���� ���� ����
            var ray = CameraController.Cam.ScreenPointToRay(UnityEngine.Input.mousePosition);

            // ������ ���̸� ���� �ش� ���� ���⿡ ���Ͱ� �����ϴ��� üũ
            var hits = Physics.RaycastAll(ray, 1000f, 1 << LayerMask.NameToLayer("Monster"));

            // ����ĳ������ ����� ��� �迭�� ���̰� 0�� �ƴ϶�� Ÿ�� ����
            HasPointTarget = hits.Length != 0;
            // ĳ���͸� Ÿ�� ������ ȸ����Ű�� ���� Ÿ���� Ʈ�������� �޴� ���̹Ƿ�
            // �׳� �� �տ� �ִ� Ÿ���� Ʈ�������� �־���..
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
                // Ű�� ������ ���� �� ����
                if (UnityEngine.Input.GetButton(input.Key))
                    input.Value.OnPress();
                else
                    // Ű�� ���� �ִٸ� ��� ȣ�� 
                    // 1. Ű�� ���� ���� �� ������� �۾��� �ʿ��� ���
                    // 2. Ű�� �� ������ �� ���� �����ϸ� �Ǵ� �۾�
                    input.Value.OnNotPress();
            }
        }

        #region Input Implementation �Է� ������
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
            // ���콺�� ����Ű�� ��ü(����)�� ������ �����Ѵٸ�
            if (pointingTarget != null)
            {
                // y�� ȸ���� �����ϰ�, ������ ���� ���� ȸ������ ������ä�� ���������� ȸ��
                var originRot = PlayerCharacter.transform.eulerAngles;
                // �÷��̾ Ÿ���� �ٶ󺸰�
                PlayerCharacter.transform.LookAt(pointingTarget);
                // ����� x, z �� ȸ�� ���� ���� ȸ�������� ����
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


        private class InputHandler
        {
            /// <summary>
            /// ��ư Ÿ���� Ű �Է� �� ������ �޼��带 �븮�� ��������Ʈ
            /// </summary>
            public delegate void InputButtonDel();
            /// <summary>
            /// �� Ÿ���� Ű �Է½� ������ �޼��带 �븮�� ��������Ʈ
            /// </summary>
            /// <param name="value">�� ��</param>
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
