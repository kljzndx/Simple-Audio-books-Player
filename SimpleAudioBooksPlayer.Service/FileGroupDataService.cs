using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.DAL;

namespace SimpleAudioBooksPlayer.Service
{
    public class FileGroupDataService : IObservableDataService<FileGroup>
    {
        public static readonly FileGroupDataService Current = new FileGroupDataService();

        private readonly ContextHelper<FilesContext, FileGroup> _helper;
        private List<FileGroup> _source;

        public event EventHandler<IEnumerable<FileGroup>> DataAdded;
        public event EventHandler<IEnumerable<FileGroup>> DataRemoved;
        public event EventHandler<IEnumerable<FileGroup>> DataUpdated;

        private FileGroupDataService()
        {
            _helper = new ContextHelper<FilesContext, FileGroup>();
        }

        public async Task<List<FileGroup>> GetData()
        {
            if (_source == null)
                _source = await _helper.ToList();

            return _source.ToList();
        }

        public async Task RenameGroup(int groupId, string newName)
        {
            var group = _source.FirstOrDefault(src => src.Index == groupId);
            if (@group == null)
                return;

            group.Name = newName;

            await _helper.Update(group);
            DataUpdated?.Invoke(this, new[] {group});
        }

        public async Task SetCover(int groupId)
        {
            var group = _source.FirstOrDefault(src => src.Index == groupId);
            if (@group == null)
                return;

            group.HasCover = true;

            await _helper.Update(group);
            DataUpdated?.Invoke(this, new[] {group});
        }

        internal async Task AddRange(IEnumerable<string> folderPaths)
        {
            var list = folderPaths.Select(p => new FileGroup(p))
                .Where(g => _source.All(src => src.FolderPath != g.FolderPath)).ToList();

            if (!list.Any())
                return;

            await _helper.AddRange(list);
            _source.AddRange(list);
            DataAdded?.Invoke(this, list);
        }

        internal async Task RemoveRange(IEnumerable<string> folderPaths)
        {
            var list = _source.Where(src => folderPaths.Any(p => src.FolderPath == p)).ToList();

            if (!list.Any())
                return;

            await _helper.RemoveRange(list);
            _source.RemoveAll(list.Contains);
            DataRemoved?.Invoke(this, list);
        }
    }
}