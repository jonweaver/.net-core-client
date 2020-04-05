﻿using Splitio.Services.EventSource.Workers;
using Splitio.Services.Logger;
using Splitio.Services.Shared.Classes;
using System;

namespace Splitio.Services.EventSource
{
    public class SSEHandler : ISSEHandler
    {
        private readonly ISplitLogger _log;
        private readonly IEventSourceClient _eventSourceClient;
        private readonly ISplitsWorker _splitsWorker;
        private readonly ISegmentsWorker _segmentsWorker;
        private readonly INotificationPorcessor _notificationPorcessor;

        public event EventHandler<EventArgs> ConnectedEvent;
        public event EventHandler<EventArgs> DisconnectEvent;

        public SSEHandler(string sseUrl,
            ISplitsWorker splitsWorker,
            ISegmentsWorker segmentsWorker,
            INotificationPorcessor notificationPorcessor,
            ISplitLogger log = null,
            IEventSourceClient eventSourceClient = null)
        {
            _splitsWorker = splitsWorker;
            _segmentsWorker = segmentsWorker;
            _notificationPorcessor = notificationPorcessor;
            _log = log ?? WrapperAdapter.GetLogger(typeof(SSEHandler));
            _eventSourceClient = eventSourceClient ?? new EventSourceClient(sseUrl);
        }

        #region Private Methods
        public void Start()
        {
            _eventSourceClient.EventReceived += EventReceived;
            _eventSourceClient.ConnectedEvent += OnConnected;
            _eventSourceClient.DisconnectEvent += OnDisconnect;
            _eventSourceClient.Connect();
        }

        public void Stop()
        {
            _eventSourceClient.Disconnect();
        }

        public void StartWorkers()
        {
            _splitsWorker.Start();
            _segmentsWorker.Start();
        }

        public void StopWorkers()
        {
            _splitsWorker.Start();
            _segmentsWorker.Stop();
        }
        #endregion

        #region Private Methods
        private void EventReceived(object sender, EventReceivedEventArgs e)
        {
            _notificationPorcessor.Proccess(e.Event);
        }

        private void OnConnected(object sender, EventArgs e)
        {
            ConnectedEvent?.Invoke(this, e);
        }

        private void OnDisconnect(object sender, EventArgs e)
        {
            DisconnectEvent?.Invoke(this, e);
        }
        #endregion
    }
}
