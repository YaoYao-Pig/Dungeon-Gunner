using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class State<T> where T : MonoBehaviour
{
    /// <summary>
    /// 全局状态
    /// </summary>
    public bool IsGlobal;
    /// <summary>
    /// 能否和其他状态同时存在
    /// </summary>
    public bool CanExistWithOtherState;
    /// <summary>
    /// 状态编号
    /// </summary>
    public string stateID;
    /// <summary>
    /// 条件列表
    /// </summary>
    private List<Trigger<T>> triggers = new List<Trigger<T>>();
    /// <summary>
    /// 转换映射表
    /// </summary>
    private Dictionary<string, string> map = new Dictionary<string, string>();

    public State() { Init(); }

    /// <summary>
    ///  初始化
    /// </summary>
    abstract public void Init();

    /// <summary>
    /// 添加条件
    /// </summary>
    /// <param name="triggerID"></param>
    /// <param name="stateID"></param>
    public void AddTrigger(string triggerID, string stateID, string FSMName)
    {
        //添加条件和状态的对应关系
        //一个状态中可能拥有多个条件对象.
        //每个状态中的每个条件都对应一个输出结果,每个状态都有一个条件转换状态映射表
        if (map.ContainsKey(triggerID))
        {
            map[triggerID] = stateID;
        }
        else
        {
            map.Add(triggerID, stateID);
            AddTriggerObject(triggerID, FSMName);//添加条件对象
        }
    }

    /// <summary>
    /// 添加条件对象
    /// </summary>
    /// <param name="triggerID"></param>
    private void AddTriggerObject(string triggerID, string FSMName)
    {
        //显然要用反射去做，条件太多了
        Type type = Type.GetType(FSMName + triggerID + "Trigger");
        if (type != null)
        {
            var triggerObj = Activator.CreateInstance(type) as Trigger<T>;
            triggers.Add(triggerObj);
        }
    }

    /// <summary>
    ///  删除条件
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
    /// 查找条件映射
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
    /// 状态行为(每个对象行为不同，让他们自己去实现吧)
    /// </summary>
    /// <param name="fSM"></param>
    abstract public void Action(T Entity);

    /// <summary>
    /// 条件检测(大部分状态类似，也有不同，用虚的)
    /// </summary>
    /// <param name="fSM"></param>
    virtual public void Reason(StateMachine<T> fSM)
    {
        if (triggers == null) { return; }
        for (int i = 0; i < triggers.Count; i++)
        {
            if (triggers[i].HandleTrigger(fSM.Entity))//如果条件满足了状态机的某个状态变化
            {
                fSM.ChangeActiveState(triggers[i].triggerid);
                return;
            }
        }
    }

    /// <summary>
    /// 离开状态(各种状态离开时可能不同，用虚的)
    /// </summary>
    /// <param name="fSM"></param>
    virtual public void ExitState(T Entity) { }

    /// <summary>
    /// 进入状态
    /// </summary>
    /// <param name="fSM"></param>
    virtual public void EnterState(T Entity) { }
}
