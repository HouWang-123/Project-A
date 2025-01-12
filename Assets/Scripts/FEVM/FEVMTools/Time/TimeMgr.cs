using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FEVM.Timmer
{
    public class TimeMgr : Singleton<TimeMgr>
    {
        /// <summary>  
        /// 定时任务列表  
        /// </summary>  
        private List<TimeTask> taskList = new List<TimeTask>();

        private List<TimeTask> willRemove = new List<TimeTask>();

        /// <summary>  
        /// 添加定时任务
        /// </summary>  
        /// <param name="timeDelay">延时执行时间间隔</param>  
        /// <param name="repeat">是否可以重复执行</param>  
        /// <param name="timeTaskCallback">执行回调</param>  
        public void AddTask(float timeDelay, bool repeat, Action timeTaskCallback, bool excuteNow = false)
        {
            AddTask(new TimeTask(timeDelay, repeat, timeTaskCallback, excuteNow));
        }

        public void AddTask(TimeTask taskToAdd)
        {
            if (taskList.Contains(taskToAdd) || taskToAdd == null) return;
            taskList.Add(taskToAdd);
            if (taskToAdd.executeNow)
            {
                Tools.Notify(taskToAdd.timeTaskCallBack);
            }
        }

        public bool ContainsTask(Action action)
        {
            if (taskList.Count == 0 || action == null) return false;
            return taskList.Find(a => a.timeTaskCallBack == action) != null;
        }

        /// <summary>  
        /// 移除定时任务
        /// </summary>
        /// <param name="taskToRemove"></param>  
        /// <returns></returns>  
        public bool RemoveTask(Action taskToRemove)
        {
            if (taskList.Count == 0 || taskToRemove == null) return false;

            var cur = taskList.Find(a => a.timeTaskCallBack == taskToRemove);
            if (cur != null)
                return taskList.Remove(cur);
            return false;
        }

        /// <summary>  
        /// 暂停定时任务  
        /// </summary>  
        /// <param name="taskToRemove"></param>  
        /// <returns></returns>  
        public bool PauseTask(Action taskToRemove)
        {
            if (taskList.Count == 0 || taskToRemove == null) return false;

            var cur = taskList.Find(a => a.timeTaskCallBack == taskToRemove);
            if (cur != null)
                cur.pause = true;
            else
                Debug.LogError("没有找到该任务，暂停失败");

            return cur != null;
        }

        /// <summary>  
        /// 继续定时任务  
        /// </summary>  
        /// <param name="taskToRemove"></param>  
        /// <returns></returns>  
        public bool ResumeTask(Action taskToRemove)
        {
            if (taskList.Count == 0 || taskToRemove == null) return false;

            var cur = taskList.Find(a => a.timeTaskCallBack == taskToRemove);
            if (cur != null)
                cur.pause = false;
            else
                Debug.LogError("没有找到该任务，继续失败");

            return cur != null;
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void FixedUpdate() // 0.02s
        {
            Tick();
        }

        void OnDestroy()
        {
            taskList.Clear();
            willRemove.Clear();
            taskList = null;
            willRemove = null;
        }

        /// <summary>  
        /// 执行定时任务  
        /// </summary>  
        private void Tick()
        {
            if (taskList == null) return;
            willRemove.Clear();
            for (var i = 0; i < taskList.Count; ++i)
            {
                TimeTask task = taskList[i];
                if (task.pause)
                    continue;

                task.timeDelay -= Time.deltaTime;
                if (task.timeDelay <= 0)
                {
                    Tools.Notify(task.timeTaskCallBack);

                    if (!task.repeat)
                    {
                        willRemove.Add(task);
                        continue;
                    }

                    task.timeDelay = task.timeDelayOnly;
                }
            }

            for (int i = 0; i < willRemove.Count; ++i)
                taskList.Remove(willRemove[i]);
        }

        public void ClearAllTask()
        {
            taskList.Clear();
        }

        /// <summary>  
        /// 定时任务封装类  
        /// </summary>  
        public class TimeTask
        {
            /// <summary>  
            /// 延迟时间  
            /// </summary>  
            private float _timeDelay;

            /// <summary>  
            /// 延迟时间  
            /// </summary>  
            private float _timeDelayOnly;

            /// <summary>  
            /// 是否需要重复执行  
            /// </summary>  
            private bool _repeat;

            /// <summary>  
            /// 回调函数  
            /// </summary>  
            private Action _timeTaskCallBack;

            private bool _pause;

            /// <summary>
            /// 立即执行一次
            /// </summary>
            private bool _executeNow;

            public float timeDelay
            {
                get { return _timeDelay; }
                set { _timeDelay = value; }
            }

            public float timeDelayOnly
            {
                get { return _timeDelayOnly; }
            }

            public bool repeat
            {
                get { return _repeat; }
                set { _repeat = value; }
            }

            public bool pause
            {
                get { return _pause; }
                set { _pause = value; }
            }

            public Action timeTaskCallBack
            {
                get { return _timeTaskCallBack; }
            }

            public bool executeNow
            {
                get { return _executeNow; }
                set { _executeNow = value; }
            }

            //构造函数  
            public TimeTask(float timeDelay, bool repeat, Action timeTaskCallBack, bool executeNow)
            {
                _timeDelay = timeDelay;
                _timeDelayOnly = timeDelay;
                _repeat = repeat;
                _timeTaskCallBack = timeTaskCallBack;
                _executeNow = executeNow;
            }

            public TimeTask(float timeDelay, Action timeTaskCallBack, bool executeNow) : this(timeDelay, true,
                timeTaskCallBack, executeNow)
            {
            }
        }
    }
}