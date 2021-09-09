using System.Linq;
using UnityEngine;

namespace ProjectW.Object
{
    public class Warp : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
                return;

            var warpStageIndex = int.Parse(transform.parent.name);
            var user = GameManager.User;

            // 스테이지 이동을 할꺼니까 이전 스테이지 인덱스에 현재 스테이지 인덱스를 넣는다.
            user.boStage.prevStageIndex = user.boStage.sdStage.index;
            // 새로 이동하는 스테이지의 기획 데이터를 받아온다.
            user.boStage.sdStage = GameManager.SD.sdStages.Where(_ => _.index == warpStageIndex).SingleOrDefault();

            var stageManager = StageManager.Instance;
            
            // LoadScene과 비슷한 역할..
            // 실제 씬을 변경하진 않음
            // 유저에게 씬을 변경하는 것처럼 로딩씬을 추가해서 잠시 메인화면을 로딩씬으로 보여준 후
            // 이전 스테이지에서 사용하던 리소스를 해제한 후 현재 스테이지 리소스를 불러온 후 필요한 객체를 인스턴스한다.
            GameManager.Instance.OnAddtiveLoadingScene(stageManager.ChangeStage(), stageManager.OnChangeStageComplete);
        }
    }
}