using UnityEngine;
using UnityEngine.UI;

// author   :   jave.lin
// 测试 TimerMgr
// TimerMgr     无参数 Timer
// TimerMgr<T>  有 T 类型参数的 Timer
public class Testing : MonoBehaviour
{
    public int no_arg_with_time_scale_timer_id = -1;
    public int no_arg_without_time_scale_timer_id = -1;
    public int arg_without_time_scale_timer_id = -1;
    public int no_arg_delaycall_timer_id = -1;
    public int no_arg_call_everyframe_timer_id = -1;
    public int no_arg_call_interval_timer_id = -1;

    public float with_time_scale_count;
    public float without_time_scale_count;

    public Image with_time_scale_img;
    public Image without_time_scale_img;

    public Slider timescale_slider;
    public Text timescale_txt;

    private void Start()
    {
        timescale_slider.value = Time.timeScale;
        timescale_txt.text = timescale_slider.value.ToString();
        Application.targetFrameRate = 60;
    }

    private void OnDestroy()
    {
        // make sure removing all timer
        if (no_arg_with_time_scale_timer_id != -1)
        {
            TimerMgr.Inst.RemoveTimerById(no_arg_with_time_scale_timer_id);
            no_arg_with_time_scale_timer_id = -1;
        }

        if (no_arg_without_time_scale_timer_id != -1)
        {
            TimerMgr.Inst.RemoveTimerById(no_arg_without_time_scale_timer_id);
            no_arg_without_time_scale_timer_id = -1;
        }

        // if (arg_without_time_scale_timer_id != -1)
        // {
        //     TimerMgr<string>.Inst.RemoveTimerById(arg_without_time_scale_timer_id);
        //     arg_without_time_scale_timer_id = -1;
        // }
        if (arg_without_time_scale_timer_id != -1)
        {
            TimerMgr<string>.Inst.RemoveTimer(arg_without_time_scale_timer_id);
            arg_without_time_scale_timer_id = -1;
        }

        if (no_arg_delaycall_timer_id != -1)
        {
            TimerMgr.Inst.RemoveTimer(no_arg_delaycall_timer_id);
            no_arg_delaycall_timer_id = -1;
        }

        if (no_arg_call_everyframe_timer_id != -1)
        {
            TimerMgr.Inst.RemoveTimer(no_arg_call_everyframe_timer_id);
            no_arg_call_everyframe_timer_id = -1;
        }

        if (no_arg_call_interval_timer_id != -1)
        {
            TimerMgr<Player>.Inst.RemoveTimer(no_arg_call_interval_timer_id);
            no_arg_call_interval_timer_id = -1;
        }
    }

    //==========================================
    // no arg with time scale
    //==========================================
    public void OnWithTimeScaleAddTimerBtnClick()
    {
        if (no_arg_with_time_scale_timer_id == -1)
        {
            no_arg_with_time_scale_timer_id = TimerMgr.Inst.AddTimer(no_arg_with_time_scale_timer_update,
                no_arg_with_time_scale_timer_complete, 0.1f, 0, true);
            Debug.Log($"OnWithTimeScaleAddTimerBtnClick, AddTimer, id : {no_arg_with_time_scale_timer_id}");
        }
    }

    public void OnWithTimeScaleRemoveTimerBtnClick()
    {
        if (no_arg_with_time_scale_timer_id != -1)
        {
            var remove_success = TimerMgr.Inst.RemoveTimerById(no_arg_with_time_scale_timer_id);
            Debug.Log(
                $"OnWithTimeScaleRemoveTimerBtnClick, RemoveTimerById, id : {no_arg_with_time_scale_timer_id}, remove_success : {remove_success}");
            no_arg_with_time_scale_timer_id = -1;
        }
    }

    private void no_arg_with_time_scale_timer_update()
    {
        Debug.Log($"no_arg_with_time_scale_timer_update");
        with_time_scale_count += 0.1f;
        if (with_time_scale_count > 1.0f)
        {
            with_time_scale_count = 0.0f;
        }

        with_time_scale_img.fillAmount = with_time_scale_count;
    }

