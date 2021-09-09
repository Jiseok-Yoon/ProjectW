using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static ProjectW.Define.Resource;

namespace ProjectW.Resource
{
    /// <summary>
    /// ���ӿ� ���Ǵ� ��� ��Ʋ�󽺸� �����ϴ� Ŭ���� 
    /// ��Ʋ�󽺶�?
    /// ���� ��������Ʈ�� �ϳ��� �ؽ��ķ� ����� ����ϴ� �� (�޸� ����ȭ)
    /// ��Ÿ�ӿ� �����Ǵ� ��������Ʈ�� ��� �ش� Ŭ������ ���� �����´�.
    /// </summary>
    public static class SpriteLoader
    {
        // ���� ��Ʋ�󽺸� �з��ϴ� ���?
        // ������ �Ը� �Ǵ� �帣�� ���� �޶��� �� ����
        // �Ϲ������� �� ������ ����, ���� ������ ���Ǵ� ��������Ʈ���� ���� �з�

        /// <summary>
        /// ��� ��Ʋ�󽺵��� ������ ��ųʸ�
        /// </summary>
        private static Dictionary<AtlasType, SpriteAtlas> atlasDic = new Dictionary<AtlasType, SpriteAtlas>();

        /// <summary>
        /// �Ű������� ���� ��Ʋ�� ����� ��Ʋ�󽺵��� ��ųʸ��� ���
        /// </summary>
        /// <param name="atlases">����ϰ��� �ϴ� ��Ʋ�� ���</param>
        public static void SetAtlas(SpriteAtlas[] atlases)
        {
            for (int i = 0; i < atlases.Length; ++i)
            {
                var key = (AtlasType)Enum.Parse(typeof(AtlasType), atlases[i].name);

                atlasDic.Add(key, atlases[i]);
            }
        }

        /// <summary>
        /// Ư�� ��Ʋ�󽺿��� ���ϴ� ��������Ʈ�� ã�Ƽ� ��ȯ
        /// </summary>
        /// <param name="type">ã���� �ϴ� ��������Ʈ�� ����ִ� ��Ʋ���� Ű ��</param>
        /// <param name="spriteKey">ã���� �ϴ� ��������Ʈ�� �̸�</param>
        /// <returns></returns>
        public static Sprite GetSprite(AtlasType type, string spriteKey)
        {
            // ��Ʋ�� Ű�� ��ųʸ��� �������� �ʴ´ٸ� ����
            if (!atlasDic.ContainsKey(type))
                return null;

            return atlasDic[type].GetSprite(spriteKey);
        }
    }
}
