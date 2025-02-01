using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class State<T> where T : MonoBehaviour
{
    /// <summary>
    /// ȫ��״̬
    /// </summary>
    public bool IsGlobal;
    /// <summary>
    /// �ܷ������״̬ͬʱ����
    /// </summary>
    public bool CanExistWithOtherState;
    /// <summary>
    /// ״̬���
    /// </summary>
    public string stateID;
    /// <summary>
    /// �����б�
    /// </summary>
    private List<Trigger<T>> triggers = new List<Trigger<T>>();
    /// <summary>
    /// ת��ӳ���
    /// </summary>
    private Dictionary<string, string> map = new Dictionary<string, string>();

    public State() { Init(); }

    /// <summary>
    ///  ��ʼ��
    /// </summary>
    abstract public void Init();

    /// <summary>
    /// �������
    /// </summary>
    /// <param name="triggerID"></param>
    /// <param name="stateID"></param>
    public void AddTrigger(string triggerID, string stateID, string FSMName)
    {
        //���������״̬�Ķ�Ӧ��ϵ
        //һ��״̬�п���ӵ�ж����������.
        //ÿ��״̬�е�ÿ����������Ӧһ��������,ÿ��״̬����һ������ת��״̬ӳ���
        if (map.ContainsKey(triggerID))
        {
            map[triggerID] = stateID;
        }
        else
        {
            map.Add(triggerID, stateID);
            AddTriggerObject(triggerID, FSMName);//�����������
        }
    }

    /// <summary>
    /// �����������
    /// </summary>
    /// <param name="triggerID"></param>
    private void AddTriggerObject(string triggerID, string FSMName)
    {
        //��ȻҪ�÷���ȥ��������̫����
        Type type = Type.GetType(FSMName + triggerID + "Trigger");
        if (type != null)
        {
            var triggerObj = Activator.CreateInstance(type) as Trigger<T>;
            triggers.Add(triggerObj);
        }
    }

    /// <summary>
    ///  ɾ������
    /// </summary>
    /// <param name="triggerID"></param>
    /// <param name="stateID"></param>
    public void RemoveTrigger(string triggerID, string stateID)
    {
        if (map.ContainsKey(triggerID))
        {
            map.Remove(triggerID);
            RemoveTriggerObject(triggerID);
        }
    }
    private void RemoveTriggerObject(string triggerID)
    {
        triggers.RemoveAll(t => t.triggerid == triggerID);
    }

    /// <summary>
    /// ��������ӳ��
    /// </summary>
    /// <param name="triggerID"></param>
    public string GetOutputStates(string triggerID)
    {
        if (map.ContainsKey(triggerID))
        {
            return map[triggerID];
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// ״̬��Ϊ(ÿ��������Ϊ��ͬ���������Լ�ȥʵ�ְ�)
    /// </summary>
    /// <param name="fSM"></param>
    abstract public void Action(T Entity);

    /// <summary>
    /// �������(�󲿷�״̬���ƣ�Ҳ�в�ͬ�������)
    /// </summary>
    /// <param name="fSM"></param>
    virtual public void Reason(StateMachine<T> fSM)
    {
        if (triggers == null) { return; }
        for (int i = 0; i < triggers.Count; i++)
        {
            if (triggers[i].HandleTrigger(fSM.Entity))//�������������״̬����ĳ��״̬�仯
            {
                fSM.ChangeActiveState(triggers[i].triggerid);
                return;
            }
        }
    }

    /// <summary>
    /// �뿪״̬(����״̬�뿪ʱ���ܲ�ͬ�������)
    /// </summary>
    /// <param name="fSM"></param>
    virtual public void ExitState(T Entity) { }

    /// <summary>
    /// ����״̬
    /// </summary>
    /// <param name="fSM"></param>
    virtual public void EnterState(T Entity) { }
}
