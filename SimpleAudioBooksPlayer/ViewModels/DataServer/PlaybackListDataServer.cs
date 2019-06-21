using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Playback;
using SimpleAudioBooksPlayer.Models;
using SimpleAudioBooksPlayer.Models.DTO;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class PlaybackListDataServer : IDataServer<MusicFileDTO, MusicFileDTO>
    {
        public static PlaybackListDataServer Current = new PlaybackListDataServer();

        private int _tempItemId = -1;
        private int _currentGroupId = -1;
        private readonly List<List<MusicFileDTO>> _tempList = new List<List<MusicFileDTO>>();
        private readonly MediaPlaybackList _playbackList = new MediaPlaybackList();

        private readonly MusicFileDataServer _musicServer = MusicFileDataServer.Current;
        private readonly FileGroupDataServer _groupServer = FileGroupDataServer.Current;
        private readonly PlaybackRecordDataServer _recordServer = PlaybackRecordDataServer.Current;

        private PlaybackListDataServer()
        {
            Data = new ObservableCollection<MusicFileDTO>();
        }

        public bool IsInit { get; private set; }
        public ObservableCollection<MusicFileDTO> Data { get; }

        public event EventHandler<IEnumerable<MusicFileDTO>> DataLoaded;
        public event EventHandler<IEnumerable<MusicFileDTO>> DataAdded;
        public event EventHandler<IEnumerable<MusicFileDTO>> DataRemoved;

        public async Task Init()
        {
            if (IsInit)
                return;

            IsInit = true;

            _musicServer.DataRemoved += MusicServer_DataRemoved;
        }

        public async Task SetSource(MusicFileDTO playTo, MusicListSortArgs sortArgs)
        {
            var groupDto = _groupServer.Data.First(g => g.Index == playTo.GroupId);
            if (groupDto is null)
                return;

            var list = _musicServer.Data.Where(m => m.GroupId == playTo.GroupId).ToList();
            if (_currentGroupId != playTo.GroupId)
            {
                SplitList(list);
                Data.Clear();
                foreach (var musicFile in list)
                    Data.Add(musicFile);
            }

            uint trackId = (uint) Data.IndexOf(playTo);
            await _recordServer.SetRecord(new PlaybackRecordDTO(playTo.Title, groupDto, trackId, sortArgs.SortMethod, false));
            await PlayTo(trackId);
        }

        private void SplitList(IEnumerable<MusicFileDTO> files)
        {
            _tempList.Clear();
            var queue = new Queue<MusicFileDTO>(files);

            while (queue.Any())
            {
                var temp = new List<MusicFileDTO>();

                for (int i = 0; i < 10; i++)
                    temp.Add(queue.Dequeue());

                _tempList.Add(temp);
            }
        }

        private async Task PlayTo(uint trackId)
        {
            var groupId = (int) (((trackId + 1) / 10) - 1);
            var group = _tempList[groupId];
            var mfId = (uint) (trackId < 10 ? trackId : trackId - (10 * (groupId + 1)));

            if (_tempItemId != groupId)
            {
                _playbackList.Items.Clear();
                foreach (var fileDto in group)
                    _playbackList.Items.Add(await fileDto.GetPlaybackItem());
                _tempItemId = groupId;
            }

            _playbackList.MoveTo(mfId);
        }
        
        private async void MusicServer_DataRemoved(object sender, IEnumerable<MusicFileDTO> e)
        {
            foreach (var fileDto in Data.Where(src => e.Any(f => f.FilePath == src.FilePath)))
            {
                var pb = await fileDto.GetPlaybackItem();
                if (_playbackList.Items.Contains(pb))
                    _playbackList.Items.Remove(pb);

                Data.Remove(fileDto);
            }
        }
    }
}