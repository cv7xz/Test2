using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
public class Messenger : MonoBehaviour
{
    public static Messenger Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    Dictionary<EventType, IEventAction> typeDict0 = new Dictionary<EventType, IEventAction>();

    Dictionary<EventType, IEventAction> typeDict1 = new Dictionary<EventType, IEventAction>();

    Dictionary<EventType, IEventAction> typeDict2 = new Dictionary<EventType, IEventAction>();

    Dictionary<EventType, IEventAction> typeDict3 = new Dictionary<EventType, IEventAction>();
    public void AddListener(EventType type, Action action)
    {
        if (typeDict0.ContainsKey(type) == false)
        {
            typeDict0.Add(type, new EventAction());
        }
        else if ((typeDict0[type] as EventAction) == null)
        {
            typeDict0.Add(type, new EventAction());
        }
        (typeDict0[type] as EventAction).actions += action;
    }

    public void RemoveListener(EventType type, Action action)
    {
        if(typeDict0.ContainsKey(type) == false)
        {
            return;
        }

        (typeDict0[type] as EventAction).actions -= action;
    }

    public void BroadCast(EventType type)
    {
        if (typeDict0.ContainsKey(type) == false)
        {
            Debug.LogWarning($"{type} is not exist");
            return;
        }
        if (type != EventType.TurnEnd && type != EventType.ChangeTarget && type != EventType.SettleDeath)
        {
            Debug.Log($"{type} BroadCast");
        }
        (typeDict0[type] as EventAction).actions.Invoke();
    }

    public void AddListener<T>(EventType type, Action<T> action)
    {
        if (typeDict1.ContainsKey(type) == false)  
        {
            typeDict1.Add(type, new EventAction<T>());
        }
        else if((typeDict1[type] as EventAction<T>) == null)  
        {
            typeDict1.Add(type, new EventAction<T>());
        }
        (typeDict1[type] as EventAction<T>).actions += action;
    }

    public void RemoveListener<T>(EventType type, Action<T> action)
    {
        if (typeDict1.ContainsKey(type) == false)
        {
            return;
        }

        (typeDict1[type] as EventAction<T>).actions -= action;
    }

    public void BroadCast<T>(EventType type,T param)
    {
        if (typeDict1.ContainsKey(type) == false)
        {
            Debug.LogWarning($"{type} is not exist");
            return;
        }
        if (type != EventType.TurnEnd && type != EventType.ChangeTarget && type != EventType.SettleDeath)
        {
            Debug.Log($"{type} BroadCast");
        }
        (typeDict1[type] as EventAction<T>).actions.Invoke(param);
    }

    public void AddListener<T1,T2>(EventType type, Action<T1, T2> action)
    {
        if (typeDict2.ContainsKey(type) == false)
        {
            typeDict2.Add(type, new EventAction<T1, T2>());
        }
        else if ((typeDict2[type] as EventAction<T1, T2>) == null)
        {
            typeDict2.Add(type, new EventAction<T1, T2>());
        }
        (typeDict2[type] as EventAction<T1, T2>).actions += action;
    }

    public void RemoveListener<T1, T2>(EventType type, Action<T1, T2> action)
    {
        if (typeDict2.ContainsKey(type) == false)
        {
            return;
        }

        (typeDict2[type] as EventAction<T1, T2>).actions -= action;
    }

    public void BroadCast<T1,T2>(EventType type,T1 param1, T2 param2)
    {
        if(typeDict2.ContainsKey(type) == false)
        {
            Debug.LogWarning($"{type} is not exist");
            return;
        }
        if(type != EventType.TurnEnd && type != EventType.ChangeTarget && type != EventType.SettleDeath)
        {
            Debug.Log($"{type} BroadCast");
        }

        (typeDict2[type] as EventAction<T1, T2>).actions.Invoke(param1, param2);
    }

    public void AddListener<T1, T2, T3>(EventType type, Action<T1, T2, T3> action)
    {
        if (typeDict3.ContainsKey(type) == false)
        {
            typeDict3.Add(type, new EventAction<T1, T2, T3>());
        }
        else if ((typeDict3[type] as EventAction<T1, T2, T3>) == null)
        {
            typeDict3.Add(type, new EventAction<T1, T2, T3>());
        }
    (typeDict3[type] as EventAction<T1, T2, T3>).actions += action;
    }

    public void RemoveListener<T1, T2, T3>(EventType type, Action<T1, T2, T3> action)
    {
        if (typeDict3.ContainsKey(type) == false)
        {
            return;
        }

        (typeDict3[type] as EventAction<T1, T2, T3>).actions -= action;
    }

    public void BroadCast<T1, T2, T3>(EventType type, T1 param1, T2 param2,T3 param3)
    {
        if (typeDict3.ContainsKey(type) == false)
        {
            Debug.LogWarning($"{type} is not exist");
            return;
        }
        Debug.Log($"{type} BroadCast");
        (typeDict3[type] as EventAction<T1, T2, T3>).actions.Invoke(param1, param2, param3);
    }
    public class IEventAction
    {

    }

    public class EventAction : IEventAction
    {
        public Action actions;
    }

    public class EventAction<T> : IEventAction
    {
        public Action<T> actions;
    }
    public class EventAction<T1,T2> : IEventAction
    {
        public Action<T1, T2> actions;
    }
    public class EventAction<T1,T2,T3>: IEventAction
    {
        public Action<T1, T2, T3> actions;
    }
    public enum EventType
    {
        TurnEnd,
        ChangeTarget,  //ÇÐ»»Ä¿±ê ÏÔÊ¾Flag

        DealDamage,
        ToughShieldBroken,
        CastFinalSkill,

        SettleDeath,
        EnemyDie,

        StatusChange,
        SkillPointChange,


        ToughDamage = 100,

        TurnStart,
        KillTarget,

        TakeDamage,

    };
}
