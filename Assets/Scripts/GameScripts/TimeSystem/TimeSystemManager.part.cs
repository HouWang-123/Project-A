using System;
using System.Collections.Generic;
using UnityEngine;

public partial class TimeSystemManager : MonoBehaviour
{
    public struct GameHourEvent
    {
        public float triggerTime; // 游戏内时间（小时）
        public Action onTrigger;
    }
    public struct PhasedChangedEvent
    {
        public TimePhaseEnum phase; // 时间段
        public Action onTrigger;
    }
    public struct GameMinuteEvent
    {
        public float triggerTime; // 游戏内时间（分钟）
        public Action onTrigger;
    }

    public List<GameHourEvent> HourScheduledEvents;
    private BitMap m_hourTriggeredFlags; // 触发状态标记
    public List<GameMinuteEvent> MinuteScheduledEvents;
    private BitMap m_minuteTriggeredFlags; // 触发状态标记
    public List<PhasedChangedEvent> PhasedChangedScheduledEvents;
    private BitMap m_phaseTriggeredFlags; // 触发状态标记

    // 每天开始时重置所有标记
    private void ResetFlags()
    {
        for (int i = 0; i < MinuteScheduledEvents.Count; i++)
        {
            m_minuteTriggeredFlags.Clear(i);
        }
        for (int i = 0; i < PhasedChangedScheduledEvents.Count; i++)
        {
            m_phaseTriggeredFlags.Clear(i);
        }
        for (int i = 0; i < HourScheduledEvents.Count; ++i)
        {
            m_hourTriggeredFlags.Clear(i);
        }
    }

    private void CheckHoursEvents(float gameHours)
    {
        float gameMinutes = gameHours * 60f;
        if (HourScheduledEvents != null)
        {
            int i = 0;
            foreach (var e in HourScheduledEvents)
            {
                if (!m_hourTriggeredFlags.Get(i) && Mathf.Abs(gameHours - e.triggerTime) < 0.0001f) // 误差范围
                {
                    Debug.Log(GetType() + "当前游戏内时间：[ 小时: " + gameHours + " ,分钟: " + gameMinutes + " ]");
                    e.onTrigger?.Invoke();
                }
                m_hourTriggeredFlags.Set(i);
                ++i;
            }
        }
    }

    private void CheckMinuteEvents(float gameMinutes)
    {
        float gameHours = gameMinutes / 60f;
        if (MinuteScheduledEvents != null)
        {
            int i = 0;
            foreach (var e in MinuteScheduledEvents)
            {
                if (!m_minuteTriggeredFlags.Get(i) && Mathf.Abs(gameMinutes - e.triggerTime) < 0.0001f) // 误差范围
                {
                    Debug.Log(GetType() + "当前游戏内时间：[ 小时: " + gameHours + " ,分钟: " + gameMinutes + " ]");
                    e.onTrigger?.Invoke();
                }
                m_minuteTriggeredFlags.Set(i);
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

