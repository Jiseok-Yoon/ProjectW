using ProjectW.SD;
using System;
using System.Linq;
using UnityEngine;

namespace ProjectW.DB
{
    [Serializable]
    public class BoStage
    {
        /// <summary>
        /// 다른 스테이지로 워프 시 이전 스테이지에 대한 인덱스를 받을 필드 (클라이언트에서만 사용)
        /// </summary>
        public int prevStageIndex;
        /// <summary>
        /// 플레이어가 마지막으로 위치한 좌표
        /// </summary>
        public Vector3 prevPos;
        /// <summary>
        /// 플레이어가 마지막으로 위치한 스테이지의 기획데이터 (없다면 시작마을로 설정)
        /// </summary>
        public SDStage sdStage;

        public BoStage(DtoStage dtoStage)
        {
            sdStage = GameManager.SD.sdStages.Where(_ => _.index == dtoStage.index).SingleOrDefault();
            prevPos = new Vector3(dtoStage.posX, dtoStage.posY, dtoStage.posZ);
        }
    }
}
