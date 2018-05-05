using System;
using System.Windows.Threading;

namespace Model
{
    /// <summary>
    /// Timer for DatabaseConnection. Periodically
    /// checks if the connection status changed.
    /// </summary>
    public class ExternalTimeDispatcher
    {
        private readonly DispatcherTimer dispatcherTimer;
        private readonly MySQLDatabaseConnection databaseReference;

        private const double refreshInterval = 2.0;

        public ExternalTimeDispatcher(MySQLDatabaseConnection databaseReference)
        {
            this.databaseReference = databaseReference;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += Tick;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(refreshInterval);
            dispatcherTimer.Start();
        }

        public void Stop()
        {
            dispatcherTimer.Stop();
        }

        private void Tick(object sender, EventArgs e)
        {
            databaseReference.CheckAvailability();
        }
    }
}
