using ProjectW.Define;
using ProjectW.UI;
using ProjectW.Util;
using System;
using UnityEngine;
using UnityEngine.U2D;

namespace ProjectW.Resource
{
    /// <summary>
    /// ��Ÿ��(����ð�)�� �ʿ��� ���ҽ��� �ҷ����� ����� ����� Ŭ���� 
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager>
    {
        public void Initialize()
        {
            LoadAllAtlas();
            LoadAllPrefabs();
        }

        /// <summary>
        /// Resources ���� ���� �⺻���� �������� �ҷ��� ��ȯ�ϴ� ���
        /// </summary>
        /// <param name="path">Resources ���� �� �ҷ��� ������ ���</param>
        /// <returns>�ҷ��� ������ ���ӿ�����Ʈ</returns>
        public GameObject LoadObject(string path)
        {
            // Resources.Load, Assets ���� �� Resources ��� �̸��� ������ �����Ѵٸ�
            // �ش� ��κ��� path�� ����, �ش� ��ο� ������ GameObject ���·� �θ� �� �ִٸ� �ҷ���
            return Resources.Load<GameObject>(path);
        }

        /// <summary>
        /// ������Ʈ Ǯ�� ����� �������� �ε��ϴ� ���
        /// </summary>
        /// <typeparam name="T">�ε��ϰ����ϴ� �������� ���� Ÿ��</typeparam>
        /// <param name="poolType">Ǯ�� ��Ͻ�Ű���� �ϴ� ������ ��ü�� ���� Ǯ���� ������Ʈ�� Ÿ��</param>
        /// <param name="path">������ ���</param>
        /// <param name="poolCount">������Ű���� �ϴ� poolable ��ü�� ��</param>
        /// <param name="loadComplete">�������� �ε��ϰ� ������Ʈ Ǯ�� ��� �� �����ų �̺�Ʈ</param>
        public void LoadPoolableObject<T>(PoolType poolType, string path, int poolCount = 1,
            Action loadComplete = null) where T : MonoBehaviour, IPoolableObject
        {
            // �������� �ε��Ѵ�.
            var obj = LoadObject(path);
            // �������� ������Ʈ�� ��� �ִ� TŸ�� ������ �����´�
            var tComponent = obj.GetComponent<T>();

            // tŸ���� Ǯ�� ���
            ObjectPoolManager.Instance.RegistPool<T>(poolType, tComponent, poolCount);

            // ���� �۾��� ��� ���� �� ���� ��ų ������ �ִٸ� ����
            loadComplete?.Invoke();
        }

        /// <summary>
        /// Resources ���� ���� ��� ��Ʋ�󽺸� �ҷ��� ��������Ʈ �δ��� ��Ͻ�Ų��.
        /// </summary>
        private void LoadAllAtlas()
        {
            var atlases = Resources.LoadAll<SpriteAtlas>("Sprite");
            SpriteLoader.SetAtlas(atlases);
        }

        /// <summary>
        /// �ΰ��ӿ��� ����� ��� �������� �θ��� ���
        /// </summary>
        private void LoadAllPrefabs()
        {
            LoadPoolableObject<HpBar>(PoolType.HpBar, $"Prefabs/UI/HpBar", 10);
            LoadPoolableObject<Object.Item>(PoolType.Item, $"Prefabs/Item", 10);
            LoadPoolableObject<DialogueButton>(PoolType.DialogueButton, $"Prefabs/UI/DialogueButton", 5);
            LoadPoolableObject<QuestSlot>(PoolType.QuestSlot, $"Prefabs/UI/QuestSlot", 10);
        }

    }
}
