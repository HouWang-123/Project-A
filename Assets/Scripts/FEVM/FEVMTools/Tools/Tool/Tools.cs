using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DG.Tweening;
using DG.Tweening.Core;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using YooAsset;
using UnityColorUtility = UnityEngine.ColorUtility;
using UnityRandom = UnityEngine.Random;

public static class Tools
{
    #region 事件

    public static void Notify(Action action)
    {
        if (action == null)
            return;

        var actions = action.GetInvocationList();
        for (int i = 0; i < actions.Length; ++i)
        {
            var curAction = actions[i] as Action;
            try
            {
                curAction();
            }
            catch (Exception e)
            {
                //EventManager.OnExceptionOccur.BroadCastEvent(e.Message, e.StackTrace);
                UnityEngine.Debug.LogError("action通知异常-->" + e);
            }
        }
    }

    public static void Notify<T>(Action<T> action, T t)
    {
        if (action == null)
            return;

        var actions = action.GetInvocationList();
        for (int i = 0; i < actions.Length; ++i)
        {
            var curAction = actions[i] as Action<T>;
            try
            {
                curAction(t);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("action通知异常-->" + e);
            }
        }
    }

    public static void Notify<T, T2>(Action<T, T2> action, T t, T2 t2)
    {
        if (action == null)
            return;

        var actions = action.GetInvocationList();
        for (int i = 0; i < actions.Length; ++i)
        {
            var curAction = actions[i] as Action<T, T2>;
            try
            {
                curAction(t, t2);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("action通知异常-->" + e);
            }
        }
    }

    public static void Notify<T, T2, T3>(Action<T> action, T t, T2 t2, T3 t3)
    {
        if (action == null)
            return;

        var actions = action.GetInvocationList();
        for (int i = 0; i < actions.Length; ++i)
        {
            var curAction = actions[i] as Action<T, T2, T3>;
            try
            {
                curAction(t, t2, t3);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("action通知异常-->" + e);
            }
        }
    }

    
    // public static bool ContainsEvent<T>(Action<T> action,Action<T> targetMethod)
    // {
    //     if (action == null)
    //         return false;
    //
    //     var actions = action.GetInvocationList();
    //     for (int i = 0; i < actions.Length; ++i)
    //     {
    //         if (actions[i].Method == targetMethod.Method)
    //         {
    //             UnityEngine.Debug.LogError("ContainsEvent true"+Time.realtimeSinceStartup);
    //             return true;
    //         }
    //     }
    //     UnityEngine.Debug.LogError("ContainsEvent false"+Time.realtimeSinceStartup);
    //     return false;
    // }

    #endregion

    /// <summary>
    /// 得到两个物体的夹角
    /// </summary>
    /// <param name="targetTrans"></param>
    /// <param name="startTrans"></param>
    /// <returns></returns>
    public static float GetAngle(Transform startTrans, Transform targetTrans)
    {
        float curAngle = 0;
        var curPosition = targetTrans.position;
        var normalDis = (curPosition - startTrans.position).normalized;
        var normal = Vector3.up.normalized;
        if (Vector3.Cross(normalDis, normal).z < 0)
            curAngle = Mathf.Acos(Vector3.Dot(normalDis, normal)) * Mathf.Rad2Deg;
        else
        {
            normal = Vector3.down.normalized;
            curAngle = Mathf.Acos(Vector3.Dot(normalDis, normal)) * Mathf.Rad2Deg + 180;
        }

        return curAngle;
    }
    public static string ColorToHex(Color color)
    {
        int r,g,b,a;
        r = Mathf.Clamp((int)(color.r * 255), 0, 255);
        g = Mathf.Clamp((int)(color.g * 255), 0, 255);
        b = Mathf.Clamp((int)(color.b * 255), 0, 255);
        a = Mathf.Clamp((int)(color.a * 255), 0, 255);
        string hex = $"#{r:X2}{g:X2}{b:X2}{a:X2}".ToLower();
        return hex;
    }
    /// <summary>
    /// 比较两个list
    /// </summary>
    /// <param name="l1"></param>
    /// <param name="l2"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool EqualList<T>(List<T> l1, List<T> l2)
    {
        if (l1.Count == l2.Count)
        {
            for (int i = 0; i < l1.Count; i++)
            {
                if (!l2.Contains(l1[i]))
                    return false;
            }
            for (int i = 0; i < l2.Count; i++)
            {
                if (!l1.Contains(l2[i]))
                    return false;
            }

            return true;
        }
        return false;
    }

