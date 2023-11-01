namespace Backworker.Model;

internal class ScheduleTaskSettings
{
    public int MillisecondsRepeatPeriod;
    public DateTime? TimeDayStart;
    public DateTime? TimeDayStop;
    public int MillisecondsRestartDelay;
    public int MillisecondsCrashRestartDelay;

    public bool TimeToStart(DateTime now, DateTime lastStartTime, DateTime lastStopDate, DateTime? lastLockTime)
    {
        bool scheduleStartDayTime = true;
        if (TimeDayStart.HasValue&& TimeDayStop.HasValue)
            scheduleStartDayTime =
                (now.TimeOfDay >= TimeDayStart.Value.TimeOfDay
                 && now.TimeOfDay < TimeDayStop.Value.TimeOfDay);

        bool scheduleStartTime =
            (now - lastStartTime).TotalMilliseconds >= MillisecondsRepeatPeriod
            && (now - lastStopDate).TotalMilliseconds >= MillisecondsRestartDelay;

        bool restartCrash =
            lastLockTime.HasValue
            && (now - lastLockTime.Value).TotalMilliseconds >= MillisecondsCrashRestartDelay;

        bool timeToStart = scheduleStartDayTime && scheduleStartTime;
        if (timeToStart && lastLockTime.HasValue)
            timeToStart = restartCrash;

        return timeToStart;
    }
}