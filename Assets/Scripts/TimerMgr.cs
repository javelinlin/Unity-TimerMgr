//#define __TIMER_MGR_PROFILE__
// author       : jave.lin
// description  : 无参数 Timer
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

// author       : jave.lin
// description  : 无参数 Timer
public class TimerMgr
{
    internal class TimerMonoBehaviour : MonoBehaviour
    {
        internal TimerMgr timer;

        private void Update()
        {
            if (timer != null)
            {
                timer.Update(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }
    }

    private static TimerMgr _inst;
    public static TimerMgr Inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = new TimerMgr();
                var timer_mono_behaviour = GameObject.FindObjectOfType<TimerMonoBehaviour>();
                if (timer_mono_behaviour == null)
                {
                    var go = new GameObject($"{typeof(TimerMgr).GetType().Name}");
                    if (timer_mono_behaviour == null)
                    {
                        timer_mono_behaviour = go.AddComponent<TimerMonoBehaviour>();
                    }
                    timer_mono_behaviour.timer = _inst;
                    go.hideFlags = HideFlags.HideAndDontSave;
                    GameObject.DontDestroyOnLoad(go);
                }
            }
            return _inst;
        }
    }
    // 对内
    internal class TimerInfo
    {
        public int instID = -1;         // Timer 的实例 ID
        public Action on_update;        // 每轮回调的方法，用泛型来避免 boxing
        public Action on_complete;      // 完成回调的方法，用泛型来避免 boxing
        public bool start_update;       // 开始时执行一次更新
        public float interval;          // 执行周期（秒）
        public int repeat;              // <=0 无限次
        public int act_times;           // 执行过的次数
        public bool remove;             // 是否需要删除的表示
        public float elapsed;           // 已过时间
        public bool with_time_scale;    // 是否应用 time scale

        public void Clear()
        {
            on_update = null;
            on_complete = null;
        }
    }

    private static List<TimerInfo> timer_list = new List<TimerInfo>(1000);      // 预申请 1000 个 timer 指针空间
    private static List<TimerInfo> adding_list = new List<TimerInfo>(100);      // 预申请 100 个 timer 指针空间

    private static Stack<TimerInfo> timer_pool = new Stack<TimerInfo>();

    private static int _s_inst_id = 0;

    /// <summary>
    /// 添加定时器
    /// </summary>
    /// <param name="on_update">定时器更新回调</param>
    /// <param name="on_complete">定时器结束回调</param>
    /// <param name="interval">时间间隔</param>
    /// <param name="start_update">开始时执行一次回调</param>
    /// <param name="repeat">重复次数</param>
    /// <param name="with_time_scale"></param>
    /// <returns>返回 Timer 实例 ID</returns>
    public int AddTimer(
        Action on_update,
        Action on_complete = null,
         float interval = 1.0f, int repeat = 1, bool start_update = false, bool with_time_scale = false)
    {
        TimerInfo timer = timer_pool.Count > 0 ? timer_pool.Pop() : new TimerInfo();
        // timer.instID = timer.instID == -1 ? ++_s_inst_id : timer.instID;
        timer.instID = ++_s_inst_id;
        timer.on_update = on_update;
        timer.on_complete = on_complete;
        timer.interval = interval;
        timer.start_update = start_update;
        timer.repeat = repeat;
        timer.act_times = 0;
        timer.with_time_scale = with_time_scale;
        timer.remove = false;
        adding_list.Add(timer);
        return timer.instID;
    }

    // 移除 callback 相同实例的单个 Timer，成功返回 True，否则返回 false
    public bool RemoveFirstTimerByCallback(Action callback)
    {
        var count = timer_list.Count;
        // 更新中的列表
        for (int i = 0; i < count; i++)
        {
            if (timer_list[i].on_update == callback)
            {
                timer_list[i].remove = true;
                return true;
            }
        }

        // 添加队列的列表
        count = adding_list.Count;
        for (int i = 0; i < count; i++)
        {
            if (adding_list[i].on_update == callback)
            {
                adding_list[i].remove = true;
                return true;
            }
        }
        return false;
    }

    // 移除 callback 相同实例的多个 Timer，移除成功，返回 > 0 的移除 Timer 的数量，否则返回 0
    public int RemoveAllTimerByCallback(Action callback)
    {
        var remove_count = 0;
        var count = timer_list.Count;
        // 更新中的列表
        for (int i = 0; i < count; i++)
        {
            if (timer_list[i].on_update == callback)
            {
                timer_list[i].remove = true;
                ++remove_count;
            }
        }

        // 添加队列的列表
        count = adding_list.Count;
        for (int i = 0; i < count; i++)
        {
            if (adding_list[i].on_update == callback)
            {
                adding_list[i].remove = true;
                ++remove_count;
            }
        }
        return remove_count;
    }

    // 移除指定 实例 ID 的 timer，成功返回 True，否则返回 false
    public bool RemoveTimerById(int id)
    {
        var count = timer_list.Count;
        // 更新中的列表
        for (int i = 0; i < count; i++)
        {
            if (timer_list[i].instID == id)
            {
                timer_list[i].remove = true;
                return true;
            }
        }

        // 添加队列的列表
        count = adding_list.Count;
        for (int i = 0; i < count; i++)
        {
            if (adding_list[i].instID == id)
            {
                adding_list[i].remove = true;
                return true;
            }
        }
        return false;
    }
    private void Update(float deltaTime_with_timescale, float deltaTime_without_timescale)
    {
#if __TIMER_MGR_PROFILE__
        Profiler.BeginSample("TimerMgr.Update 111");
#endif
        // add
        if (adding_list.Count > 0)
        {
#if __TIMER_MGR_PROFILE__
            Profiler.BeginSample("TimerMgr.Update 222");
#endif
            var len = adding_list.Count;
            for (int i = 0; i < len; i++)
            {
                var timer = adding_list[i];
                if (timer.remove) // 还没添加进来之前，又被删除了
                {
                    timer.Clear();
                    timer_pool.Push(timer);
                }
                else // 如果还是有效的 timer 才添加到更新列表中
                {
                    timer_list.Add(timer);
                }
            }
            adding_list.Clear();
#if __TIMER_MGR_PROFILE__
            Profiler.EndSample();
#endif
        }

        int count = timer_list.Count;
        if (count > 0)
        {
#if __TIMER_MGR_PROFILE__
            Profiler.BeginSample("TimerMgr.Update 333");
#endif
            // update
            for (int i = 0; i < count; i++)
            {
                var timer = timer_list[i];
                if (timer.remove)
                {
                    continue;
                }
                if (timer.repeat > 0)
                {
                    if (timer.act_times >= timer.repeat)
                    {
                        timer.remove = true;
                        timer.on_complete?.Invoke();
                        continue;
                    }
                }

                if (timer.interval == 0)
                {
                    timer.on_update?.Invoke();
                    ++timer.act_times;
                }
                else
                {
                    if (timer.start_update && timer.act_times == 0)
                    {
                        // 初始执行一次
                        timer.on_update?.Invoke();
                        ++timer.act_times;
                    }

                    var apply_time = timer.with_time_scale ? deltaTime_with_timescale : deltaTime_without_timescale;
                    timer.elapsed += apply_time;

                    if (timer.elapsed >= timer.interval)
                    {
                        // 这里暂时不做补帧处理
                        timer.elapsed = timer.elapsed % timer.interval;
                        timer.on_update?.Invoke();
                        ++timer.act_times;
                    }
                }
            }

#if __TIMER_MGR_PROFILE__
            Profiler.EndSample();
#endif

#if __TIMER_MGR_PROFILE__
            Profiler.BeginSample("TimerMgr.Update 444");
#endif
            // remove
            var idx = -1;
            for (int i = 0; i < count; i++)
            {
                var timer = timer_list[i];
                if (timer.remove)
                {
                    timer.Clear();
                    timer_pool.Push(timer);
                    continue;
                }
                else
                {
                    ++idx;
                }
                timer_list[idx] = timer;
            }
            idx += 1;
            if (idx < count)
            {
                timer_list.RemoveRange(idx, count - idx);
            }
#if __TIMER_MGR_PROFILE__
            Profiler.EndSample();
#endif
        }

#if __TIMER_MGR_PROFILE__
        Profiler.EndSample();
#endif
    }
}

