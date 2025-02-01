using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trigger<T> where T : MonoBehaviour
{
    public string triggerid;//条件枚举，看看当前符合什么条件

    public Trigger()
    {
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    abstract public void Init();//因为构造方法无法继承，所以初始化要想继承只能另写

    /// <summary>
    /// 处理条件
    /// </summary>
    abstract public bool HandleTrigger(T Entity);
}
