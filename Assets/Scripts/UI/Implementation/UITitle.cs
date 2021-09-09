using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectW.UI
{
    public class UITitle : MonoBehaviour
    {
        /// <summary>
        /// 현재 로딩 상태 설명
        /// </summary>
        public Text loadStateDesc;

        /// <summary>
        /// 로딩 상태 게이지
        /// </summary>
        public Image loadFillGauge;

        public void SetLoadStateDescription(string loadState)
        {
            loadStateDesc.text = $"Load {loadState.ToString()}...";
        }

        /// <summary>
        /// 로딩 바 애니메이션 처리
        /// </summary>
        /// <param name="loadPer"></param>
        /// <returns></returns>
        public IEnumerator LoadGaugeUpdate(float loadPer)
        {
            // ui의 fillAmount 값이랑 현재 로딩 퍼센테이지 값이랑 같지 않다면 반복
            while (!Mathf.Approximately(loadFillGauge.fillAmount, loadPer))
            {
                loadFillGauge.fillAmount = Mathf.Lerp(loadFillGauge.fillAmount, loadPer, Time.deltaTime * 2f);
                yield return null;
            }
        }
    }
}
