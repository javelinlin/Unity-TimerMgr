# Unity-TimerMgr
 可实现按指定频率（周期）调用指定的回调函数，或实现延迟调用

# Usage
```csharp
    public int no_arg_delaycall_timer_id = -1;
    public int no_arg_everyframecall_timer_id = -1;
    public int with_arg_intervalcall_timer_id = -1;
    public void UnitTest()
    {
        _Testing_DelayCall();
        _Testing_CallEveryFrame();
        _Testing_CallInterval();
    }

    private void _Testing_DelayCall()
    {
        if (no_arg_delaycall_timer_id != -1)
        {
            TimerMgr.Inst.RemoveTimer(no_arg_delaycall_timer_id);
        }

        no_arg_delaycall_timer_id =
            TimerMgr.Inst.DelayCall(() => { Debug.Log("This is my delay call testing, after 2 seonds."); }, 2f);
    }

    private void _Testing_CallEveryFrame()
    {
        if (no_arg_everyframecall_timer_id != -1)
        {
            TimerMgr.Inst.RemoveTimer(no_arg_everyframecall_timer_id);
        }

        var counter = 0;
        no_arg_everyframecall_timer_id = TimerMgr.Inst.EveryFrameCall(() =>
        {
            if (++counter > 120) // 60 FPS，相当于每 2 秒输出一次
            {
                counter = 0;
                Debug.Log("This is my everyframe call tesitng.");
            }
        });
    }

    public class Player
    {
        public Vector2 pos;
    }

    private Player player = new Player();

    private void _Testing_CallInterval()
    {
        if (with_arg_intervalcall_timer_id != -1)
        {
            TimerMgr<Player>.Inst.RemoveTimer(with_arg_intervalcall_timer_id);
        }

        with_arg_intervalcall_timer_id = TimerMgr<Player>.Inst.IntervalCall(player =>
        {
            player.pos += Vector2.one;
            Debug.Log($"call interval, current player pos : {player.pos}");
        }, player, 1.5f);
    }
```
