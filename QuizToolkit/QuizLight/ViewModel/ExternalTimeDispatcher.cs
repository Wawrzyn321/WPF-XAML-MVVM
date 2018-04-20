using System;
using System.Windows.Threading;

namespace QuizLight.ViewModel
{
    /// <summary>
    /// Wrapper for DispatcherTimer.
    /// </summary>
    public class ExternalTimeDispatcher
    {
        public Action OnTimeout;
        public Action<int> OnTick;

        public int RemainingTime => targetTime - accumulator;

        private readonly DispatcherTimer dispatcherTimer;

        private int accumulator;
        private int targetTime;

        public ExternalTimeDispatcher(int targetTime)
        {
            this.targetTime = targetTime;
            accumulator = 0;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += Tick;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();
        }

        public void Stop()
        {
            dispatcherTimer.Stop();
        }

        public void Reset(int targetTime)
        {
            this.targetTime = targetTime;
            accumulator = 0;
            dispatcherTimer.Start();
        }

        private void Tick(object sender, EventArgs e)
        {
            accumulator++;
            OnTick?.Invoke(targetTime - accumulator);

            if (accumulator > targetTime)
            {
                OnTimeout?.Invoke();
                dispatcherTimer.Stop();
            }
        }
    }
}
