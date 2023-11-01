namespace Backworker.Model;

internal class BackworkerTask
{
    public int Id;
    public string Name;
    public int Type;
    public bool Active;
    public string MagicString;
    public ScheduleTaskSettings Schedule;

    public DateTime LastStartTime;
    public DateTime LastStopTime;
    private double _duration
    {
        get
        {
            var _duration = (LastStopTime - LastStartTime).TotalMilliseconds;
            return _duration > 0 ? _duration / 1000 : 0;
        }
    }

    public void Start()
    {
        LastStartTime = DateTime.UtcNow;
    }

    public void Stop()
    {
        LastStopTime = DateTime.UtcNow;
    }

    public bool NeedToStart(DateTime now, DateTime? lockTime)
    {
        return Active &&
               Schedule.TimeToStart(now, LastStartTime, LastStopTime, lockTime);
    }

}
