﻿using ProjectW.DB;
using ProjectW.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectW.Object
{
    public class NPC : MonoBehaviour
    {
        public BoNpc boNPC;
        public BoxCollider interactableArea;
        public bool isIntreacting;

        public void Start()
        {
            var bounds = GetComponentInChildren<SkinnedMeshRenderer>().bounds;
            interactableArea = gameObject.AddComponent<BoxCollider>();
            interactableArea.center = Vector3.zero + new Vector3(0, bounds.extents.y, 0);
            interactableArea.size.Set(bounds.extents.x * 2f, bounds.extents.y, bounds.extents.z * 2f);
            interactableArea.isTrigger = true;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.GetMask("Player"))
                return;
            if (isIntreacting)
                return;
            Debug.Log("hi");
            if (Input.GetKeyDown(KeyCode.E))
            {
                isIntreacting = true;
                OnDialogue();
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != LayerMask.GetMask("Player"))
                return;
            var uiDialogue = UIWindowManager.Instance.GetWindow<UIDialogue>();
            uiDialogue.EndDialogue();

        }
        public void Initialize(BoNpc boNPC)
        {
            this.boNPC = boNPC;
        }

        /// <summary>
        /// 플레이어가 NPC에 접촉한 상태로 상호작용 키를 눌렀을 때
        /// 다이얼로그가 활성화되는 기능
        /// </summary>
        public void OnDialogue()
        {
            // UI 다이얼로그의 참조를 가져옴
            var uiDialogue = UIWindowManager.Instance.GetWindow<UIDialogue>();

            // 다이얼로그를 세팅 (해당 NPC가 가진 이름과 기본 대화로)
            // BoDialogue를 생성하여 해당 정보를 uiDialogue에 세팅

            var boDialogue = new BoDialogue();
            boDialogue.speaker = boNPC.sdNPC.name;

            var speeches = new List<string>();

            // 기본 대화 중 하나를 랜덤하게 선택.
            var randIndex = Random.Range(0, boNPC.sdNPC.speechRef.Length);

            var speechRef = boNPC.sdNPC.speechRef[randIndex];
            var speech = GameManager.SD.sdString.Where(_ => _.index == boNPC.sdNPC.speechRef[randIndex]).SingleOrDefault().kr;

            // 랜덤하게 뽑은 대사에 대사를 특정문자를 이용하여 여러 개로 나눴을 경우
            // 해당 특정문자로 문자열을 나눈다.
            boDialogue.speeches = speech.Contains("/") ? speech.Split('/') : new string[] { speech };


            // 설정된 다이얼로그 데이터를 UI 다이얼로그에 적용
            uiDialogue.SetDialogue(boDialogue);
            uiDialogue.Open();

        }
    }
}
