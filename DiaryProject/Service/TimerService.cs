using System.Diagnostics.CodeAnalysis;
using DiaryProject.Events;
using DiaryProject.Models;
using Timer = System.Timers.Timer;

namespace DiaryProject.Service;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class TimerService(IEventAggregator aggregator)
{
    private readonly Dictionary<int, Timer> _startTimers = new();

    private readonly Dictionary<int, Timer> _endTimers = new();

    private void RegisterTimer(int id, TimeSpan delay, bool isStart)
    {
        var timer = new Timer(delay);
        timer.AutoReset = true;
        timer.Enabled = true;
        timer.Elapsed += (_, _) =>
        {
            // 如果注册的是开始事件，id会被激活设为真；反之则会取消激活设为假
            aggregator.GetEvent<TimerStatusChanged>().Publish(new TimerStatusChangedModel { Id = id, Status = isStart, SendNotification = true});
            
            // 该计时器不再被使用，从字典中移除，关闭并且释放该计时器
            if (isStart) _startTimers.Remove(id);
            else _endTimers.Remove(id);
            timer.Stop();
            timer.Dispose();
        };
        
        if (isStart) _startTimers.TryAdd(id, timer);
        else _endTimers.TryAdd(id,timer);
    }

    public void DropTracing(int id)
    {
        if (_startTimers.Remove(id, out var t1))
        {
            t1.Stop();
            t1.Dispose();
        }
        
        if (_endTimers.Remove(id, out var t2))
        {
            t2.Stop();
            t2.Dispose();
        }

        aggregator.GetEvent<TimerStatusChanged>().Publish(new TimerStatusChangedModel { Id = id, Status = false, SendNotification = false});
    }
    
    public void RegisterToTimers(MemoRecord? memo)
    {
        if (memo == null) return;
        // 在流程开始时清除对id的追踪
        var id = memo.Id;
        DropTracing(id);
        aggregator.GetEvent<TimerStatusChanged>().Publish(new TimerStatusChangedModel { Id = id, Status = false, SendNotification = false});
        
        if (!memo.Active || memo.EndTime <= DateTime.Now) return;
        
        // 注册事件结束计时器
        var endDelay = new TimeSpan(memo.EndTime.Ticks - DateTime.Now.Ticks);
        RegisterTimer(id, endDelay, false);
        
        // 如果现在事件已经在开始时间以后，说明事件已经开始，发送通知并且不再注册开始计时器
        if (memo.StartTime <= DateTime.Now)
        {
            aggregator.GetEvent<TimerStatusChanged>().Publish(new TimerStatusChangedModel { Id = id, Status = true, SendNotification = true});
            return;
        }
        
        // 注册时间开始计时器
        var startDelay = new TimeSpan(memo.StartTime.Ticks - DateTime.Now.Ticks);
        RegisterTimer(id, startDelay, true);
    }
}