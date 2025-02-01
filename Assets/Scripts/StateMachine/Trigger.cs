using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trigger<T> where T : MonoBehaviour
{
    public string triggerid;//����ö�٣�������ǰ����ʲô����

    public Trigger()
    {
        Init();
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    abstract public void Init();//��Ϊ���췽���޷��̳У����Գ�ʼ��Ҫ��̳�ֻ����д

    /// <summary>
    /// ��������
    /// </summary>
    abstract public bool HandleTrigger(T Entity);
}
