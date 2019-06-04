using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleAudioBooksPlayer.Service
{
    public interface IDataService<TData> where TData : class
    {
        event EventHandler<IEnumerable<TData>> DataAdded;
        event EventHandler<IEnumerable<TData>> DataRemoved;

        Task<List<TData>> GetData();
    }
}