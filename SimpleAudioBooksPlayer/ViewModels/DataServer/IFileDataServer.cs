using System;
using System.Collections.Generic;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public interface IFileDataServer<T> : IDataServer<T, T>
    {
        event EventHandler<IEnumerable<T>> DataUpdated;
    }
}