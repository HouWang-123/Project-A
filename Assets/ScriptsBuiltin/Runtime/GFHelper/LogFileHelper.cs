using GameFramework;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// 增加日志文件类型
/// </summary>
public class LogFileHelper : DefaultLogHelper
{
    private readonly string CurrentLogPath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, "current.log"));
    private readonly string PreviousLogPath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, "previous.log"));

    public LogFileHelper()
    {
        Application.logMessageReceived += OnLogMessageReceived;

        try
        {
            if (File.Exists(PreviousLogPath))
            {
                File.Delete(PreviousLogPath);
            }

            if (File.Exists(CurrentLogPath))
            {
                File.Move(CurrentLogPath, PreviousLogPath);
            }
        }
        catch
        {
        }
    }

    private void OnLogMessageReceived(string logMessage, string stackTrace, LogType logType)
    {
        string log = Utility.Text.Format("[{0}][{1}] {2}{4}{3}{4}", "zone: " + TimeZoneInfo.Local.Id.ToString() + ": " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), logType.ToString(), logMessage ?? "<Empty Message>", stackTrace ?? "<Empty StackTrace>", Environment.NewLine);
        try
        {
            File.AppendAllText(CurrentLogPath, log, Encoding.UTF8);
        }
        catch
        {
        }
    }
}

