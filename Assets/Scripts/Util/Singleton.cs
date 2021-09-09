using UnityEngine;

namespace ProjectW.Util
{
    /// <summary>
    /// �̱��� ���̽� Ŭ����
    /// </summary>
    /// <typeparam name="T">Singleton<T>�� ��ӹ޴� Ÿ�� (�̱������� ������� �ϴ� �Ļ�Ŭ����)</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        /// <summary>
        /// �̱��� �ν��Ͻ��� ã�ų� �����ϴ� ���� �� �ٸ� �����忡�� ��������� �Ǵ��� ��ü
        /// </summary>
        private static object syncObject = new object();

        /// <summary>
        /// tŸ�� �ν��Ͻ� ��ü
        /// </summary>
        private static T instance;

        /// <summary>
        /// �ܺο��� �ν��Ͻ� ��ü�� �����ϱ� ���� ������Ƽ
        /// </summary>
        public static T Instance
        {
            get
            {
                // �ν��Ͻ��� ���ٸ�
                if (instance == null)
                {
                    // lock�� ���� ����ȭ ������ �Ϲ������� ����Ƽ�� �̱� ������ ȯ�濡���� ���������,
                    // ��Ƽ ������ ȯ�濡�� �̱��� �ʱ�ȭ ������ ������ �������ϰ� �ϱ� ����
                    // (�� ���� �� ,�ʱ�ȭ ������ �����ϰ� �� ���Ĵ� ������ ���������� �ʴٴ� ��)
                    // �ν��Ͻ��� ã�ų� �����ϴ� ������ ���� ���� �� �ٸ� �����忡��
                    // ������ ������ ������ �� ���� ���� �Ǵ� (��� ���̶�� ���������� ����ϰ� ��)
                    lock (syncObject)
                    {
                        // �ν��Ͻ��� ã�´�
                        instance = FindObjectOfType<T>();
                        // �˻������� �ν��Ͻ��� ���ٸ�
                        if (instance == null)
                        {
                            // �ν��Ͻ��� ���� ����
                            GameObject obj = new GameObject();
                            obj.name = typeof(T).Name;
                            instance = obj.AddComponent<T>();
                        }
                    }
                }

                // ���� ������ ���� �ν��Ͻ��� �ִٸ� �״�� ��ȯ
                // ���ٸ� ã�ų� ���� �����ؼ� ��ȯ
                return instance;
            }
        }


        protected virtual void Awake()
        {
            // ��븦 ��ӹ޾����Ƿ� ��� ��ü�� �ʱ�ȭ�� ������ ��
            // Awake�� �ڵ������� ȣ��Ǵµ� �� �� �˻縦 ���� �ν��Ͻ��� ���ٸ� �̸� �־�д�.
            // ����? Instance ������Ƽ ���� �� ���ʿ��� �˻� ������ �ִ��� �����ϱ� ����..
            if (instance == null)
            {
                instance = this as T;
            }
            // ���� �� �� �ν��Ͻ��� �����Ѵٸ� ��ü�� �ʱ�ȭ �Ǳ����� �ش� �ν��Ͻ��� �����Ͽ��ٴ� ��
            // �ش� Ŭ������ ��븦 ��ӹ޾Ұ� �Ϲ������� ��(���̶�Ű)�� �÷��� �����
            // �׷� �� �� �ν��Ͻ��� �����Ѵٴ� ���� ���� �÷��� �ν��Ͻ��� �ʱ�ȭ �Ǳ� ����
            // �ٸ� Ŭ�������� �̱������� �ش� Ŭ������ �����Ͽ� ������ �ν��Ͻ��� �߰������� �����Ǿ��ٴ� �ǹ�
            // ����, �߰��� ������ �ν��Ͻ��� �����ϰ� ���� ���� ��ġ�� �̱����� ������ �� �ֵ��� �Ѵ�.
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (instance != this)
                return;

            instance = null;
        }

        public static bool HasInstance()
        {
            return instance ? true : false;
        }
    }
}