using ProjectW.Define;
using ProjectW.UI;
using ProjectW.Util;
using System;
using UnityEngine;
using UnityEngine.U2D;

namespace ProjectW.Resource
{
    /// <summary>
    /// 런타임(실행시간)에 필요한 리소스를 불러오는 기능을 담당할 클래스 
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager>
    {
        public void Initialize()
        {
            LoadAllAtlas();
            LoadAllPrefabs();
        }

        /// <summary>
        /// Resources 폴더 내의 기본적인 프리팹을 불러와 반환하는 기능
        /// </summary>
        /// <param name="path">Resources 폴더 내 불러올 에셋의 경로</param>
        /// <returns>불러온 프리팹 게임오브젝트</returns>
        public GameObject LoadObject(string path)
        {
            // Resources.Load, Assets 폴더 내 Resources 라는 이름의 폴더가 존재한다면
            // 해당 경로부터 path를 읽음, 해당 경로에 파일이 GameObject 형태로 부를 수 있다면 불러옴
            return Resources.Load<GameObject>(path);
        }

        /// <summary>
        /// 오브젝트 풀로 사용할 프리팹을 로드하는 기능
        /// </summary>
        /// <typeparam name="T">로드하고자하는 프리팹이 갖는 타입</typeparam>
        /// <param name="poolType">풀에 등록시키고자 하는 프리팹 객체가 갖는 풀러블 컴포넌트의 타입</param>
        /// <param name="path">프리팹 경로</param>
        /// <param name="poolCount">생성시키고자 하는 poolable 객체의 수</param>
        /// <param name="loadComplete">프리팹을 로드하고 오브젝트 풀에 등록 후 실행시킬 이벤트</param>
        public void LoadPoolableObject<T>(PoolType poolType, string path, int poolCount = 1,
            Action loadComplete = null) where T : MonoBehaviour, IPoolableObject
        {
            // 프리팹을 로드한다.
            var obj = LoadObject(path);
            // 프리팹이 컴포넌트로 들고 있는 T타입 참조를 가져온다
            var tComponent = obj.GetComponent<T>();

            // t타입의 풀을 등록
            ObjectPoolManager.Instance.RegistPool<T>(poolType, tComponent, poolCount);

            // 위의 작업이 모두 끝난 후 실행 시킬 로직이 있다면 실행
            loadComplete?.Invoke();
        }

        /// <summary>
        /// Resources 폴더 내의 모든 아틀라스를 불러와 스프라이트 로더에 등록시킨다.
        /// </summary>
        private void LoadAllAtlas()
        {
            var atlases = Resources.LoadAll<SpriteAtlas>("Sprite");
            SpriteLoader.SetAtlas(atlases);
        }

        /// <summary>
        /// 인게임에서 사용할 모든 프리팹을 부르는 기능
        /// </summary>
        private void LoadAllPrefabs()
        {
            LoadPoolableObject<HpBar>(PoolType.HpBar, $"Prefabs/UI/HpBar", 10);
            LoadPoolableObject<Object.Item>(PoolType.Item, $"Prefabs/Item", 10);
        }

    }
}
