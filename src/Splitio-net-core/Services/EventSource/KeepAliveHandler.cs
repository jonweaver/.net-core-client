﻿using System;
using System.Diagnostics;
using System.Threading;

namespace Splitio.Services.EventSource
{
    public class KeepAliveHandler : IKeepAliveHandler
    {
        private readonly object _clockLock = new object();
        private Stopwatch _clock;

        public event EventHandler<EventArgs> ReconnectEvent;

        #region Public Methods
        public void Start(CancellationToken cancellationToken)
        {
            _clock = new Stopwatch();
            _clock.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                var seconds = GetTimerValue() / 1000;

                if (seconds >= 70)
                {
                    OnReconnect(EventArgs.Empty);
                    _clock.Stop();
                    break;
                }
            }
        }

        public void Restart()
        {
            lock (_clockLock)
            {
                _clock.Restart();
            }
        }
        #endregion

        #region Private Methods
        private long GetTimerValue()
        {
            lock (_clockLock)
            {
                return _clock.ElapsedMilliseconds;
            }
        }

        private void OnReconnect(EventArgs e)
        {
            ReconnectEvent?.Invoke(this, e);
        }
        #endregion
    }
}