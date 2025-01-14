using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyEventObj;

namespace MyEventObj
{
    public class EventAbs
    {
        public UnityAction Action;
    }

    public class EventAbs<T> : EventAbs
    {
        public new UnityAction<T> Action;
    }
    public class EventAbs<T,T2> : EventAbs
    {
        public new UnityAction<T,T2> Action;
    }
    public class EventAbs<T,T2,T3> : EventAbs
    {
        public new UnityAction<T,T2,T3> Action;
    }
    public class EventAbs<T,T2,T3,T4> : EventAbs
    {
        public new UnityAction<T,T2,T3,T4> Action;
    }
}

public class EventManager
{
    public static EventManager Instance;

    private Dictionary<string, EventAbs> actions;
    private Dictionary<string, Dictionary<string, EventAbs>> actionTs;

    static EventManager()
    {
        Instance = new EventManager();
    }

    private EventManager()
    {
        actions = new Dictionary<string, EventAbs>();
        actionTs = new Dictionary<string, Dictionary<string, EventAbs>>();
    }

    public void RegistEvent(string name, UnityAction action)
    {
        if(actions.ContainsKey(name))
        {
            actions[name].Action += action;
        }
        else
        {
            EventAbs eventAbs = new EventAbs();
            eventAbs.Action = action;
            actions.Add(name, eventAbs);
        }
    }

    public void RegistEvent<T>(string name, UnityAction<T> action)
    {
        if(actionTs.ContainsKey(name))
        {
            var dic = actionTs[name];
            if(dic.ContainsKey(typeof(T).FullName))
            {
                EventAbs<T> abs = dic[typeof(T).FullName] as EventAbs<T>;
                abs.Action += action;
            }
            else
            {
                EventAbs<T> abs = new EventAbs<T>();
                abs.Action = action;
                dic.Add(typeof(T).FullName, abs);
            }
        }
        else
        {
            Dictionary<string, EventAbs> dic = new Dictionary<string, EventAbs>();
            EventAbs<T> abs = new EventAbs<T>();
            abs.Action += action;
            dic.Add(typeof (T).FullName, abs);
            actionTs.Add(name, dic);
        }
    }

    public void RemoveEvent(string name, UnityAction action)
    {
        if(actions.ContainsKey(name))
        {
            actions[name].Action -= action;
        }
    }

    public void RemoveEvent<T>(string name, UnityAction<T> action)
    {
        if(actionTs.ContainsKey(name))
        {
            if(actionTs[name].ContainsKey(typeof(T).FullName))
            {
                EventAbs<T> abs = actionTs[name][typeof(T).FullName] as EventAbs<T>;
                abs.Action -= action;
            }
        }
    }

    public void RunEvent(string name)
    {
        if(actions.ContainsKey(name))
        {
            actions[name]?.Action?.Invoke();
        }
    }

    public void RunEvent<T>(string name, T obj)
    {
        if(actionTs.ContainsKey(name))
        {
            if(actionTs[name].ContainsKey(typeof(T).FullName))
            {
                EventAbs<T> abs = actionTs[name][typeof(T).FullName] as EventAbs<T>;
                abs?.Action?.Invoke(obj);
            }
        }
    }

}
