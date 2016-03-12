using System;
using System.Threading;

namespace Anteeo.Dashboard.Web.Monitoring
{
    public interface ITimerProvider
    {
        void Register(TimerCallback callback, TimeSpan interval);
    }

    public class TimerProvider : ITimerProvider
    {
        public Timer Timer { get; set; }

        public void Register(TimerCallback callback, TimeSpan interval)
        {
            Timer = new Timer(callback, null, interval, interval);
        }
    }
}