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

            // �������� �̵��� �Ҳ��ϱ� ���� �������� �ε����� ���� �������� �ε����� �ִ´�.
            user.boStage.prevStageIndex = user.boStage.sdStage.index;
            // ���� �̵��ϴ� ���������� ��ȹ �����͸� �޾ƿ´�.
            user.boStage.sdStage = GameManager.SD.sdStages.Where(_ => _.index == warpStageIndex).SingleOrDefault();

            var stageManager = StageManager.Instance;
            
            // LoadScene�� ����� ����..
            // ���� ���� �������� ����
            // �������� ���� �����ϴ� ��ó�� �ε����� �߰��ؼ� ��� ����ȭ���� �ε������� ������ ��
            // ���� ������������ ����ϴ� ���ҽ��� ������ �� ���� �������� ���ҽ��� �ҷ��� �� �ʿ��� ��ü�� �ν��Ͻ��Ѵ�.
            GameManager.Instance.OnAddtiveLoadingScene(stageManager.ChangeStage(), stageManager.OnChangeStageComplete);
        }
    }
}