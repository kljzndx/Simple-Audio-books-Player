using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.DAL;

namespace SimpleAudioBooksPlayer.Service
{
    public class PlaybackRecordDataService : IObservableDataService<PlaybackRecord>
    {
        public static readonly PlaybackRecordDataService Current;

        private readonly ContextHelper<FilesContext, PlaybackRecord> _helper;
        private List<PlaybackRecord> _source;

        static PlaybackRecordDataService()
        {
            Current = new PlaybackRecordDataService();
        }

        private PlaybackRecordDataService()
        {
            _helper = new ContextHelper<FilesContext, PlaybackRecord>();
        }

        public event EventHandler<IEnumerable<PlaybackRecord>> DataAdded;
        public event EventHandler<IEnumerable<PlaybackRecord>> DataRemoved;
        public event EventHandler<IEnumerable<PlaybackRecord>> DataUpdated;

        public async Task<List<PlaybackRecord>> GetData()
        {
            if (_source is null)
                _source = await _helper.ToList();

            return _source.ToList();
        }

        public Task Add(PlaybackRecord source) => AddRange(new[] {source});

        private async Task AddRange(IEnumerable<PlaybackRecord> source)
        {
            var sl = source.ToList();
            if (!sl.Any())
                return;

            await _helper.AddRange(sl);
            _source.AddRange(sl);

            DataAdded?.Invoke(this, sl);
        }

        public Task Remove(int source) => RemoveRange(new[] {source});

        public async Task RemoveRange(IEnumerable<int> source)
        {
            var sl = source.ToList();
            var needRemove = _source.Where(src => sl.Contains(src.GroupId)).ToList();
            if (!needRemove.Any())
                return;

            await _helper.RemoveRange(needRemove);
            _source.RemoveAll(needRemove.Contains);

            DataRemoved?.Invoke(this, needRemove);
        }

        public Task Update(PlaybackRecord source) => UpdateRange(new[] {source});

        private async Task UpdateRange(IEnumerable<PlaybackRecord> source)
        {
            var sl = source.ToList();
            if (!sl.Any())
                return;

            await _helper.UpdateRange(sl);

            _source.RemoveAll(src => sl.Any(p => p.GroupId == src.GroupId));
            _source.AddRange(sl);

            DataUpdated?.Invoke(this, sl);
        }
    }
}