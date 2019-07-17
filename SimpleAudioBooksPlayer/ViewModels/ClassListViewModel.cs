using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

namespace SimpleAudioBooksPlayer.ViewModels
{
    public class ClassListViewModel : ViewModelBase
    {
        public readonly ClassListDataServer Server = ClassListDataServer.Current;

        public ClassListViewModel()
        {
            Data = Server.Data;
        }

        public ObservableCollection<ClassItemDTO> Data { get; }

        public Task Add(string name) => Server.Add(name);
    }
}