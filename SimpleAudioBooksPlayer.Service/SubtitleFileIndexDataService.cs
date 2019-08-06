using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.DAL;

namespace SimpleAudioBooksPlayer.Service
{
    public class SubtitleFileIndexDataService : IObservableDataService<SubtitleFileIndex>
    {
        public static readonly SubtitleFileIndexDataService Current = new SubtitleFileIndexDataService();

        private readonly ContextHelper<FilesContext, SubtitleFileIndex> _helper = new ContextHelper<FilesContext, SubtitleFileIndex>();
        private List<SubtitleFileIndex> _source;

        private SubtitleFileIndexDataService()
        {
        }

        public event EventHandler<IEnumerable<SubtitleFileIndex>> DataAdded;
        public event EventHandler<IEnumerable<SubtitleFileIndex>> DataRemoved;
        public event EventHandler<IEnumerable<SubtitleFileIndex>> DataUpdated;

        public async Task<List<SubtitleFileIndex>> GetData()
        {
            if (_source is null)
                _source = await _helper.ToList();

            return _source.ToList();
        }

        public async Task AddRange(IEnumerable<SubtitleFileIndex> source)
        {
            var sl = source.ToList();
            if (!sl.Any())
                return;

            await _helper.AddRange(sl);
            _source.AddRange(sl);

            DataAdded?.Invoke(this, sl);
        }

        public async Task RemoveRange(IEnumerable<int> source)
        {
            var sl = source.ToList();
            var needRemove = _source.Where(src => sl.Contains(src.Index)).ToList();
            if (!needRemove.Any())
                return;

            await _helper.RemoveRange(needRemove);
            _source.RemoveAll(needRemove.Contains);

            DataRemoved?.Invoke(this, needRemove);
        }

        public async Task UpdateRange(IEnumerable<SubtitleFileIndex> source)
        {
            var sl = source.ToList();
            if (!sl.Any())
                return;

            await _helper.UpdateRange(sl);

            _source.RemoveAll(src => sl.Any(s => s.Index == src.Index));
            _source.AddRange(sl);

            DataUpdated?.Invoke(this, sl);
        }
    }
}