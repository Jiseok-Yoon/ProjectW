using System.Linq;
using UnityEngine;
using ProjectW.DB;
using ProjectW.UI;

namespace ProjectW.Object
{
    public class NPC : MonoBehaviour
    {
        public BoNPC boNPC;

        private Collider coll;

        public void Initialize(BoNPC boNPC)
        {
            this.boNPC = boNPC;

            var stagePos = boNPC.sdNPC.stagePos;

            // 객체 이름을 설정해둔 프리팹 이름으로 변경
            gameObject.name = boNPC.sdNPC.resourcePath.Remove(0, boNPC.sdNPC.resourcePath.LastIndexOf('/') + 1);

            // 그 외 위치,회전 값을 설정
            transform.position = new Vector3(stagePos[0], stagePos[1], stagePos[2]);
            transform.eulerAngles = new Vector3(stagePos[3], stagePos[4], stagePos[5]);

            coll ??= GetComponent<Collider>();
        }

        public void NPCUpdate()
        {
            CheckInteraction();
        }

        /// <summary>
        /// 플레이어가 NPC에 접촉한 상태로 상호작용 키를 눌렀을 때
        /// 다이얼로그가 활성화되는 기능
        /// </summary>
        public void OnDialogue()
        {
            boNPC.isInteraction = true;

            // UI 다이얼로그의 참조를 가져옴
            var uiDialogue = UIWindowManager.Instance.GetWindow<UIDialogue>();

            // 다이얼로그를 세팅 (해당 NPC가 가진 이름과 기본 대화로)
            // BoDialogue를 생성하여 해당 정보를 uiDialogue에 세팅

            var boDialogue = new BoDialogue();
            boDialogue.speaker = boNPC.sdNPC.name;

            // 기본 대화 중 하나를 랜덤하게 선택
            var randIndex = Random.Range(0, boNPC.sdNPC.speechRef.Length);

            var speechRef = boNPC.sdNPC.speechRef[randIndex];
            var speech = GameManager.SD.sdString.Where(_ => _.index == speechRef).SingleOrDefault()?.kr;

            // 랜덤하게 뽑은 대사에 대사를 특정문자를 이용하여 여러 개로 나눴을 경우
            // 해당 특정문자로 문자열을 나눈다.
            boDialogue.speeches = speech.Contains("/") ? speech.Split('/') : new string[] { speech };

            // 설정된 다이얼로그 데이터를 UI 다이얼로그에 적용
            uiDialogue.SetDialogue(boDialogue);
        }

        /// <summary>
        /// NPC의 인터렉션 영역을 설정하고, 해당 영역에 플레이어가 들어왔는지 확인한 뒤
        /// 들어왔다면, 상호작용 키를 눌렀는지 확인하여 눌렀다면 대화창을 활성화하는 기능
        /// </summary>
        private void CheckInteraction()
        {
            var colls = Physics.OverlapBox(coll.bounds.center, coll.bounds.extents, transform.rotation, 1 << LayerMask.NameToLayer("Player"));

            // colls의 길이가 0이라면 플레이어가 인터렉션 영역에 없다는 뜻
            if (colls.Length == 0)
            {
                // 해당 npc가 이미 상호작용 중이라면 (상호작용 중이었는데 플레이어가 영역을 벗어낫음)
                if (boNPC.isInteraction)
                { 
                    // 영역을 벗어났으므로 상호작용 플래그를 끄고, 다이얼로그를 종료시킴
                    boNPC.isInteraction = false;
                    UIWindowManager.Instance.GetWindow<UIDialogue>().EndDialogue();
                }

                return;
            }

            // 아래의 코드가 호출된다는 것은 영역에 플레이어가 존재한다는 뜻
            // E(상호작용 키)를 눌렀고, 현재 npc의 상호작용 플래그가 꺼져있을 때만
            // 다이얼로그를 활성화 시킴
            if (Input.GetKeyDown(KeyCode.E) && !boNPC.isInteraction)
                OnDialogue();
        }
    }
}
