using System;
using System.Collections.Generic;

namespace SimpleAudioBooksPlayer.Service
{
    public interface IObservableDataService<TData> : IDataService<TData> where TData:class
    {
        event EventHandler<IEnumerable<TData>> DataUpdated;
    }
}