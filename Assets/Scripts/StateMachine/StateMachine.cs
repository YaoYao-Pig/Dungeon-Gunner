using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T :MonoBehaviour
{
    public int ID { get { return id; } }//全局独一无二ID
    public T Entity;//需要使用状态机的实体
    private int id;
    private static int next_Id = 0;

    private string AIConfigName;//配置文件名称
    private string FSMName;//状态机对象名称
    private string defaultStateId;   //默认状态编号

    private bool IsInGlobal; //是否处于全局状态
    private State<T> currentState;      //当前状态
    private string currentStateId;   //当前状态编号（根据编号得到当前状态）
    private State<T> defaultState;      //默认状态
    private State<T> lastState; //进入全局状态前的上一状态

    private List<State<T>> states = new List<State<T>>();//状态集合
    private List<State<T>> globalStates = new List<State<T>>();//全局状态集合

    public StateMachine(string AIConfigName, string defaultStateId, string FSMName, T Entity) 
    {
        id = next_Id;
        next_Id++;
        this.AIConfigName = AIConfigName;
        this.defaultStateId = defaultStateId;
        this.FSMName = FSMName;
        this.Entity = Entity;
        ConfigFSM();
        InitDefaultState();
    }

    // Update is called once per frame
    public void Update()
    {
        for (int i = 0; i < globalStates.Count; i++) 
        {
            if (globalStates[i].CanExistWithOtherState) 
            {
                globalStates[i].Action(Entity);
            }
        }

        //实时检查条件变化更新状态
        currentState.Reason(this);
        currentState.Action(Entity);
    }

    /// <summary>
    /// 调用配置文件配置状态机（确定条件和状态的映射关系）
    /// </summary>
    public void ConfigFSM()
    {
        //使用AI配置文件读取配置文件中的信息到字典中
        var dic = ConfigurationReader.LoadAI(AIConfigName);
        foreach (var stateName in dic.Keys)
        {
            //创建状态对象
            var type = Type.GetType(FSMName + stateName + "State");
            var stateObj = Activator.CreateInstance(type) as State<T>;
            //添加条件映射
            foreach (var trigger in dic[stateName].Keys)
            {
                //字符串
                stateObj.AddTrigger(trigger, dic[stateName][trigger], FSMName);
            }
            //放入状态集合库
            if (!stateObj.IsGlobal)
            {
                states.Add(stateObj);
            }
            else 
            {
                globalStates.Add(stateObj);
            }
        }
    }

    /// <summary>
    /// 初始化默认状态
    /// </summary>
    public void InitDefaultState()
    {
        //根据属性窗口指定的默认状态编号来初始化
        defaultState = states.Find(s => s.stateID == defaultStateId);
        currentState = defaultState;
        currentStateId = defaultStateId;
    }

    /// <summary>
    /// 根据条件修改状态
    /// </summary>
    /// <param name="triggerID"></param>
    public void ChangeActiveState(string triggerID)
    {
        //1.根据条件检查当前状态、当前条件，确定下一状态
        var nextStateId = currentState.GetOutputStates(triggerID);
        if (nextStateId == string.Empty)//比如：如果是死亡没有下一个状态
        {
            return;
        }
        State<T> nextState = states.Find(s => s.stateID == nextStateId);
        //2.退出当前状态
        currentState.ExitState(Entity);
        currentState = nextState;//更新当前状态
        currentStateId = nextStateId;//更新当前状态编号
        //3.进入下一状态
        currentState.EnterState(Entity);

    }

    /// <summary>
    /// 修改为全局状态
    /// </summary>
    public void ChangeToGlobalState(string nextStateId) 
    {
        State<T> nextState = globalStates.Find(s => s.stateID == nextStateId);
        if (!IsInGlobal)
        {
            currentState.ExitState(Entity);
            lastState = currentState;
            currentState = nextState;
            currentState.EnterState(Entity);
            IsInGlobal = true;
        }
        else 
        {
            if (nextState != currentState) 
            {
                currentState.ExitState(Entity);
                currentState = nextState;
                currentState.EnterState(Entity);
            }
        }
    }

    /// <summary>
    /// 全局状态转为普通状态
    /// </summary>
    public void RecoverToLastState() 
    {
        currentState.ExitState(Entity);
        currentState = lastState;
        currentState.EnterState(Entity);
    }
}
