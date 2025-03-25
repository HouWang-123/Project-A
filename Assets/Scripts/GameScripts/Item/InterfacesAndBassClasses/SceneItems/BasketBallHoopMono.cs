using Spine.Unity;
using System;
using System.Collections.Generic;
using Spine;
using UnityEngine;
/// <summary>
/// 篮球框的Mono
/// </summary>
public class BasketballHoopMono : RoomRiddleItemBase
{
    [Header("解密房间的引用")]
    public RoomRiddleMonoBase riddleGameObject;
    [SerializeField]
    [Header("骨骼动画组件")]
    private SkeletonAnimation skeletonAnimation;
    // 目前场景中篮球的数量
    public int BasketBallCount = 1;
    // 一个动画所需的篮球数
    private int basketNumPerStep;
    // 放入篮筐的篮球
    private Stack<Throwable> throwables;

    // 仅在首次调用 Update 方法之前调用 Start
    private void Start()
    {
        basketNumPerStep = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(BasketBallCount / 3.0)));
        throwables = new Stack<Throwable>();
        TrackEntry trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, "step1", false);
        trackEntry.TimeScale = 0f;
    }

    // 当 MonoBehaviour 将被销毁时调用此函数
    private void OnDestroy()
    {
        throwables.Clear();
        throwables = null;
    }

    // 如果另一个碰撞器进入了触发器，则调用 OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        // 是否为篮球
        if (other.TryGetComponent<Throwable>(out Throwable throwableCom))
        {
            if (throwableCom.ItemID == 230002)
            {
                PushBasketBall(throwableCom);
            }
        }
    }

    public void PushBasketBall(Throwable throwableCom)
    {
        // 根据放入篮筐的篮球数量判断应该播放哪个动画，篮筐一共有三个动画，step1、step2、step3
        if (throwables.Count <  BasketBallCount)
        {
            throwables.Push(throwableCom);
            throwableCom.gameObject.SetActive(false);
            skeletonAnimation.state.SetEmptyAnimation(0, 0.1f); // 停止当前轨道的动画
            if (throwables.Count % basketNumPerStep == 0)
            {
                TrackEntry trackEntry = skeletonAnimation.state.SetAnimation(0, $"step{throwables.Count / basketNumPerStep}", false);
                trackEntry.TimeScale = 1f;
            }
            else if (throwables.Count == BasketBallCount)
            {
                TrackEntry trackEntry = skeletonAnimation.state.SetAnimation(0, "step3", false);
                trackEntry.TimeScale = 1f;
                // 生成安全区域
                isDone = true;
                SpawnSafeArea();
            }
        }
    }

    public Throwable PopBasketBall()
    {
        if (throwables.Count > 0)
        {
            throwables.Pop().gameObject.SetActive(true);
            return throwables.Pop();
        }
        return null;
    }

    private void SpawnSafeArea()
    {
        if (riddleGameObject != null)
        {
            riddleGameObject.SetRiddle();
        }
    }
}
