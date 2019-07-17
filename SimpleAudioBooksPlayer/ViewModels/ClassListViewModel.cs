using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

namespace SimpleAudioBooksPlayer.ViewModels
{
    public class ClassListViewModel : ViewModelBase
    {
        private readonly ClassListDataServer _server = ClassListDataServer.Current;

        public ClassListViewModel()
        {
            Data = _server.Data;
        }

        public ObservableCollection<ClassItemDTO> Data { get; }

        public Task Add(string name) => _server.Add(name);
    }
}