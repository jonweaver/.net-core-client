﻿namespace Splitio.Services.SplitFetcher.Interfaces
{
    public interface ISplitFetcher
    {
        void Start();
        void Stop();
        void FetchSplits();
        void Clear();
    }
}
