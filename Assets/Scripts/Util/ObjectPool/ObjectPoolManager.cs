using ProjectW.Define;
using ProjectW.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectW.Util
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        /// <summary>
        /// 모든 풀을 보관할 딕셔너리
        /// ObjectPool<Character>, ObjectPool<Monster> 등의 서로 다른 타입의 풀을 편리하게 보관하기위해
        /// object 타입으로 박싱하여 관리한다.. 대신 성능면에서 좋은 방법은 아니겠죠..
        /// </summary>
        private Dictionary<PoolType, object> poolDic = new Dictionary<PoolType, object>();

        /// <summary>
        /// 풀 딕셔너리에 새로운 풀을 생성하여 등록하는 기능
        /// </summary>
        /// <typeparam name="T">생성할 풀의 타입</typeparam>
        /// <param name="type">딕셔너리에 사용할 풀의 키 값</param>
        /// <param name="obj">풀의 poolableObject를 생성하여 등록하기 위한 원형객체(프리팹)</param>
        /// <param name="poolCount">초기에 생성할 poolableObject의 수</param>
        public void RegistPool<T>(PoolType type, T obj, int poolCount = 1)
            where T : MonoBehaviour, IPoolableObject
        {
            ObjectPool<T> pool = null;

            // 동일한 키의 풀이 존재하는지 검사
            if (poolDic.ContainsKey(type))
                pool = poolDic[type] as ObjectPool<T>;
            // 해당 키 값의 풀이 등록되어있지 않다면 생성하여 딕셔너리에 추가
            else
            {
                pool = new ObjectPool<T>();
                poolDic.Add(type, pool);
            }

            // 풀에 홀더가 있는지 체크하여 없으면 생성
            if (pool.holder == null)
            {
                // 사용하고자하는 풀의 타입 이름으로 홀더를 생성
                pool.holder = new GameObject($"{type.ToString()}Holder").transform;
                // 홀더의 부모 객체를 오브젝트 풀 매니저로 설정
                pool.holder.parent = transform;
                pool.holder.position = Vector3.zero;
            }

            // 풀이 가지고 있을 poolableObject를 poolCount만큼 생성
            for (int i = 0; i < poolCount; ++i)
            {
                var poolableObj = Instantiate(obj);
                poolableObj.name = obj.name;
                poolableObj.transform.SetParent(pool.holder);
                poolableObj.gameObject.SetActive(false);

                pool.RegistPoolableObject(poolableObj);
            }
        }

        /// <summary>
        /// poolDic에 등록된 풀을 찾아서 반환
        /// </summary>
        /// <param name="type">찾고자 하는 풀의 키 값</param>
        /// <typeparam name="T">찾고자 하는 풀의 타입</typeparam>
        /// <returns>찾고자하는 풀</returns>
        public ObjectPool<T> GetPool<T>(PoolType type) where T : MonoBehaviour, IPoolableObject
        {
            // 딕셔너리에 해당 키가 존재하는지 검사
            if (!poolDic.ContainsKey(type))
                return null;

            return poolDic[type] as ObjectPool<T>;
        }

        /// <summary>
        /// 특정 풀을 클리어하는 기능 
        /// </summary>
        /// <typeparam name="T">클리어하고자 하는 풀의 타입</typeparam>
        /// <param name="type">클리어하고자 하는 풀의 키 값</param>
        public void ClearPool<T>(PoolType type) where T : MonoBehaviour, IPoolableObject
        {
            // 오브젝트 풀은 객체를 파괴하지않고 재사용한다고 하지 않았나요???
            // 클리어 풀은 언제 사용되는지..
            // 특정 씬에서 들고 있을 필요가 없는 풀이 발생했을 때, 해당 풀을 비우기 위해

            var pool = GetPool<T>(type)?.Pool;

            // 풀이 없다면 리턴
            if (pool == null)
                return;

            // 있다면 풀 안에 있는 풀러블 객체를 전부 파괴한다.
            for (int i = 0; i < pool.Count; ++i)
            {
                if (pool[i] != null)
                    Destroy(pool[i].gameObject);
            }

            pool.Clear();
        }
    }
}
