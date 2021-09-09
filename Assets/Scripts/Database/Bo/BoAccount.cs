using System;

namespace ProjectW.DB
{
    // �Ϲ������� �������� ���� Dto�� Bo�� ��ȯ�Ͽ� ���
    // Dto�� ���������� �ΰ��� �������� ����� ���� ����
    // Bo�� �ΰ��� ���������� ���ǰ�, ����� ���� �����Ƿ� ����ȭ�� �ʿ䰡 ����
    // ������, �۾��������� �����͸� Ȯ���ϱ� ���� �������� ���� ������ ����ȭ�Ͽ� 
    // �����͸� �ν����Ϳ� ����
#if UNITY_EDITOR   // #���� �����ϴ� ������ ���� ��ó����..
                   // ��ó����� �����Ϸ��� Ư���� ����� �����ϱ� ���� ����..
                   // ���� �ۼ��� �ڵ�� #if ~ #endif ���̿� �ִ� �ڵ尡 ����Ƽ �����Ϳ�����
                   // �����ϵ����ϴ� �ڵ� (���� �ÿ� ���� Ÿ���� �����Ͱ� �ƴ϶�� �ڵ� ��ü�� ���܉�)
    [Serializable]
#endif
    public class BoAccount
    {
        public string nickname;
        public int gold;

        public BoAccount(DtoAccount dtoAccount)
        {
            nickname = dtoAccount.nickname;
            gold = dtoAccount.gold;
        }
    }
}
