using System;
using System.Threading.Tasks;
using Windows.Storage;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileModels;

namespace SimpleAudioBooksPlayer.Models.FileFactories
{
    public class SubtitleFileFactory : IFileFactory<SubtitleFile>
    {
        public Task<SubtitleFile> CreateByFile(StorageFile file, FileGroupDTO group)
        {
            this.LogByObject("创建文件对象");
            return Task.FromResult(new SubtitleFile(group, file.Name));
        }

        public async Task<SubtitleFile> CreateByPath(string path, FileGroupDTO group)
        {
            return await CreateByFile(await StorageFile.GetFileFromPathAsync(path), group);
        }
    }
}