    public static void SetParentDontKeepScale(Transform self, Transform parent)
    {
        self.SetParent(parent);
        self.localPosition = Vector3.zero;
        self.localEulerAngles = Vector3.zero;
        self.localScale = Vector3.one;
    }
    public static void CopyRectTransform(RectTransform source, RectTransform target)
    {
        if (source == null || target == null)
        {
            Debugger.LogError("Source or target RectTransform is null.");
            return;
        }

        // 复制位置、旋转、缩放
        target.localPosition = source.localPosition;
        target.localRotation = source.localRotation;
        target.localScale = source.localScale;

        // 复制锚点、锚点位置、偏移量和尺寸
        target.anchorMin = source.anchorMin;
        target.anchorMax = source.anchorMax;
        target.anchoredPosition = source.anchoredPosition;
        target.sizeDelta = source.sizeDelta;
        target.pivot = source.pivot;
        target.offsetMin = source.offsetMin;
        target.offsetMax = source.offsetMax;
    }
    public static void SetParent(Transform self, Transform parent,bool isFirstChildTrans = false)
    {
        self.SetParent(parent);
        self.localPosition = Vector3.zero;
        self.localEulerAngles = Vector3.zero;
        self.localScale = Vector3.one;
        if (isFirstChildTrans)
            self.SetAsFirstSibling();
    }

    /// <summary>
    /// 获得随机百分率
    /// </summary>
    /// <returns></returns>
    public static float GetRandomPercent()
    {
        return UnityRandom.Range(0, 101) / 100.0f;
    }

    
    public static string GetTimeHour(int time)
    {
        int h = time / 3600;
        int f = (time % 3600) / 60;
        int s = (time % 3600) % 60;
        string hour = h < 10 ? "0" + h : h.ToString();
        string mins = f < 10 ? "0" + f : f.ToString();
        string secs = s < 10 ? "0" + s : s.ToString();
        return hour + ":" + mins + ":" + secs;
    }
    
    public static string GetTime(int time)
    {
        int f = time / 60;
        int s = time % 60;

        string mins = f < 10 ? "0" + f : f.ToString();
        string secs = s < 10 ? "0" + s : s.ToString();
        return mins + " : " + secs;
    }
    
    /// <summary>
    /// 获得时间格式 只显示分钟
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string GetTimeMinute(int time)
    {
        int f = time / 60;
        string mins = f < 10 ? "0" + f : f.ToString();
        return mins;
    }

    public static T GetComponent<T>(GameObject gameObject) where T : Component
    {
        var t = gameObject.GetComponent<T>();
        if (t == null)
            t = gameObject.AddComponent<T>();

        return t;
    }

    /// <summary>
    /// 判断某个点是否在矩形内
    /// </summary>
    /// <param name="centerPos"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    public static bool InRect(Vector3 centerPos,float height,float width,Vector3 targetPoint)
    {
        
        var leftTopPoint = new Vector3(centerPos.x - width * 0.5f, centerPos.y + height * 0.5f,0);
        var leftBottomPoint = new Vector3(centerPos.x - width * 0.5f, centerPos.y - height * 0.5f,0);
        
        var rightBottomPoint = new Vector3(centerPos.x + width * 0.5f, centerPos.y - height * 0.5f,0);
        var rightTopPoint = new Vector3(centerPos.x + width * 0.5f, centerPos.y + height * 0.5f,0);


        float res = GetCross(leftTopPoint, leftBottomPoint, targetPoint) * GetCross(rightBottomPoint, rightTopPoint, targetPoint);
        
        float res1 = GetCross(leftBottomPoint, rightBottomPoint, targetPoint) * GetCross(rightTopPoint, leftTopPoint, targetPoint);
        
        return res >= 0 &&　res1 >= 0;

    }

    /// <summary>
    /// 叉乘结果 逆时针输入
    /// </summary>
    /// <param name="p"> 目标点）</param>
    /// <param name="p1"> 点1 （矩形四个点）</param>
    /// <param name="p2"> 点2 （矩形四个点）</param>
    /// <returns></returns>
    public static float GetCross( Vector3 p1 ,Vector3 p2,Vector3 p) 
    {
        //a.x*b.y-b.x*a.y
        //return (p2.x - p1.x) * (p.y - p1.y) - (p.x - p1.x) * (p2.y - p1.y);
        var edge = p1 - p2;
        var targetEdge = p1 - p;
        return edge.x*targetEdge.y - targetEdge.x*edge.y;
    }

