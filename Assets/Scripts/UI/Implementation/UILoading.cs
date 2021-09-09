using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ProjectW.UI
{
    /// <summary>
    /// �ε����� UI ��Ʈ��
    /// </summary>
    public class UILoading : UIWindow
    {
        private string dot = string.Empty;
        private const string loadStateDescription = "Load Next Scene";

        public TextMeshProUGUI loadStateDesc;
        public Image loadGauge;

        /// <summary>
        /// �ε� ���� ī�޶�
        /// </summary>
        public Camera cam;

        private void Update()
        {
            loadGauge.fillAmount = GameManager.Instance.loadProgress;

            // 20 ������ ����
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