    private void no_arg_with_time_scale_timer_complete()
    {
        Debug.Log($"no_arg_with_time_scale_timer_complete");
        no_arg_with_time_scale_timer_id = -1;
    }


    //==========================================
    // no arg without time scale
    //==========================================
    public void OnWithoutTimeScaleAddTimerBtnClick()
    {
        if (no_arg_without_time_scale_timer_id == -1)
        {
            no_arg_without_time_scale_timer_id = TimerMgr.Inst.AddTimer(no_arg_without_time_scale_timer_update,
                no_arg_without_time_scale_timer_complete, 0.1f, 0, false);
            Debug.Log($"OnWithoutTimeScaleAddTimerBtnClick, AddTimer, id : {no_arg_without_time_scale_timer_id}");
        }
    }

    public void OnWithoutTimeScaleRemoveTimerBtnClick()
    {
        if (no_arg_without_time_scale_timer_id != -1)
        {
            var remove_success = TimerMgr.Inst.RemoveTimerById(no_arg_without_time_scale_timer_id);
            Debug.Log(
                $"OnWithoutTimeScaleRemoveTimerBtnClick, RemoveTimerById, id : {no_arg_without_time_scale_timer_id}, remove_success : {remove_success}");
            no_arg_without_time_scale_timer_id = -1;
        }
    }

    private void no_arg_without_time_scale_timer_update()
    {
        Debug.Log($"no_arg_without_time_scale_timer_update");
        without_time_scale_count += 0.1f;
        if (without_time_scale_count > 1.0f)
        {
            without_time_scale_count = 0.0f;
        }

        without_time_scale_img.fillAmount = without_time_scale_count;
    }

    private void no_arg_without_time_scale_timer_complete()
    {
        Debug.Log($"no_arg_without_time_scale_timer_complete");
        no_arg_without_time_scale_timer_id = -1;
    }

    public void OnTimeScaleSlider()
    {
        Time.timeScale = timescale_slider.value;
        timescale_txt.text = Time.timeScale.ToString();
    }

    //==========================================
    // dealy call
    //==========================================
    public void OnDelaySayHiBtnClick()
    {
        // // 写法1:
        // if (arg_without_time_scale_timer_id != -1)
        // {
        //     TimerMgr<string>.Inst.RemoveTimerById(arg_without_time_scale_timer_id);
        // }
        // arg_without_time_scale_timer_id = TimerMgr<string>.Inst.AddTimer(name =>
        // {
        //     Debug.Log($"Hi, {name}");
        //     arg_without_time_scale_timer_id = -1;
        // }, "Jave", null, null);

        // 写法2:
        if (arg_without_time_scale_timer_id != -1)
        {
            TimerMgr<string>.Inst.RemoveTimer(arg_without_time_scale_timer_id);
        }

        arg_without_time_scale_timer_id = TimerMgr<string>.Inst.NextFrameCall(name =>
        {
            Debug.Log($"Hi, {name}");
            arg_without_time_scale_timer_id = -1;
        }, "Jave");
    }

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
        if (no_arg_call_everyframe_timer_id != -1)
        {
            TimerMgr.Inst.RemoveTimer(no_arg_call_everyframe_timer_id);
        }

        var counter = 0;
        no_arg_call_everyframe_timer_id = TimerMgr.Inst.EveryFrameCall(() =>
        {
            if (++counter > 120) // 60 FPS，相当于每 2 秒输出一次
            {
                counter = 0;
                Debug.Log("This is my call everyframe tesitng.");
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
        if (no_arg_call_interval_timer_id != -1)
        {
            TimerMgr<Player>.Inst.RemoveTimer(no_arg_call_interval_timer_id);
        }

        no_arg_call_interval_timer_id = TimerMgr<Player>.Inst.IntervalCall(player =>
        {
            player.pos += Vector2.one;
            Debug.Log($"current player pos : {player.pos}");
        }, player, 1.5f);
    }
}