    /// <summary>
    /// 打字机效果 ugui Text
    /// </summary>
    /// <param name="target"></param>
    /// <param name="content"></param>
    /// <param name="duration"></param>
    /// <param name="richTextEnabled"></param>
    /// <param name="scrambleMode"></param>
    /// <param name="scrambleChars"></param>
    /// <returns></returns>
    public static Tweener DOText(
        Text target,
        string content,
        float duration,
        Action finish = null,
        bool richTextEnabled = true,
        ScrambleMode scrambleMode = ScrambleMode.All,
        string scrambleChars = null)
    {
        return DOTween.To((DOGetter<string>) (() => target.text), (DOSetter<string>) (x => target.text = x), content, duration)
            .SetOptions(richTextEnabled, scrambleMode, scrambleChars).SetTarget<Tweener>((object) target)
            .OnComplete(() =>
            {
                Notify(finish);
            });
    }
    
    /// <summary>
    /// 打字机效果  TextMeshProUGUI Text
    /// </summary>
    /// <param name="target"></param>
    /// <param name="content"></param>
    /// <param name="duration"></param>
    /// <param name="richTextEnabled"></param>
    /// <param name="scrambleMode"></param>
    /// <param name="scrambleChars"></param>
    /// <returns></returns>
    public static Tweener DOText(
        TextMeshProUGUI target,
        string content,
        float duration,
        Action finish = null,
        bool richTextEnabled = true,
        ScrambleMode scrambleMode = ScrambleMode.None,
        string scrambleChars = null)
    {
        return DOTween.To((DOGetter<string>) (() => target.text), (DOSetter<string>) (x => target.text = x), content,
                duration)
            .SetOptions(richTextEnabled, scrambleMode, scrambleChars).SetTarget<Tweener>((object) target).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Notify(finish);
            })
           ;
    }
    
    
    /// <summary>
    /// 时间戳转为C#格式时间
    /// </summary>
    /// <param name="timeStamp">Unix时间戳格式</param>
    /// <returns>C#格式时间</returns>
    public static DateTime GetDateTime(string timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }
    
    /// <summary>
    /// DateTime时间格式转换为Unix时间戳格式
    /// </summary>
    /// <param name="time"> DateTime时间格式</param>
    /// <returns>Unix时间戳格式</returns>
    public static int ConvertDateTimeInt(System.DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (int)(time - startTime).TotalSeconds;
    }
    
    
    /// <summary>
    /// 根据当前时间戳获得距离次日凌晨的秒数
    /// </summary>
    /// <param name="timeStamp">当前时间戳</param>
    /// <returns>距离次日凌晨的秒数</returns>
    public static int GetMorrrowLerpTimes(int timeStamp)
    {
        DateTime nowTime = GetDateTime(timeStamp.ToString());
        DateTime morrrow = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 0, 0, 0);
        int moTime = ConvertDateTimeInt(morrrow);
        return 86400 - (timeStamp - moTime);
    }
    
    // public static FEVMToolsEnum.TargetDirect GetTargetDirect(this Vector3 thisPos,Vector3 targetPos)
    // {
    //     if (thisPos.x <= targetPos.x &&　thisPos.y <= targetPos.y)
    //     {
    //         return FEVMToolsEnum.TargetDirect.RightTop;
    //     }
    //
    //     if (thisPos.x > targetPos.x &&　thisPos.y <= targetPos.y)
    //     {
    //         return FEVMToolsEnum.TargetDirect.LeftTop;
    //     }
    //     
    //     if (thisPos.x > targetPos.x &&　thisPos.y > targetPos.y)
    //     {
    //         return FEVMToolsEnum.TargetDirect.LeftBottom;
    //     }
    //    
    //     return FEVMToolsEnum.TargetDirect.RightBottom;
    // }

    public static int RandomInt(int min, int max)
    {
        return UnityRandom.Range(min, max);
    }
    public static float RandomFloat(float min, float max)
    {
        return UnityRandom.Range(min, max);
    }
    //
    // public static void SetPositionX(this Entity entity, float x)
    // {
    //     entity.Position = new Vector3(x, entity.Position.y, entity.Position.z);
    // }
    //
    // public static void SetPositionY(this Entity entity, float y)
    // {
    //     entity.Position = new Vector3( entity.Position.x, y,   entity.Position.z);
    // }
    //
    // public static void SetPositionZ(this Entity entity, float z)
    // {
    //     entity.Position = new Vector3(entity.Position.x, entity.Position.y, z);
    // }
    //
    // public static void SetLocalScaleX(this Entity entity, float x)
    // {
    //     entity.LocalScale = new Vector3(x, entity.LocalScale.y, entity.LocalScale.z);
    // }
    //
    // public static void SetLocalScaleY(this Entity entity, float y)
    // {
    //     entity.LocalScale = new Vector3( entity.LocalScale.x, y, entity.LocalScale.z);
    // }
    //
    // public static void SetLocalScaleZ(this Entity entity, float z)
    // {
    //     entity.LocalScale = new Vector3(entity.LocalScale.x, entity.LocalScale.y, z);
    // }
    //
    // public static void SetEntityMirror(RoleEntity roleEntity,Vector3 targetPos)
    // {
    //     roleEntity.SetLocalScaleX(roleEntity.OriginalLocalScale.x);
    //     roleEntity.SetLocalScaleX(targetPos.x >= roleEntity.Position.x? roleEntity.OriginalLocalScale.x : -roleEntity.OriginalLocalScale.x);
    // }

    public static string AbsoluteToRelativePath(string absolute)
    {
        string relative = absolute;
        relative = relative.Substring(relative.IndexOf("Assets"));
        relative = relative.Replace('\\', '/');
        return relative;
    }
    
    public static string RelativeToAbsolutePath(string relative)
    {
        //string path="Assets\";
        DirectoryInfo direction = new DirectoryInfo(relative);
        return direction.FullName;
    }

    public static int RandomOffset()
    {
        return Tools.RandomInt(1, 20);
    }
    
    /// <summary>
    /// 解析描述{}
    /// </summary>
    /// <param name="des"></param>
    /// <returns></returns>
    public static string HandleDescribeText(string des,string changeContent)
    {
        StringBuilder sb = new StringBuilder();
        var des01 = des.Split("{".ToCharArray());
        var des02 = des.Split("}".ToCharArray());
        if (des01.Length <= 1)
        {
            return sb.Append(des01[0]).ToString();
        }
        return sb.Append(des01[0]).Append(changeContent).Append(des02[1]).ToString();
    }
    
    /// <summary>
    /// 路径格式化
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string FormatPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            UnityEngine.Debug.LogError("path format 异常！"+path);
            return "";
        }
        return path.Replace('"'.ToString(), "");
    }
    
    /// <summary>
    /// 路径格式化
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string FormatPath(int path)
    {
        var temp = path.ToString();
        if (string.IsNullOrEmpty(temp))
        {
            UnityEngine.Debug.LogError("path format 异常！"+path);
            return "";
        }
        return temp.Replace('"'.ToString(), "");
    }
    
    public static void CalculateCurrentCoolDown(int offsetSeconds, out int hour, out int minute, out int second)
    {
        // 获取当前时间
        DateTime currentTime = DateTime.Now;
        // 计算目标时间
        DateTime targetTime = currentTime.AddSeconds(offsetSeconds);
        // 计算冷却时间
        TimeSpan coolDown = targetTime - currentTime;
        // 设置输出参数
        hour = coolDown.Hours;
        minute = coolDown.Minutes;
        second = coolDown.Seconds;
    }

    public static Color[] GetColorsFromAnyString(string stringsThatMayContainColor)
    {
        // 用正则表达式匹配 # 后面跟着六位或八位十六进制字符的颜色码
        string hexPattern = @"#(?:[0-9a-fA-F]{6}|[0-9a-fA-F]{8})\b";
        Regex regex = new Regex(hexPattern);

        // 匹配所有的十六进制颜色
        MatchCollection matches = regex.Matches(stringsThatMayContainColor);

        List<Color> colors = new List<Color>();

        foreach (Match match in matches)
        {
            string hexColor = match.Value;

            // 尝试解析十六进制颜色并添加到列表中
            if (UnityColorUtility.TryParseHtmlString(hexColor, out Color color))
            {
                colors.Add(color);
            }
        }
        
        return colors.ToArray();
    }
}