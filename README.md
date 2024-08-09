# Unity-TimerMgr
 可实现按指定频率（周期）调用指定的回调函数，或实现延迟调用

# Usage
```csharp
    public int no_arg_delaycall_timer_id = -1;
    public int no_arg_everyframecall_timer_id = -1;
    public int with_arg_intervalcall_timer_id = -1;
    public int no_arg_nextframecall_timer_id = -1;
    public void UnitTest()
    {
        _Testing_DelayCall();
        _Testing_EveryFrameCall();
        _Testing_IntervalCall();
        _Testing_NextFrameCall();
    }


    //==============================
    // delay call
    //==============================
    private void _Testing_DelayCall()
    {
        if (no_arg_delaycall_timer_id != -1)
        {
            TimerMgr.Inst.RemoveTimer(no_arg_delaycall_timer_id);
        }

        no_arg_delaycall_timer_id = TimerMgr.Inst.DelayCall(() =>
        {
           Debug.Log("This is my delay call testing, after 2 seonds.");
        }, 2f);
    }

    //==============================
    // every frame call
    //==============================
    private void _Testing_EveryFrameCall()
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

    //==============================
    // interval call
    //==============================
    public class Player
    {
        public Vector2 pos;
    }

    private Player player = new Player();

    private void _Testing_IntervalCall()
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

    //==============================
    // next frame call
    //==============================
    private void _Testing_NextFrameCall()
    {
        const int TIMES = 10;

        // sync call
        _refreshSyncCount = 0;
        for (int i = 0; i < TIMES; i++)
        {
            _RefreshBySync();
        }
        
        // async call (next frame)
        _refreshNextFrameCount = 0;
        for (int i = 0; i < TIMES; i++)
        {
            if (no_arg_nextframecall_timer_id == -1)
            {
                no_arg_nextframecall_timer_id = TimerMgr.Inst.NextFrameCall(_RefreshByNextFrame);
            }
        }
        
        /*
         * 输出：
         *  _RefreshBySync call times : 1
            _RefreshBySync call times : 2
            _RefreshBySync call times : 3
            _RefreshBySync call times : 4
            _RefreshBySync call times : 5
            _RefreshBySync call times : 6
            _RefreshBySync call times : 7
            _RefreshBySync call times : 8
            _RefreshBySync call times : 9
            _RefreshBySync call times : 10
            
            _RefreshByNextFrame call times : 1
         */
    }

    private int _refreshSyncCount = 0;
    private void _RefreshBySync()
    {
        ++_refreshSyncCount;
        Debug.Log($"{nameof(_RefreshBySync)} call times : {_refreshSyncCount}");
    }

    private int _refreshNextFrameCount = 0;

    private void _RefreshByNextFrame()
    {
        ++_refreshNextFrameCount;
        Debug.Log($"{nameof(_RefreshByNextFrame)} call times : {_refreshNextFrameCount}");
        no_arg_nextframecall_timer_id = -1;
    }
```
