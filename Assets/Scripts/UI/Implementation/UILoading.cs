using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ProjectW.UI
{
    /// <summary>
    /// 로딩씬의 UI 컨트롤
    /// </summary>
    public class UILoading : UIWindow
    {
        private string dot = string.Empty;
        private const string loadStateDescription = "Load Next Scene";

        public TextMeshProUGUI loadStateDesc;
        public Image loadGauge;

        /// <summary>
        /// 로딩 씬의 카메라
        /// </summary>
        public Camera cam;

        private void Update()
        {
            loadGauge.fillAmount = GameManager.Instance.loadProgress;

            // 20 프레임 마다
            if (Time.frameCount % 20 == 0)
            {
                if (dot.Length >= 3)
                    dot = string.Empty;
                else
                    dot = string.Concat(dot, ".");

                loadStateDesc.text = $"{loadStateDescription}{dot}";
            }
        }
    }
}
