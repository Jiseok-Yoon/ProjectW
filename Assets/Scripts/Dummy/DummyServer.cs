using UnityEngine;
using ProjectW.Util;
using ProjectW.Network;
using UnityEditor;
using System.Collections;

namespace ProjectW.Dummy
{
    public class DummyServer : Singleton<DummyServer>
    {
        /// <summary>
        /// 더미서버에서 갖는 유저 데이터 (더머서버에서의 유저 DB라고 생각하면 됌)
        /// </summary>
        public UserDataSo userData;
        public INetworkClient dummyModule;

        public void Initialize()
        {
            dummyModule = new ServerModuleDummy(this);
        }

        /// <summary>
        /// 더미 유저 데이터를 저장하는 기능
        /// 서버와 통신 후에 UserDataSo에 통신 데이터를 저장하고 해당 파일을 저장
        /// (에디터에서만 사용 가능)
        /// </summary>
        public void Save()
        {
            StartCoroutine(OnSaveProgress());

            // 코루틴으로 데이터를 저장하는 이유?
            // 유저 데이터를 쓰는 과정에서 일반적인 메서드로 저장을 진행 시
            // 동기화 상태에서의 데이터를 쓰는 행위가 데이터가 클 수록 시간이 오래걸림..
            // 그럼 이 때, 프레임이 일시적으로 떨어지고 화면이 잠시 멈추는 것처럼 보임..
            // 따라서, 데이터를 쓰는 행위를 동기에서 비동기로 변경하기 위해 
            // 코루틴으로 처리한다. (실제 비동기는 아님.. 코루틴이므로 비동기처럼 작동하게 하는것..)
            IEnumerator OnSaveProgress()
            {
                // 저장시킬 유저데이터를 더티 플래그로 설정한다.
                // 더티?
                // 유니티에서 런타임 시 (프리팹 또는 스크립터블 오브젝트)에서 사용되는 데이터들은
                // 일반적으로 휘발성 데이터(Volatile Data)
                // 하지만 런타임 시 사용되던 데이터를 저장하고 싶을 때 디스크에 쓸 수 있게
                // 더티 플래그를 설정하면 됌..
                EditorUtility.SetDirty(userData);
                AssetDatabase.SaveAssets();

                yield return null;
            }
        }
    }
}
