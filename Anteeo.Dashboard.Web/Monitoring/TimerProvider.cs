using System;
using System.Threading;

namespace Anteeo.Dashboard.Web.Monitoring
{
    public interface ITimerProvider
    {
        void Register(TimerCallback callback, int interval);
    }

    public class TimerProvider : ITimerProvider
    {
        public Timer Timer { get; set; }

        public void Register(TimerCallback callback, int interval)
        {
            Timer = new Timer(callback, null, 0, interval);
        }
    }
}