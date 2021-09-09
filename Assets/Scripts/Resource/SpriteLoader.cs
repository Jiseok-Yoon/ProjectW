using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static ProjectW.Define.Resource;

namespace ProjectW.Resource
{
    /// <summary>
    /// 게임에 사용되는 모든 아틀라스를 관리하는 클래스 
    /// 아틀라스란?
    /// 여러 스프라이트를 하나의 텍스쳐로 만들어 사용하는 것 (메모리 최적화)
    /// 런타임에 생성되는 스프라이트는 모두 해당 클래스를 통해 가져온다.
    /// </summary>
    public static class SpriteLoader
    {
        // 보통 아틀라스를 분류하는 방법?
        // 게임의 규모 또는 장르에 따라 달라질 수 있음
        // 일반적으로 씬 단위로 관리, 여러 씬에서 사용되는 스프라이트들은 따로 분류

        /// <summary>
        /// 모든 아틀라스들을 관리할 딕셔너리
        /// </summary>
        private static Dictionary<AtlasType, SpriteAtlas> atlasDic = new Dictionary<AtlasType, SpriteAtlas>();

        /// <summary>
        /// 매개변수로 받은 아틀라스 목록의 아틀라스들을 딕셔너리에 등록
        /// </summary>
        /// <param name="atlases">등록하고자 하는 아틀라스 목록</param>
        public static void SetAtlas(SpriteAtlas[] atlases)
        {
            for (int i = 0; i < atlases.Length; ++i)
            {
                var key = (AtlasType)Enum.Parse(typeof(AtlasType), atlases[i].name);

                atlasDic.Add(key, atlases[i]);
            }
        }

        /// <summary>
        /// 특정 아틀라스에서 원하는 스프라이트를 찾아서 반환
        /// </summary>
        /// <param name="type">찾고자 하는 스프라이트가 들어있는 아틀라스의 키 값</param>
        /// <param name="spriteKey">찾고자 하는 스프라이트의 이름</param>
        /// <returns></returns>
        public static Sprite GetSprite(AtlasType type, string spriteKey)
        {
            // 아틀라스 키가 딕셔너리에 존재하지 않는다면 리턴
            if (!atlasDic.ContainsKey(type))
                return null;

            return atlasDic[type].GetSprite(spriteKey);
        }
    }
}
