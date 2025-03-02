using System.Collections.Generic;
using cfg.buff;
using Unity.Collections;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{
    public Dictionary<int,List<BuffComponet>> components = new Dictionary<int, List<BuffComponet>>();
    public void AddBuff<T>(int targetId,int buffType,int buffTime) where T : BuffComponet
    {
        if(components.ContainsKey(targetId))
        {
            BuffComponet targetBuff = components[targetId].Find(x=>x.buff.ID.Equals(buffType));
            if (targetBuff == null)
            {
                T buff = new BuffComponet() as T;
                buff?.InitBuff(buffType,buffTime);
                components[targetId].Add(buff);
                buff?.Execute();
            }
            else
            {
                targetBuff.BuffTime = targetBuff.buff.CanTimeStack ?  
                    targetBuff.BuffTime < targetBuff.buff.MaxTime ? 
                        targetBuff.BuffTime += buffTime : 
                        targetBuff.BuffTime = targetBuff.buff.MaxTime : 
                    targetBuff.BuffTime = buffTime;
                targetBuff.Execute();
            }
        }
        else
        {
            components.Add(targetId,new List<BuffComponet>());
            T buff = new BuffComponet() as T;
            buff?.InitBuff(buffType,buffTime);
            components[targetId].Add(buff);
            buff?.Execute();
        }
    }
    public void RemoveBuff(int targetId,int buffType)
    {
        if(components.ContainsKey(targetId))
        {
            BuffComponet buff = components[targetId].Find(x=>x.buff.ID.Equals(buffType));
            components[targetId].Remove(buff);
        }
    }

    public bool HasComponet(int targetId,int buffType)
    {
        if(components.ContainsKey(targetId))
        {
            return components[targetId].Find(x=>x.buff.ID.Equals(buffType)) != null;
        }
        return false;
    }

    /// <summary>
    /// 升级Buff
    /// </summary>
    /// <param name="targetId">玩家id</param>
    /// <param name="buffType">上一级的buffid</param>
    /// <returns></returns>
    public bool UpdateBuff(int targetId,int buffType)
    {
        if(components.ContainsKey(targetId))
        {
            BuffComponet buff = components[targetId].Find(x=>x.buff.ID.Equals(buffType));
            if (buff != null)
            {
                if (buff.buff.NextCondition > 0)
                {
                    BuffComponet targetBuff = new BuffComponet();
                    targetBuff.InitBuff(buff.buff.NextCondition,999);
                    RemoveBuff(targetId,buffType);
                    targetBuff.Execute();
                    components[targetId].Add(targetBuff);
                    return true;
                }
                buff.Execute();
                return true;
            }
        }
        return false;
    }
}
