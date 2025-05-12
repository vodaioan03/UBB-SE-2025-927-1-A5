#pragma warning disable IDE0290

using System;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using DuoClassLibrary.Interfaces.Helpers;

namespace DuoClassLibrary.Helpers
{
    /// <summary>
    /// Wrapper class for a cross-platform timer that implements our custom IDispatcherTimer interface.
    /// This class uses System.Timers.Timer to provide timer functionality.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RealDispatcherTimer : IDispatcherTimer
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RealDispatcherTimer class.
        /// </summary>
        public RealDispatcherTimer()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += OnElapsed;
        }

        #endregion

        #region Fields

        private readonly System.Timers.Timer timer;

        #endregion

        #region Events

        /// <summary>
        /// Event that occurs when the timer interval has elapsed.
        /// </summary>
        public event EventHandler<object> Tick;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the amount of time between timer ticks.
        /// </summary>
        public TimeSpan Interval
        {
            get => TimeSpan.FromMilliseconds(timer.Interval);
            set => timer.Interval = value.TotalMilliseconds;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start() => timer.Start();

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop() => timer.Stop();

        /// <summary>
        /// Handles the Elapsed event of the timer and raises the Tick event.
        /// </summary>
        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
