using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.DAL;

namespace SimpleAudioBooksPlayer.Service
{
    public class ClassListDataService : IObservableDataService<ClassItem>
    {
        public static readonly ClassListDataService Current = new ClassListDataService();

        private readonly ContextHelper<FilesContext, ClassItem> _helper = new ContextHelper<FilesContext, ClassItem>();
        private List<ClassItem> _source;

        private ClassListDataService()
        {
        }

        public event EventHandler<IEnumerable<ClassItem>> DataAdded;
        public event EventHandler<IEnumerable<ClassItem>> DataRemoved;
        public event EventHandler<IEnumerable<ClassItem>> DataUpdated;

        public async Task<List<ClassItem>> GetData()
        {
            if (_source is null)
                _source = await _helper.ToList();

            return _source;
        }


    }
}