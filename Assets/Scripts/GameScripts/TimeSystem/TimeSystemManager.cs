using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class TimeSystemManager : Singleton<TimeSystemManager>
{
    // 时间配置，不同阶段对应的现实的秒数，一秒进行处理，乘60是为了把游戏小时转换为游戏秒
    // 前四分钟是夜晚，然后六分钟是白天，两分钟是黄昏，三分钟是夜晚，测试期间除以10方便观察
    private const float realtimeMinute_Night1 = 0.4f;
    private const float realtimeMinute_Day = 0.6f;
    private const float realtimeMinute_Dusk = 0.2f;
    private const float realtimeMinute_Night2 = 0.3f;
    private readonly Dictionary<TimePhaseEnum, float> phaseDurations = 
        new() 
        { 
            { TimePhaseEnum.Night1, realtimeMinute_Night1 * 60f }, { TimePhaseEnum.Day, realtimeMinute_Day * 60f },
            { TimePhaseEnum.Dusk, realtimeMinute_Dusk * 60f }, { TimePhaseEnum.Night2, realtimeMinute_Night2 * 60f }
        };
    // 十五分钟为游戏内一天
    private const float realSecondsPerGameDay = (realtimeMinute_Night1 + realtimeMinute_Day 
        + realtimeMinute_Dusk + realtimeMinute_Night2) * 60f;
    // 时间阶段开始的秒数
    private readonly Dictionary<TimePhaseEnum, float> phaseStartSeconds = new();


    // 时间相关的事件
    private readonly UnityEvent<TimePhaseEnum, TimePhaseEnum> onTimePhaseChanged = new();
    public UnityEvent<TimePhaseEnum, TimePhaseEnum> OnTimePhaseChanged { get { return onTimePhaseChanged; } }
    // 每天结束的事件
    private readonly UnityEvent onDayPassed = new();
    public UnityEvent OnDayPassed { get { return onDayPassed; } }
    // 小时结束的事件
    private readonly UnityEvent<float> onHourPassed = new();
    public UnityEvent<float> OnHourPassed { get { return onHourPassed; } }
    // 分钟结束的事件
    private readonly UnityEvent<float> onMinutePassed = new();
    public UnityEvent<float> OnMinutePassed {  get { return onMinutePassed; } }

    private float elapsedRealSeconds;
    // 当前游戏时间的天数
    private int gameDay = 0;
    public int GameDay { get { return gameDay; } }
    // 当前游戏时间的小时数，计算真实的秒数占游戏一小时对应的现实秒(游戏一天的总秒数除以24)的比例
    public int GameHour { get { return Mathf.FloorToInt(24f * elapsedRealSeconds / realSecondsPerGameDay) % 24; } }
    public int GameMinute { get { return Mathf.FloorToInt(60f * 24f * elapsedRealSeconds / realSecondsPerGameDay) % 60; } }
    private TimePhaseEnum oldPhase = TimePhaseEnum.Night1;
    private TimePhaseEnum currentPhase = TimePhaseEnum.Night1;
    private float phaseProgress;
    private float timeSpeed = 1f;
    public float TimeSpeed { get { return timeSpeed; } set { timeSpeed = value; } }

    private void Awake()
    {
        float totalSeconds = 0f;
        for (int i = 0; i < phaseDurations.Count; ++i)
        {
            if (i != 0)
            {
                totalSeconds += phaseDurations[(TimePhaseEnum)(i - 1)];
            }
            phaseStartSeconds.Add((TimePhaseEnum)i, totalSeconds);
        }

        m_hourTriggeredFlags = new(128);
        m_minuteTriggeredFlags = new(128);
        m_phaseTriggeredFlags = new(128);
        HourScheduledEvents = new();
        MinuteScheduledEvents = new();
        PhasedChangedScheduledEvents = new();

        OnHourPassed.AddListener(CheckHoursEvents);
        OnMinutePassed.AddListener(CheckMinuteEvents);
        OnTimePhaseChanged.AddListener(CheckPhaseEvents);
        OnDayPassed.AddListener(ResetFlags);
        // 默认白天开始
        elapsedRealSeconds = phaseStartSeconds[TimePhaseEnum.Day];
    }

    protected void OnDestroy()
    {
        OnHourPassed.RemoveAllListeners();
        OnMinutePassed.RemoveAllListeners();
        OnTimePhaseChanged.RemoveAllListeners();
        OnDayPassed.RemoveAllListeners();
    }

    void Update()
    {
        // 时间流逝计算，按T加快时间流速
        float delta = Time.deltaTime * timeSpeed;
        elapsedRealSeconds += delta;

        // 游戏时间计算
        float gameTimeNormalized = elapsedRealSeconds / realSecondsPerGameDay;
        currentPhase = CurrentTimePhase(elapsedRealSeconds);
        if (CycleCalculator.Subtract((int)currentPhase, (int)oldPhase, (int)TimePhaseEnum.END) == 2)
        {
            oldPhase = (TimePhaseEnum)CycleCalculator.Subtract((int)currentPhase, 1, (int)TimePhaseEnum.END);
        }
        if (currentPhase != oldPhase)
        {
            onTimePhaseChanged.Invoke(oldPhase, currentPhase);
        }
        phaseProgress = CurrentPhaseProgress(elapsedRealSeconds, currentPhase);
        // 每天循环
        if (elapsedRealSeconds >= realSecondsPerGameDay)
        {
            elapsedRealSeconds = 0f;
            ++gameDay;
            OnDayPassed.Invoke(); // 触发新一天事件
        }
        // 每小时事件，到小时改变的时候
        // 24 * t / C > 24 * (t - C1) / C
        if (Mathf.FloorToInt(24f * elapsedRealSeconds / realSecondsPerGameDay) > Mathf.FloorToInt(24f * (elapsedRealSeconds - delta) / realSecondsPerGameDay))
        {
            onHourPassed.Invoke(gameTimeNormalized * 24f);
        }

        // 每分钟事件，到分钟改变的时候
        if (Mathf.FloorToInt(60f * 24f * elapsedRealSeconds / realSecondsPerGameDay) > Mathf.FloorToInt(60f * 24f * (elapsedRealSeconds - delta) / realSecondsPerGameDay))
        {
            onMinutePassed.Invoke(gameTimeNormalized * 24f * 60f);
        }
    }

    public TimePhaseEnum GetCurrentPhase() => currentPhase;
    public float GetPhaseProgress() => phaseProgress;

    private TimePhaseEnum CurrentTimePhase(float seconds)
    {
        if (seconds < phaseStartSeconds[(TimePhaseEnum)1])
        {
            return (TimePhaseEnum)0;
        }
        else if (seconds >= phaseStartSeconds[(TimePhaseEnum)phaseStartSeconds.Count - 1])
        {
            return (TimePhaseEnum)(phaseStartSeconds.Count - 1);
        }
        else
        {
            for (int i = 1; i <= phaseStartSeconds.Count - 2; ++i)
            {
                if (seconds >= phaseStartSeconds[(TimePhaseEnum)i] && seconds < phaseStartSeconds[(TimePhaseEnum)(i + 1)])
                {
                    return (TimePhaseEnum)i;
                }
            }
        }

        return (TimePhaseEnum)0;
    }

    private float CurrentPhaseProgress(float seconds, TimePhaseEnum timePhase)
    {
        int timePhaseindex = (int)timePhase;
        float timePassedByPhase = 0f;
        for (int i = 0; i < timePhaseindex; ++i)
        {
            timePassedByPhase += phaseDurations[timePhase];
        }
        return (seconds - timePassedByPhase) / phaseDurations[timePhase];
    }
}

