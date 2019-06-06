using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public interface IDataServer<TOutput, TOperation>
    {
        bool IsInit { get; }
        ObservableCollection<TOutput> Data { get; }

        event EventHandler<IEnumerable<TOperation>> DataAdded;
        event EventHandler<IEnumerable<TOperation>> DataRemoved;
    }
}