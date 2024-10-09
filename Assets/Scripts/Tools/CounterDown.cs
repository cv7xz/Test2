using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CounterDown<T1,T2> : MonoBehaviour
{
    public float currentTime;
    public float maxTime;

    public Action<T1,T2> action;
    public Action actions;
    public T1 param1;
    public T2 param2;

    public bool pauseFlag;
    public CounterDown(float initTime,float MaxTime,T1 P1,T2 P2)
    {
        pauseFlag = false;
        currentTime = initTime;
        maxTime = MaxTime;
        param1 = P1;
        param2 = P2;
    }
    public void Update()
    {
        if(pauseFlag == false)
        {
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(0,currentTime);
        }

        if (action != null && currentTime <= 0)
        {
            action?.Invoke(param1,param2);
            action = null;
        }

        if(actions != null && currentTime <= 0)
        {
            actions?.Invoke();
            actions = null;
        }
    }
    public void ResetTimer()
    {
        currentTime = maxTime;
    }

    public void PauseTimer()
    {
        pauseFlag = true;
    }
    public void ContinueTimer()
    {
        pauseFlag = false;
    }
    public void AddEndAction(Action T)
    {
        actions += T;
    }
}
