using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class HandleSWGroup: MonoBehaviour
{
    public List<GameObject> OffSw = new List<GameObject>();
    public List<GameObject> OnSw = new List<GameObject>();

    private void Start()
    {
        OffSw = GetHandleSW<RiddleSwitch,GameObject>();
        OnSw = GetHandleSW<RiddleSwitch,GameObject>();
    }

    private List<A> GetHandleSW<T,A>() 
        where A : Object
        where T : MonoBehaviour
    {
        List<A> currentSW = new List<A>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<T>().GetType() == typeof(T))
            {
                currentSW.Add(transform.GetChild(i).gameObject as A);
            }
        }
        return currentSW;
    }

    public void ChangeGroupSWByMessage(string Message)
    {
        //todo 后续做组内相关开关切换状态
    }
}