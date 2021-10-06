using ProjectW.Resource;
using ProjectW.SD;
using ProjectW.Util;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ProjectW.Define.Dialogue;
using static ProjectW.Define.Resource;

namespace ProjectW.UI
{
    public class DialogueButton : MonoBehaviour, IPoolableObject
    {
        public TextMeshProUGUI title;
        public Image icon;
        public Button btn;

        public bool CanRecycle { get; set; } = true;

        public void Initialize(DialogueButtonType type, params int[] datas)
        {
            // 버튼을 초기화 시, 변경되어야할 데이터가 무엇인가?
            //  -> 버튼의 종류에 따라 아이콘 이미지가 변경되어야 함
            //  -> 버튼의 종류 및 데이터에 따라 텍스트가 변경되어야 함
            //  -> 버튼의 종류에 따라 버튼에 바인딩하는 이벤트도 변경되어야 함

            var spriteKey = string.Empty;
            var text = string.Empty;

            switch (type)
            {
                case DialogueButtonType.Shop:
                    spriteKey = "gem";
                    text = "상  점";
                    btn.onClick.AddListener(OnClickShop);
                    break;
                case DialogueButtonType.Quest:
                    spriteKey = "exclamation_mark";
                    var sdQuest = GameManager.SD.sdQuests.Where(_ => _.index == datas[0]).SingleOrDefault();
                    text = sdQuest.name;
                    btn.onClick.AddListener(() => { OnClickQuest(sdQuest); });
                    break;
            }

            icon.sprite = SpriteLoader.GetSprite(AtlasType.IconAtlas, spriteKey);
            title.text = text;
        }

        private void OnClickShop()
        { 
        
        }

        /// <summary>
        /// 퀘스트 타입의 버튼 클릭시 실행될 메서드
        /// </summary>
        /// <param name="sdQuest">버튼 클릭 시 퀘스트 수주창에 해당 퀘스트 정보를 넘겨야하므로 퀘스트 데이터를 받는다</param>
        private void OnClickQuest(SDQuest sdQuest)
        {
            var uiWindowManager = UIWindowManager.Instance;

            // 이 시점에서 가장 최상위의 창 -> UIDialogue
            // 다이얼로그 UI 끄고, 퀘스트 창을 켠다.
            uiWindowManager.GetTopWindow().Close();
            UIWindowManager.Instance.GetWindow<UIQuest>().Open(Define.Quest.QuestWindow.Order, sdQuest);
        }
    }
}
