using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace SimpleAudioBooksPlayer.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MusicListViewModel>();
            SimpleIoc.Default.Register<GroupListViewModel>();
            SimpleIoc.Default.Register<PlayRecordViewModel>();
        }

        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
        public MusicListViewModel MusicList => SimpleIoc.Default.GetInstance<MusicListViewModel>();
        public GroupListViewModel GroupList => SimpleIoc.Default.GetInstance<GroupListViewModel>();
        public PlayRecordViewModel PlayRecord => SimpleIoc.Default.GetInstance<PlayRecordViewModel>();
    }
}