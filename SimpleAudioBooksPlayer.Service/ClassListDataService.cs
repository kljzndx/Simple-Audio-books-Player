using System;
using System.Collections.Generic;
using System.Linq;
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

            return _source.ToList();
        }

        public async Task Rename(int index, string newName)
        {
            var item = _source.FirstOrDefault(c => c.Index == index);
            if (item is null)
                return;

            item.Name = newName;
            await _helper.Update(item);

            DataUpdated?.Invoke(this, new[] {item});
        }

        public async Task SetBackgroundColor(int index, string newColor)
        {
            var item = _source.FirstOrDefault(c => c.Index == index);
            if (item is null)
                return;

            item.BackgroundColor = newColor;
            await _helper.Update(item);

            DataUpdated?.Invoke(this, new[] {item});
        }

        public async Task Add(string name)
        {
            var item = new ClassItem(name);

            await _helper.Add(item);
            _source.Add(item);

            DataAdded?.Invoke(this, new[] {item});
        }

        public async Task Remove(int index)
        {
            var item = _source.FirstOrDefault(c => c.Index == index);
            if (item is null)
                return;

            await _helper.Remove(item);
            _source.Remove(item);

            DataRemoved?.Invoke(this, new[] {item});
        }
    }
}