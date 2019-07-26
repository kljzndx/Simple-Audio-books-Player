using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.Log;

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
            {
                _source = await _helper.ToList();

                if (_source.FirstOrDefault(c => c.Index == -1) is ClassItem classItem && _source.First().Index != -1)
                {
                    this.LogByObject("检测到 ‘未指定’ 分类不在顶端，正在移动");
                    _source.Remove(classItem);
                    _source.Insert(0, classItem);
                }
            }

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

        public async Task Add(int classId, string name)
        {
            var item = new ClassItem(classId, name);

            await _helper.Add(item);
            if (classId == -1)
                _source.Insert(0, item);
            else
                _source.Add(item);

            DataAdded?.Invoke(this, new[] {item});
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