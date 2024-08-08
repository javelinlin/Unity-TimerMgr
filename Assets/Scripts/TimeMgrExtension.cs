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
    public static int CallNextFrame(this TimerMgr timer, Action callback)
    {
        return timer.AddTimer(null, callback, 0.0f, 1, false);
    }

    // 每一帧调用
    public static int CallEveryFrame(this TimerMgr timer, Action callback)
    {
        return timer.AddTimer(callback, null, 0.0f, 0, false);
    }

    // 延迟调用
    public static int DelayCallback(this TimerMgr timer, Action callback, float delay)
    {
        return timer.AddTimer(null, callback, delay, 1, false);
    }

    // 频率调用
    public static int CallInterval(this TimerMgr timer, Action callback, float interval)
    {
        return timer.AddTimer(callback, null, interval, 0, false);
    }

    // ======================
    // 有参调用
    // ======================
    // 下一帧调用
    public static int CallNextFrame<T>(this TimerMgr<T> timer, Action<T> callback, T arg)
    {
        return timer.AddTimer(callback, arg, null, default, 0.0f, 1, false);
    }

    // 每一帧调用
    public static int CallEveryFrame<T>(this TimerMgr<T> timer, Action<T> callback, T arg)
    {
        return timer.AddTimer(callback, arg, null, default, 0.0f, 0, false);
    }

    // 延迟调用
    public static int DelayCallback<T>(this TimerMgr<T> timer, Action<T> callback, T arg, float delay)
    {
        return timer.AddTimer(null, default, callback, default, delay, 0, false);
    }

    // 频率调用
    public static int CallInterval<T>(this TimerMgr<T> timer, Action<T> callback, T arg, float interval)
    {
        return timer.AddTimer(callback, arg, null, default, interval, 0, false);
    }
}