using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class TimeSystemManager : MonoBehaviour
{
    private static TimeSystemManager _instance;
    private TimeSystemManager() { }
    
    public static TimeSystemManager Instance
    {
        get
        {
            // 在非播放模式下直接返回null，防止创建新实例
            if (!Application.isPlaying) return null;
            if (_instance == null)
            {
                // 查找游戏对象上的单例组件，如果找不到则创建一个新的游戏对象并附加组件
                _instance = FindFirstObjectByType<TimeSystemManager>();
                if (_instance == null)
                {
                    GameObject obj = new()
                    {
                        name = "TimeSystemManager(Singleton)"
                    };
                    _instance = obj.AddComponent<TimeSystemManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    // 时间配置
    private readonly Dictionary<TimePhaseEnum, float> phaseDurations = 
        new() 
        { 
            { TimePhaseEnum.Night1, 0.1f * 60f }, { TimePhaseEnum.Day, 0.1f * 60f },
            { TimePhaseEnum.Dusk, 0.1f * 60f }, { TimePhaseEnum.Night2, 0.1f * 60f }
        };

    // 时间阶段开始的秒数
    private readonly Dictionary<TimePhaseEnum, float> phaseStartSeconds = new();

    private const float realSecondsPerGameDay = 0.4f * 60f; // 游戏一天的时间对应的现实秒数

    // 时间相关的事件
    private readonly UnityEvent<TimePhaseEnum, TimePhaseEnum> onTimePhaseChanged = new();
    public UnityEvent<TimePhaseEnum, TimePhaseEnum> OnTimePhaseChanged { get { return onTimePhaseChanged; } }
    private readonly UnityEvent<float> onMinutePassed = new(); // 参数为游戏内时间
    public UnityEvent<float> OnMinutePassed {  get { return onMinutePassed; } }
    private readonly UnityEvent onDayPassed = new(); // 每日结束事件
    public UnityEvent OnDayPassed { get { return onDayPassed; } }

    private float elapsedRealSeconds;
    private TimePhaseEnum oldPhase = TimePhaseEnum.Night1;
    private TimePhaseEnum currentPhase = TimePhaseEnum.Night1;
    private float phaseProgress;

    private void Awake()
    {
        // 确保单例唯一性
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        float totalSeconds = 0f;
        for (int i = 0; i < phaseDurations.Count; ++i)
        {
            if (i != 0)
            {
                totalSeconds += phaseDurations[(TimePhaseEnum)(i - 1)];
            }
            phaseStartSeconds.Add((TimePhaseEnum)i, totalSeconds);
        }

        m_timeTriggeredFlags = new(128);
        m_phaseTriggeredFlags = new(128);
        TimeScheduledEvents = new();
        PhasedChangedScheduledEvents = new();

        OnMinutePassed.AddListener(CheckMinuteEvents);
        OnTimePhaseChanged.AddListener(CheckPhaseEvents);
        OnDayPassed.AddListener(ResetFlags);
    }

    private void OnDestroy()
    {
        OnMinutePassed.RemoveAllListeners();
        OnTimePhaseChanged.RemoveAllListeners();
        OnDayPassed.RemoveAllListeners();
        // 确保静态引用被清除
        if (_instance == this)
        {
            _instance = null;
        }
    }

    void Update()
    {
        // 时间流逝计算，按T加快时间流速
        float delta;
        if (Input.GetKey(KeyCode.T))
        {
            delta = Time.deltaTime * 5f;
            Debug.Log(GetType() + "按下了时间加速");
        }
        else
        {
            delta = Time.deltaTime;
        }
        elapsedRealSeconds += delta;

        // 游戏时间计算
        float gameTimeNormalized = elapsedRealSeconds / realSecondsPerGameDay;
        currentPhase = CurrentTimePhase(elapsedRealSeconds);
        if (Mathf.Abs(currentPhase - oldPhase) == 2)
        {
            oldPhase = currentPhase;
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
            OnDayPassed.Invoke(); // 触发新一天事件
        }

        // 每分钟事件，到分钟改变的时候
        if (Mathf.FloorToInt(elapsedRealSeconds / 60f) > Mathf.FloorToInt((elapsedRealSeconds - delta) / 60f))
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

