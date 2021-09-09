using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectW.Util
{
    /// <summary>
    /// 오브젝트 풀링을 수행할 클래스
    /// </summary>
    /// <typeparam name="T">모노를 갖고, IPoolableObject를 구현하는 타입만</typeparam>
    public class ObjectPool<T> where T : MonoBehaviour, IPoolableObject
    {
        /// <summary>
        /// 오브젝트 풀 리스트
        /// </summary>
        public List<T> Pool { get; private set; } = new List<T>();

        /// <summary>
        /// 해당 풀의 객체들의 인스턴스를 하이라키 상에서 들고 있을 부모 객체의 트랜스폼
        /// (하이라키 상에서 보기좋게 정리용으로 사용할 홀더..)
        /// </summary>
        public Transform holder;

        /// <summary>
        /// 풀에서 재사용한 가능한 객체가 존재하는지 
        /// </summary>
        /// Predicate -> Func<T, bool>
        public bool canRecycle => Pool.Find(_ => _.CanRecycle) != null;

        /// <summary>
        /// 풀링할 새로운 오브젝트를 등록
        /// </summary>
        /// <param name="obj">풀에 등록할 객체</param>
        public void RegistPoolableObject(T obj)
        {
            Pool.Add(obj);
        }
        
        /// <summary>
        /// 객체를 다시 풀에 반환
        /// (정확히는 객체는 항상 똑같은 풀에 있고, 풀에서 객체를 꺼낸다고 표현을 하고 있지만
        /// 실제로 이루어지는 작업은, 재사용 가능한 객체를 찾아서 활성/비활성화 && 객체의 부모 변경)
        ///  풀에 있을 경우 -> 객체의 부모는 오브젝트 풀의 홀더 필드
        ///  풀에서 꺼냈을 경우 -> 상황에 따라 다른 부모를 가짐 
        /// </summary>
        /// <param name="obj"></param>
        public void ReturnPoolableObject(T obj)
        {
            obj.transform.SetParent(holder);
            obj.gameObject.SetActive(false);
            obj.CanRecycle = true;
        }

        /// <summary>
        /// 재사용 가능한 풀 내의 객체를 반환
        /// </summary>
        /// <returns></returns>
        public T GetPoolableObject()
        {
            return GetPoolableObject(_ => _.CanRecycle);
        }

        /// <summary>
        /// 재사용 가능한 풀 내의 객체를 반환
        /// </summary>
        /// <param name="pred">풀 내에서 재사용 객체를 특정 조건으로 검사할 predicate</param>
        /// <returns>재사용할 객체</returns>
        public T GetPoolableObject(Func<T, bool> pred)
        {
            // 풀 내에서 재사용 가능한 객체가 존재하는지 검사
            if (!canRecycle)
            {
                // 재사용 가능한 객체가 없을 경우 들어옴

                // 풀에서 조건과 동일한 객체가 있을 경우 반환, 없다면 null을 반환
                var protoObj = Pool.Find(_ => pred(_));

                // 원형객체가 있다면
                if (protoObj != null)
                {
                    // 해당 원형객체를 통해 새로운 객체를 생성
                    var newObj = GameObject.Instantiate(protoObj.gameObject, holder);
                    newObj.name = protoObj.name;
                    newObj.SetActive(false);
                    // 새로 생성한 객체를 풀에 등록
                    RegistPoolableObject(newObj.GetComponent<T>());
                }
                else
                    return null;
            }

            // 파라미터로 받은 조건에 해당하는 객체가 존재하는지 검사
            T recycleObj = Pool.Find(_ => pred(_) && _.CanRecycle);

            if (recycleObj == null)
                return null;

            // 해당 객체를 반환해서 재사용할 것이므로, 더 이상 재사용할수 없게 재사용 불가능 상태로 변경
            recycleObj.CanRecycle = false;

            return recycleObj;
        }
    }
}
