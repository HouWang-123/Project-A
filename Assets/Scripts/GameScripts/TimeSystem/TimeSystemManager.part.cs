using System;
using System.Collections.Generic;
using UnityEngine;

public partial class TimeSystemManager : MonoBehaviour
{
    [System.Serializable]
    public struct TimedEvent
    {
        public float triggerTime; // 游戏内时间（小时）
        public Action onTrigger;
    }
    public struct PhasedChangedEvent
    {
        public TimePhaseEnum phase; // 时间段
        public Action onTrigger;
    }
    
    public List<TimedEvent> TimeScheduledEvents;
    private BitMap m_timeTriggeredFlags; // 触发状态标记
    public List<PhasedChangedEvent> PhasedChangedScheduledEvents;
    private BitMap m_phaseTriggeredFlags; // 触发状态标记

    // 每天开始时重置所有标记
    private void ResetFlags()
    {
        for (int i = 0; i < TimeScheduledEvents.Count; i++)
        {
            m_timeTriggeredFlags.Clear(i);
        }
        for (int i = 0; i < PhasedChangedScheduledEvents.Count; i++)
        {
            m_phaseTriggeredFlags.Clear(i);
        }
    }

    private void CheckMinuteEvents(float gameMinutes)
    {
        float gameHours = gameMinutes / 60f;
        if (TimeScheduledEvents != null)
        {
            int i = 0;
            foreach (var e in TimeScheduledEvents)
            {
                if (!m_timeTriggeredFlags.Get(i) && Mathf.Abs(gameHours - e.triggerTime) < 0.0001f) // 误差范围
                {
                    Debug.Log(GetType() + "当前游戏内时间：" + gameMinutes);
                    e.onTrigger?.Invoke();
                }
                m_timeTriggeredFlags.Set(i);
                ++i;
            }
        }
    }

    private void CheckPhaseEvents(TimePhaseEnum oldValue, TimePhaseEnum currentValue)
    {
        if (PhasedChangedScheduledEvents != null)
        {
            int i = 0;
            foreach (var e in PhasedChangedScheduledEvents)
            {
                if (!m_phaseTriggeredFlags.Get(i) && currentValue == e.phase)
                {
                    Debug.Log(GetType() + "当前时间段：" + currentValue);
                    e.onTrigger?.Invoke();
                }
                m_phaseTriggeredFlags.Set(i);
                ++i;
            }
        }
    }
}

