using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T :MonoBehaviour
{
    public int ID { get { return id; } }//ȫ�ֶ�һ�޶�ID
    public T Entity;//��Ҫʹ��״̬����ʵ��
    private int id;
    private static int next_Id = 0;

    private string AIConfigName;//�����ļ�����
    private string FSMName;//״̬����������
    private string defaultStateId;   //Ĭ��״̬���

    private bool IsInGlobal; //�Ƿ���ȫ��״̬
    private State<T> currentState;      //��ǰ״̬
    private string currentStateId;   //��ǰ״̬��ţ����ݱ�ŵõ���ǰ״̬��
    private State<T> defaultState;      //Ĭ��״̬
    private State<T> lastState; //����ȫ��״̬ǰ����һ״̬

    private List<State<T>> states = new List<State<T>>();//״̬����
    private List<State<T>> globalStates = new List<State<T>>();//ȫ��״̬����

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

        //ʵʱ��������仯����״̬
        currentState.Reason(this);
        currentState.Action(Entity);
    }

    /// <summary>
    /// ���������ļ�����״̬����ȷ��������״̬��ӳ���ϵ��
    /// </summary>
    public void ConfigFSM()
    {
        //ʹ��AI�����ļ���ȡ�����ļ��е���Ϣ���ֵ���
        var dic = ConfigurationReader.LoadAI(AIConfigName);
        foreach (var stateName in dic.Keys)
        {
            //����״̬����
            var type = Type.GetType(FSMName + stateName + "State");
            var stateObj = Activator.CreateInstance(type) as State<T>;
            //�������ӳ��
            foreach (var trigger in dic[stateName].Keys)
            {
                //�ַ���
                stateObj.AddTrigger(trigger, dic[stateName][trigger], FSMName);
            }
            //����״̬���Ͽ�
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
    /// ��ʼ��Ĭ��״̬
    /// </summary>
    public void InitDefaultState()
    {
        //�������Դ���ָ����Ĭ��״̬�������ʼ��
        defaultState = states.Find(s => s.stateID == defaultStateId);
        currentState = defaultState;
        currentStateId = defaultStateId;
    }

    /// <summary>
    /// ���������޸�״̬
    /// </summary>
    /// <param name="triggerID"></param>
    public void ChangeActiveState(string triggerID)
    {
        //1.����������鵱ǰ״̬����ǰ������ȷ����һ״̬
        var nextStateId = currentState.GetOutputStates(triggerID);
        if (nextStateId == string.Empty)//���磺���������û����һ��״̬
        {
            return;
        }
        State<T> nextState = states.Find(s => s.stateID == nextStateId);
        //2.�˳���ǰ״̬
        currentState.ExitState(Entity);
        currentState = nextState;//���µ�ǰ״̬
        currentStateId = nextStateId;//���µ�ǰ״̬���
        //3.������һ״̬
        currentState.EnterState(Entity);

    }

    /// <summary>
    /// �޸�Ϊȫ��״̬
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
    /// ȫ��״̬תΪ��ͨ״̬
    /// </summary>
    public void RecoverToLastState() 
    {
        currentState.ExitState(Entity);
        currentState = lastState;
        currentState.EnterState(Entity);
    }
}
