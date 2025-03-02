using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncTimer : Singleton<AsyncTimer>
{
    // 存储所有任务的字典
    private Dictionary<string, CancellationTokenSource> _taskTokens = new Dictionary<string, CancellationTokenSource>();

    // 私有构造函数，确保单例

    // 启动定时任务
    public void StartTask(string taskId, float interval, float duration, Action onTick, Action onComplete = null)
    {
        if (_taskTokens.ContainsKey(taskId))
        {
            Debug.Log($"当前 {taskId} 任务已经在执行.");
            return;
        }

        var cts = new CancellationTokenSource();
        _taskTokens[taskId] = cts;
        RunTask(taskId, interval, duration, onTick, onComplete, cts.Token);
    }

    // 停止定时任务
    public void StopTask(string taskId)
    {
        if (_taskTokens.TryGetValue(taskId, out var cts))
        {
            cts.Cancel();
            _taskTokens.Remove(taskId);
        }
        else
        {
            Debug.Log($"找不到 {taskId} 的任务.");
        }
    }

    // 停止所有任务
    public void StopAllTasks()
    {
        foreach (var taskId in _taskTokens.Keys)
        {
            StopTask(taskId);
        }
    }

    private async void RunTask(string taskId, float interval, float duration, Action onTick, Action onComplete, CancellationToken token)
    {
        DateTime startTime = DateTime.Now;

        try
        {
            while ((DateTime.Now - startTime).TotalSeconds < duration)
            {
                if (token.IsCancellationRequested)
                    return;

                onTick?.Invoke(); // 执行任务
                await Task.Delay((int)(interval * 1000), token); // 等待 interval 秒
            }

            onComplete?.Invoke(); // 任务结束
        }
        catch (TaskCanceledException)
        {
            Debug.Log($"任务执行完毕");
        }
        finally
        {
            _taskTokens.Remove(taskId); // 任务完成后移除
        }
    }
}