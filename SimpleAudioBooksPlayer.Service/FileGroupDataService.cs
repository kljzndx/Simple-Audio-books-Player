﻿using System;
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

        public Task SetClass(int groupId, int classId) => SetClass(new[] {groupId}, classId);

        public async Task SetClass(IEnumerable<int> groupIdes, int classId)
        {
            var needSet = _source.Where(g => groupIdes.Any(t => t == g.Index)).ToList();
            if (!needSet.Any())
                return;

            foreach (var fileGroup in needSet)
                fileGroup.ClassId = classId;

            await _helper.UpdateRange(needSet);
            DataUpdated?.Invoke(this, needSet);
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

        public async Task SetCover(int groupId, bool hasCover = true)
        {
            var group = _source.FirstOrDefault(src => src.Index == groupId);
            if (@group == null)
                return;

            group.HasCover = hasCover;

            await _helper.Update(group);
            DataUpdated?.Invoke(this, new[] {group});
        }

        public async Task IntelligentAddRange(IEnumerable<string> folderPaths)
        {
            var list = folderPaths.Where(p => _source.All(src => src.FolderPath != p))
                .Select(p => new FileGroup(p)).ToList();

            if (!list.Any())
                return;

            await _helper.AddRange(list);
            _source.AddRange(list);
            DataAdded?.Invoke(this, list);
        }

        public async Task IntelligentRemoveRange(IEnumerable<string> allFolderPaths)
        {
            var list = _source.Where(src => allFolderPaths.All(p => p != src.FolderPath)).ToList();

            if (!list.Any())
                return;

            await _helper.RemoveRange(list);
            _source.RemoveAll(list.Contains);
            DataRemoved?.Invoke(this, list);
        }
    }
}