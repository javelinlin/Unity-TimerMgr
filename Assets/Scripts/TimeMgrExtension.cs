// jave.lin 2024/08/08
// 扩展 TimerMgr

using System;

public static class TimeMgrExtension
{
    public static bool RemoveTimer(this TimerMgr timer, int id)
    {
        if (id != -1)
        {
            timer.RemoveTimerById(id);
            return true;
        }

        return false;
    }

    public static bool RemoveTimer<T>(this TimerMgr<T> timer, int id)
    {
        if (id != -1)
        {
            timer.RemoveTimerById(id);
            return true;
        }

        return false;
    }

    // ======================
    // 无参调用
    // ======================
    // 下一帧调用
    public static int NextFrameCall(this TimerMgr timer, Action callback)
    {
        return timer.AddTimer(null, callback, 0.0f, 1, false);
    }

    // 每一帧调用
    public static int EveryFrameCall(this TimerMgr timer, Action callback)
    {
        return timer.AddTimer(callback, null, 0.0f, 0, false);
    }

    // 延迟调用
    public static int DelayCall(this TimerMgr timer, Action callback, float delay)
    {
        return timer.AddTimer(null, callback, delay, 1, false);
    }

    // 频率调用
    public static int IntervalCall(this TimerMgr timer, Action callback, float interval)
    {
        return timer.AddTimer(callback, null, interval, 0, false);
    }

    // ======================
    // 有参调用
    // ======================
    // 下一帧调用
    public static int NextFrameCall<T>(this TimerMgr<T> timer, Action<T> callback, T arg)
    {
        return timer.AddTimer(callback, arg, null, default, 0.0f, 1, false);
    }

    // 每一帧调用
    public static int EveryFrameCall<T>(this TimerMgr<T> timer, Action<T> callback, T arg)
    {
        return timer.AddTimer(callback, arg, null, default, 0.0f, 0, false);
    }

    // 延迟调用
    public static int DelayCall<T>(this TimerMgr<T> timer, Action<T> callback, T arg, float delay)
    {
        return timer.AddTimer(null, default, callback, default, delay, 0, false);
    }

    // 频率调用
    public static int IntervalCall<T>(this TimerMgr<T> timer, Action<T> callback, T arg, float interval)
    {
        return timer.AddTimer(callback, arg, null, default, interval, 0, false);
    }
}