public interface ITimerUpdate
{
    void Update(float deltaTime_with_timescale, float deltaTime_without_timescale);
}
internal class ITimerUpdateMono : MonoBehaviour
{
    internal ITimerUpdate timer;

    private void Update()
    {
        if (timer != null)
        {
            timer.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
}

// author       : jave.lin
// description  : 有参数 Timer，也可以使用 object 类型的参数，但是 object 频繁的 boxing/unboxing 会导致过度 GC.Alloc 而触发 GC.Collect
public class TimerMgr<T> : ITimerUpdate
{
    private static TimerMgr<T> _inst;
    public static TimerMgr<T> Inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = new TimerMgr<T>();
                var timer_mono_behaviour = GameObject.FindObjectOfType<ITimerUpdateMono>();
                if (timer_mono_behaviour == null)
                {
                    var go = new GameObject($"{typeof(TimerMgr<T>).GetType().Name}");
                    if (timer_mono_behaviour == null)
                    {
                        timer_mono_behaviour = go.AddComponent<ITimerUpdateMono>();
                    }

                    timer_mono_behaviour.timer = _inst;
                    go.hideFlags = HideFlags.HideAndDontSave;
                    GameObject.DontDestroyOnLoad(go);
                }
            }

            return _inst;
        }
    }
    // 对内
    internal class TimerInfo
    {
        public int instID = -1;         // Timer 的实例 ID
        public Action<T> on_update;     // 每轮回调的方法，用泛型来避免 boxing
        public T on_update_arg;         // 每轮回调的方法参数
        public Action<T> on_complete;   // 完成回调的方法，用泛型来避免 boxing
        public T on_complete_arg;       // 完成回调的方法参数
        public bool start_update;       // 开始时执行一次update
        public float interval;          // 执行周期（秒）
        public int repeat;              // <=0 无限次
        public int act_times;           // 执行过的次数
        public bool remove;             // 是否需要删除的表示
        public float elapsed;           // 已过时间
        public bool with_time_scale;    // 是否应用 time scale

        public void Clear()
        {
            on_update = null;
            on_update_arg = default(T);
            on_complete = null;
            on_complete_arg = default(T);
        }
    }

    private static List<TimerInfo> timer_list = new List<TimerInfo>(1000);      // 预申请 1000 个 timer 指针空间
    private static List<TimerInfo> adding_list = new List<TimerInfo>(100);      // 预申请 100 个 timer 指针空间

    private static Stack<TimerInfo> timer_pool = new Stack<TimerInfo>();

    private static int _s_inst_id = 0;

    // 返回 Timer 实例 ID
    // 如果能使用泛型无参数的 TimerMgr，不尽量不使用 TimerMgr<T> ，因为Action<T> callback 参数传入会有 GC.Alloc
    // 如果无法避免下，只能使用 TimerMgr<T> 的话，在调用不频繁，或是只会调用一次的地方，尽量使用 lambda 匿名函数，消耗会比预先定义的函数会少一些，特别在 IL2CPP下
    // 特别注意的是，无论是 TimerMgr，还his TimerMgr<T>，在 AddTimer 时，尽量不使用闭包
    // 如何确定一个匿名函数是闭包：匿名函数内无引用函数外的临时变量
    // 意思是：
    // - 如果有预定义方法，就有 GC，如下：
    // 预定义：         private void OnUpdate<T>(T arg){ } 
    // 传入预定义：     TimerMgr<T>.Inst.AddTimer(OnUpdate);
    // - 如果有匿名方法（注意不是闭包），就没 GC，如下：
    // 传入匿名函数：   TimerMgr<T>.Inst.AddTimer(arg => { });
    // 可参考：https://docs.unity3d.com/cn/current/Manual/BestPracticeUnderstandingPerformanceInUnity4-1.html 下的 “IL2CPP 下的匿名方法”
    public int AddTimer(
        Action<T> on_update, T on_update_arg = default(T),
        Action<T> on_complete = null, T on_complete_arg = default(T),
        float interval = 1.0f, int repeat = 1, bool start_update = false, bool with_time_scale = false)
    {
        TimerInfo timer = timer_pool.Count > 0 ? timer_pool.Pop() : new TimerInfo();
        // timer.instID = timer.instID == -1 ? ++_s_inst_id : timer.instID;
        timer.instID = ++_s_inst_id;
        timer.on_update = on_update;
        timer.on_update_arg = on_update_arg;
        timer.on_complete = on_complete;
        timer.on_complete_arg = on_complete_arg;
        timer.start_update = start_update;
        timer.interval = interval;
        timer.repeat = repeat;
        timer.act_times = 0;
        timer.with_time_scale = with_time_scale;
        timer.remove = false;

        if (timer.on_update == null && timer.on_complete == null)
        {
            Debug.LogError($"Timer 回调不能同时 on_update 和 on_complete 都为空");
            return -1;
        }
        
        adding_list.Add(timer);
        return timer.instID;
    }

    // 移除 callback 相同实例的单个 Timer，成功返回 True，否则返回 false
    public bool RemoveFirstTimerByCallback(Action<T> callback)
    {
        var count = timer_list.Count;
        // 更新中的列表
        for (int i = 0; i < count; i++)
        {
            if (timer_list[i].on_update == callback)
            {
                timer_list[i].remove = true;
                return true;
            }
        }

        // 添加队列的列表
        count = adding_list.Count;
        for (int i = 0; i < count; i++)
        {
            if (adding_list[i].on_update == callback)
            {
                adding_list[i].remove = true;
                return true;
            }
        }
        return false;
    }

    // 移除 callback 相同实例的多个 Timer，移除成功，返回 > 0 的移除 Timer 的数量，否则返回 0
    public int RemoveAllTimerByUpdateCallback(Action<T> callback)
    {
        var remove_count = 0;
        var count = timer_list.Count;
        // 更新中的列表
        for (int i = 0; i < count; i++)
        {
            if (timer_list[i].on_update == callback)
            {
                timer_list[i].remove = true;
                ++remove_count;
            }
        }

        // 添加队列的列表
        count = adding_list.Count;
        for (int i = 0; i < count; i++)
        {
            if (adding_list[i].on_update == callback)
            {
                adding_list[i].remove = true;
                ++remove_count;
            }
        }
        return remove_count;
    }
    
    // 移除 callback 相同实例的多个 Timer，移除成功，返回 > 0 的移除 Timer 的数量，否则返回 0
    public int RemoveAllTimerByCompleteCallback(Action<T> callback)
    {
        var remove_count = 0;
        var count = timer_list.Count;
        // 更新中的列表
        for (int i = 0; i < count; i++)
        {
            if (timer_list[i].on_complete == callback)
            {
                timer_list[i].remove = true;
                ++remove_count;
            }
        }

        // 添加队列的列表
        count = adding_list.Count;
        for (int i = 0; i < count; i++)
        {
            if (adding_list[i].on_complete == callback)
            {
                adding_list[i].remove = true;
                ++remove_count;
            }
        }
        return remove_count;
    }

    // 移除指定 实例 ID 的 timer，成功返回 True，否则返回 false
    public bool RemoveTimerById(int id)
    {
        var count = timer_list.Count;
        // 更新中的列表
        for (int i = 0; i < count; i++)
        {
            if (timer_list[i].instID == id)
            {
                timer_list[i].remove = true;
                return true;
            }
        }

        // 添加队列的列表
        count = adding_list.Count;
        for (int i = 0; i < count; i++)
        {
            if (adding_list[i].instID == id)
            {
                adding_list[i].remove = true;
                return true;
            }
        }
        return false;
    }
    public void Update(float deltaTime_with_timescale, float deltaTime_without_timescale)
    {
#if __TIMER_MGR_PROFILE__
        Profiler.BeginSample("TimerMgr<T>.Update 111");
#endif
        // add
        if (adding_list.Count > 0)
        {
#if __TIMER_MGR_PROFILE__
            Profiler.BeginSample("TimerMgr<T>.Update 222");
#endif
            var len = adding_list.Count;
            for (int i = 0; i < len; i++)
            {
                var timer = adding_list[i];
                if (timer.remove) // 还没添加进来之前，又被删除了
                {
                    timer.Clear();
                    timer_pool.Push(timer);
                }
                else // 如果还是有效的 timer 才添加到更新列表中
                {
                    timer_list.Add(timer);
                }
            }
            adding_list.Clear();
#if __TIMER_MGR_PROFILE__
            Profiler.EndSample();
#endif
        }

        int count = timer_list.Count;
        if (count > 0)
        {
#if __TIMER_MGR_PROFILE__
            Profiler.BeginSample("TimerMgr<T>.Update 333");
#endif
            // update
            for (int i = 0; i < count; i++)
            {
                var timer = timer_list[i];
                if (timer.remove)
                {
                    continue;
                }
                if (timer.repeat > 0)
                {
                    if (timer.act_times >= timer.repeat)
                    {
#if __TIMER_MGR_PROFILE__
                        Profiler.BeginSample("TimerMgr<T>.Update 333.111");
#endif
                        timer.remove = true;
                        timer.on_complete?.Invoke(timer.on_complete_arg);
#if __TIMER_MGR_PROFILE__
                        Profiler.EndSample();
#endif
                        continue;
                    }
                }

                if (timer.interval == 0)
                {
#if __TIMER_MGR_PROFILE__
                    // 这里 Profile 发现有 GC，因为外头的回调的内容有 GC 问题
                    Profiler.BeginSample("TimerMgr<T>.Update 333.222");
#endif
                    timer.on_update?.Invoke(timer.on_update_arg);
                    ++timer.act_times;
#if __TIMER_MGR_PROFILE__
                    Profiler.EndSample();
#endif
                }
                else
                {
                    if (timer.start_update && timer.act_times == 0)
                    {
                        // 开始时执行一次update
                        timer.on_update?.Invoke(timer.on_update_arg);
                        ++timer.act_times;
                    }

                    var apply_time = timer.with_time_scale ? deltaTime_with_timescale : deltaTime_without_timescale;

                    timer.elapsed += apply_time;

                    if (timer.elapsed >= timer.interval)
                    {
                        // 这里暂时不做补帧处理
#if __TIMER_MGR_PROFILE__
                    // 这里 Profile 发现有 GC，因为外头的回调的内容有 GC 问题
                    Profiler.BeginSample("TimerMgr<T>.Update 333.222");
#endif
                        timer.elapsed = timer.elapsed % timer.interval;
                        timer.on_update?.Invoke(timer.on_update_arg);
                        ++timer.act_times;
#if __TIMER_MGR_PROFILE__
                    Profiler.EndSample();
#endif
                    }
                }


            }
#if __TIMER_MGR_PROFILE__
            Profiler.EndSample();
#endif

#if __TIMER_MGR_PROFILE__
            Profiler.BeginSample("TimerMgr<T>.Update 444");
#endif
            // remove
            var idx = -1;
            for (int i = 0; i < count; i++)
            {
                var timer = timer_list[i];
                if (timer.remove)
                {
                    timer.Clear();
                    timer_pool.Push(timer);
                    continue;
                }
                else
                {
                    ++idx;
                }
                timer_list[idx] = timer;
            }
            idx += 1;
            if (idx < count)
            {
                timer_list.RemoveRange(idx, count - idx);
            }
#if __TIMER_MGR_PROFILE__
            Profiler.EndSample();
#endif
        }
#if __TIMER_MGR_PROFILE__
        Profiler.EndSample();
#endif
    }
}