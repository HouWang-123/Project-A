using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using Animation = Spine.Animation;

public static class AnimationController
{
    public static void PlayAnim(GameObject monsterBaseFSM, StateEnum animationEnum, int trackIndex, bool loop, float playSpeed = 1.0f, float mixDuration = 0.2f)
    {
        // 状态对应动画
        var monsterFSM = monsterBaseFSM.GetComponent<MonsterBaseFSM>();
        // 动画个数
        List<string> animNames = monsterFSM.AnimationEnumWithName[animationEnum];
        // 大于1就随机等概率播放某一个动画
        int animNameIndex;
        if (animNames.Count > 1)
        {
            animNameIndex = Random.Range(0, animNames.Count);
        }
        else
        {
            animNameIndex = 0;
        }
        PlayAnimation(monsterFSM.SkeletonAnim, trackIndex, animNames[animNameIndex], loop, playSpeed, mixDuration);
    }
    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="skeletonAnimation">骨骼动画组件</param>
    /// <param name="trackIndex">轨道</param>
    /// <param name="animName">动画名字</param>
    /// <param name="loop">动画是否循环</param>
    /// <param name="playSpeed">动画播放速度</param>
    /// <param name="mixDuration">动画混合过渡时间</param>
    private static void PlayAnimation(SkeletonAnimation skeletonAnimation, int trackIndex, string animName, bool loop, float playSpeed = 1.0f, float mixDuration = 0.2f)
    {
        if (!skeletonAnimation.skeleton.Data.Animations.Exists(a => a.Name == animName))
        {
            Debug.LogError($"AnimationController /PlayAnimation() => 找不到动画: {animName}");
            return;
        }
        skeletonAnimation.state.SetEmptyAnimation(trackIndex, mixDuration); // 停止当前轨道的动画
        var trackEntry = skeletonAnimation.state.SetAnimation(trackIndex, animName, loop);
        Debug.Log($"AnimationController /PlayAnimation() => 播放动画: {trackEntry.Animation.Name}");
        // StartCoroutine(SmoothSpeedChange(trackEntry, playSpeed));
        // trackEntry.MixDuration = 0.2f; // 动画混合防止突变
        trackEntry.Complete += TrackEntry_Complete;

    }
    /*private IEnumerator SmoothSpeedChange(TrackEntry trackEntry, float targetSpeed, float duration = 0.5f)
    {
        float initialSpeed = trackEntry.TimeScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            trackEntry.TimeScale = Mathf.Lerp(initialSpeed, targetSpeed, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        trackEntry.TimeScale = targetSpeed;
    }*/

    private static void TrackEntry_Complete(TrackEntry trackEntry)
    {
        Debug.Log($"AnimationController /TrackEntry_Complete() => 动画: {trackEntry.Animation.Name} 播放完成");
    }

    public static float AnimationTotalTime(SkeletonAnimation skeletonAnimation, int trackIndex = 0)
    {
        TrackEntry trackEntry = skeletonAnimation.state.GetCurrent(trackIndex);
        if (trackEntry != null)
        {
            Animation currentAnimation = trackEntry.Animation;
            float animationDuration = currentAnimation.Duration;
            return animationDuration;
        }
        return 0f;
    }
}

