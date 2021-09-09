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
        /// ���̼������� ���� ���� ������ (���Ӽ��������� ���� DB��� �����ϸ� ��)
        /// </summary>
        public UserDataSo userData;
        public INetworkClient dummyModule;

        public void Initialize()
        {
            dummyModule = new ServerModuleDummy(this);
        }

        /// <summary>
        /// ���� ���� �����͸� �����ϴ� ���
        /// ������ ��� �Ŀ� UserDataSo�� ��� �����͸� �����ϰ� �ش� ������ ����
        /// (�����Ϳ����� ��� ����)
        /// </summary>
        public void Save()
        {
            StartCoroutine(OnSaveProgress());

            // �ڷ�ƾ���� �����͸� �����ϴ� ����?
            // ���� �����͸� ���� �������� �Ϲ����� �޼���� ������ ���� ��
            // ����ȭ ���¿����� �����͸� ���� ������ �����Ͱ� Ŭ ���� �ð��� �����ɸ�..
            // �׷� �� ��, �������� �Ͻ������� �������� ȭ���� ��� ���ߴ� ��ó�� ����..
            // ����, �����͸� ���� ������ ���⿡�� �񵿱�� �����ϱ� ���� 
            // �ڷ�ƾ���� ó���Ѵ�. (���� �񵿱�� �ƴ�.. �ڷ�ƾ�̹Ƿ� �񵿱�ó�� �۵��ϰ� �ϴ°�..)
            IEnumerator OnSaveProgress()
            {
                // �����ų ���������͸� ��Ƽ �÷��׷� �����Ѵ�.
                // ��Ƽ?
                // ����Ƽ���� ��Ÿ�� �� (������ �Ǵ� ��ũ���ͺ� ������Ʈ)���� ���Ǵ� �����͵���
                // �Ϲ������� �ֹ߼� ������(Volatile Data)
                // ������ ��Ÿ�� �� ���Ǵ� �����͸� �����ϰ� ���� �� ��ũ�� �� �� �ְ�
                // ��Ƽ �÷��׸� �����ϸ� ��..
                EditorUtility.SetDirty(userData);
                AssetDatabase.SaveAssets();

                yield return null;
            }
        }
    }
}
