using ChronoPower.Models;

namespace ChronoPower.Services;

public interface IPowerService
{
    void ExecuteAction(PowerActionType action, bool force);
    void CancelWindowsScheduledShutdown();
    void ScheduleWakeTimer(DateTime wakeTime);
    void CancelWakeTimer();
}
