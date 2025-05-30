﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static TimeSystemManager;
/// <summary>
/// 全局光控制器
/// </summary>
public class GlobalLightController : MonoBehaviour
{
    // 光源切换过渡时间
    [SerializeField] private float transitionSpeed = 1.5f;
    [SerializeField] private Light2D m_globalLight2D;
    public Light2D GlobalLight2D { get { return m_globalLight2D; } set { m_globalLight2D = value; } }
    // 时间段对应的事件
    private readonly List<PhasedChangedEvent> phasedEvents;
    // 时间段对应的光的颜色
    readonly Dictionary<TimePhaseEnum, Color> timePhaseToColor;
    // 记录离开安全屋前的时间段对应的颜色
    private Color safetyHouseColor;

    public GlobalLightController()
    {
        timePhaseToColor = new(4)
        {

            { TimePhaseEnum.Night2, new (20f / 255f, 20f / 255f, 25f / 255f) },
            { TimePhaseEnum.Day, new (190f / 255f, 190f / 255f, 220f / 255f) },
            { TimePhaseEnum.Dusk, new (120f / 255f, 50f / 255f, 40f / 255f) },
            { TimePhaseEnum.Night1, new (30f / 255f, 30f / 255f, 70f / 255f) }
        };

        phasedEvents = new(4);
    }

    private void Awake()
    {
        if (m_globalLight2D == null)
        {
            m_globalLight2D = GetComponent<Light2D>();
        }
        m_globalLight2D.lightType = Light2D.LightType.Global;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // 添加时间段改变的事件
        for (int i = 1; i <= 3; ++i)
        {
            int currentPhaseIndex = i;
            PhasedChangedEvent phasedEvent = new()
            {
                phase = (TimePhaseEnum)currentPhaseIndex,
                onTrigger = () => { HandleColor((TimePhaseEnum)currentPhaseIndex); }
            };
            phasedEvents.Add(phasedEvent);
            TimeSystemManager.Instance.PhasedChangedScheduledEvents.Add(phasedEvent);
        }
        // EventManager.Instance.RegistEvent(EventConstName.PlayerEnterSafeHouseEvent, PlayerEnterSaftHouseHandler);
        // EventManager.Instance.RegistEvent(EventConstName.PlayerLeaveSafeHouseEvent, PlayerLeaveSaftHouseHandler);
    }

    // private void PlayerEnterSaftHouseHandler()
    // {
    //     // 恢复之前记录的光照
    //     m_globalLight2D.color = safetyHouseColor;
    // }
    //
    // private void PlayerLeaveSaftHouseHandler()
    // {
    //     // 记录离开安全屋时的光照
    //     safetyHouseColor = m_globalLight2D.color;
    //     // 设置光源为晚上
    //     m_globalLight2D.color = timePhaseToColor[TimePhaseEnum.Night1];
    // }

    private void OnDestroy()
    {
        // EventManager.Instance.RemoveEvent(EventConstName.PlayerEnterSafeHouseEvent, PlayerEnterSaftHouseHandler);
        // EventManager.Instance.RemoveEvent(EventConstName.PlayerLeaveSafeHouseEvent, PlayerLeaveSaftHouseHandler);
        phasedEvents.Clear();
        timePhaseToColor.Clear();
    }

    private void HandleColor(TimePhaseEnum timePhaseEnum)
    {
        if (!Enum.IsDefined(typeof(TimePhaseEnum), timePhaseEnum))
        {
            throw new ArgumentOutOfRangeException($"时间段枚举类型并没有这样的枚举[{timePhaseEnum}]");
        }
        if (timePhaseToColor.TryGetValue(timePhaseEnum, out Color color))
        {
            Debug.Log($"{GetType()}/ HandleColor=> 当前时段[{timePhaseEnum}]，转换光线的颜色中");
            StartCoroutine(TransitionColor(color));
        }
        else
        {
            Debug.LogError($"{GetType()}/ onTrigger=> 丢失时间段的颜色信息: {timePhaseEnum}");
            return;
        }
    }

    // 使用插值实现颜色平滑过渡
    protected IEnumerator TransitionColor(Color targetColor)
    {
        float t = 0;
        Color start = m_globalLight2D.color;
        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            m_globalLight2D.color = Color.Lerp(start, targetColor, t);

            // 添加0.5%随机噪点
            if (Time.frameCount % 3 == 0)
            {
                m_globalLight2D.color += UnityEngine.Random.Range(-0.005f, 0.005f) * Color.white;
            }
            yield return null;
        }
    }
}
