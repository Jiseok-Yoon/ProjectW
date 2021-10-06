using System.Linq;
using UnityEngine;
using ProjectW.DB;
using ProjectW.UI;
using System.Collections.Generic;

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

            // NPC에게서 수주할 수 있는 퀘스트 목록만 추림
            //  -> 확인을 위해 유저 퀘스트 데이터를 가져옴
            var boQuest = GameManager.User.boQuest;

            // Linq Except 두 집합의 차집합을 구하는 메서드
            // (이 때, 반환되는 요소는 Except을 호출하는 집합을 기준으로 두번째 집합에 없는 요소들이 반환)

            // 유저가 이미 진행중인 퀘스트라면 제외
            // NPC의 인덱스 목록 이랑 유저가 진행중인 퀘스트 인덱스 목록
            var canOrderQuests = boNPC.sdNPC.questRef.Except(boQuest.progressQuests.Select(_ => _.sdQuest.index));
            // 유저가 이미 완료한 퀘스트라면 제외
            canOrderQuests = canOrderQuests.Except(boQuest.completedQuests.Select(_ => _.index));

            // orderQuests -> 유저가 진행중이 아니고, 완료하지 않은 퀘스트 인덱스만 남아있음
            var orderQuests = canOrderQuests.ToList();
            var sdQuests = GameManager.SD.sdQuests;
            // 남은 퀘스트 중에 선행 퀘스트를 완료해야만 진행할 수 있는 퀘스트인가?
            // 위의 과정을 거쳐 걸려진 퀘스트들의 선행퀘스트 목록을 가져옴
            for (int i = 0; i < orderQuests.Count; ++i)
            {
                // 퀘스트 테이블에서 추려낸 퀘스트가 선행퀘스 데이터가 존재하는지, 존재한다면 선행퀘스트 인덱스 목록을 가져옴
                var antecedentQuest = sdQuests.Where(_ => _.index == orderQuests[i]).SingleOrDefault()?.antecedentQuest;

                // 선행 퀘스트 목록의 첫번째 원소의 값이 0 이라면? 선행퀘스트가 존재하지 않는다는 것
                if (antecedentQuest[0] == 0)
                    continue;

                // Linq Intersect 두 집합의 교집합을 구하는 메서드 
                // 구한 교집합의 길이가 선행퀘스트 목록의 길이와 같다면? 선행퀘스트를 전부 완료했다는 것
                if (antecedentQuest.Length !=
                    antecedentQuest.Intersect(boQuest.completedQuests.Select(_ => _.index)).Count())
                {
                    // 선행퀘스트를 전부 완료하지 않았으므로, 수주 퀘스트 목록에서 지워버린다.
                    orderQuests.RemoveAt(i);
                    --i;
                }
            }

            // 선행퀘스트 여부까지 걸러 최종적으로 수주할 수 있는 퀘스트목록을 구했으므로 boDialogue에 설정
            boDialogue.quests = orderQuests.ToArray();
            
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
                    UIWindowManager.Instance.GetWindow<UIDialogue>().Close();
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
