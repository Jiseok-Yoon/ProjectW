using ProjectW.DB;
using ProjectW.SD;
using System.Collections.Generic;
using System.Linq;

namespace ProjectW.Network
{
    /// <summary>
    /// 로그인 시 필요한 데이터들을 서버에 요청하는 기능을 하는 클래스 
    /// </summary>
    public class LoginHandler
    {
        /// <summary>
        /// 리스폰스 핸들러를 통해 데이터를 받아 처리한다.
        /// </summary>
        public ResponsHandler<DtoAccount> accountHandler;
        public ResponsHandler<DtoCharacter> characterHandler;
        public ResponsHandler<DtoStage> stageHandler;
        public ResponsHandler<DtoItem> itemHandler;

        public LoginHandler()
        {
            accountHandler = new ResponsHandler<DtoAccount>(GetAccountSuccess, OnFailed);
            characterHandler = new ResponsHandler<DtoCharacter>(GetCharacterSuccess, OnFailed);
            stageHandler = new ResponsHandler<DtoStage>(GetStageSuccess, OnFailed);
            itemHandler = new ResponsHandler<DtoItem>(GetItemSuccess, OnFailed);
        }

        public void Connect()
        {
            ServerManager.Server.Login(0, accountHandler);
        }

        /// <summary>
        /// 계정 정보 요청 성공 시 실행할 메서드
        /// </summary>
        /// <param name="dtoAccount">서버에서 보내준 계정 정보</param>
        public void GetAccountSuccess(DtoAccount dtoAccount)
        {
            // 서버에서 받은 dto 데이터를 bo 데이터로 변환 후 게임 매니저의 
            // 모든 bo데이터 관리 객체가 들고 있게 한다.
            GameManager.User.boAccount = new BoAccount(dtoAccount);

            // 다음으로 스테이지 정보를 요청한다.
            ServerManager.Server.GetStage(0, stageHandler);
        }

        /// <summary>
        /// 스테이지 정보 요청 성공 시 실행할 메서드
        /// </summary>
        /// <param name="dtoStage">서버에서 보내준 스테이지 정보</param>
        public void GetStageSuccess(DtoStage dtoStage)
        { 
            GameManager.User.boStage = new BoStage(dtoStage);

            ServerManager.Server.GetItem(0, itemHandler);
        }

        /// <summary>
        /// 캐릭터 정보 요청 성공 시 실행할 메서드
        /// </summary>
        /// <param name="dtoCharacter">서버에서 보내준 캐릭터 정보</param>
        public void GetCharacterSuccess(DtoCharacter dtoCharacter)
        {
            GameManager.User.boCharacter = new BoCharacter(dtoCharacter);

            OnLoginFinished();
        }

        /// <summary>
        /// 아이템 정보 요청 성공 시 실행할 메서드
        /// </summary>
        /// <param name="dtoItem">서버에서 보내준 전체 아이템 정보</param>
        public void GetItemSuccess(DtoItem dtoItem)
        {
            GameManager.User.boItems = new List<BoItem>();
            var boItems = GameManager.User.boItems;

            for (int i = 0; i < dtoItem.items.Count; ++i)
            {
                var dtoItemElement = dtoItem.items[i];
                BoItem boItem = null;

                var sdItem = GameManager.SD.sdItems.Where(_ => _.index == dtoItemElement.index).SingleOrDefault();

                if (sdItem.itemType == Define.Item.ItemType.Equipment)
                {
                    boItem = new BoEquipment(sdItem);
                    var boEquipment = boItem as BoEquipment;
                    boEquipment.reinforceValue = dtoItemElement.reinforceValue;
                    boEquipment.isEquip = dtoItemElement.isEquip;
                }
                else
                {
                    boItem = new BoItem(sdItem);
                }

                SetItem(boItem, dtoItemElement);

                boItems.Add(boItem);
            }

            ServerManager.Server.GetCharacter(0, characterHandler);

            void SetItem(BoItem boItem, DtoItemElement dtoItemElement) 
            {
                boItem.slotIndex = dtoItemElement.slotIndex;
                boItem.amount = dtoItemElement.amount;
            }
        }

        /// <summary>
        /// 모든 로그인 절차가 끝난 후 실행할 메서드
        /// </summary>
        private void OnLoginFinished()
        {
            // 모노를 갖지 않는 클래스에서 FindOfType 같은 메서드를 사용하고 싶다면?
            // 모노를 갖는 객체로 접근하여 해당 메서드를 사용하면 됌.
            var titleController = GameManager.FindObjectOfType<TitleController>();
            
            if (titleController == null)
                return;
            
            titleController.LoadComplete = true;
        }

        /// <summary>
        /// 서버에 특정 요청 실패 시 실행될 메서드
        /// </summary>
        /// <param name="dtoBase">에러 코드 및 메세지</param>
        public void OnFailed(DtoBase dtoBase)
        { 
            
        }
    }
}
