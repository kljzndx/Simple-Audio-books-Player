using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using SimpleAudioBooksPlayer.ViewModels.SidePages;

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
            SimpleIoc.Default.Register<SettingsViewModel>();
        }

        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
        public MusicListViewModel MusicList => SimpleIoc.Default.GetInstance<MusicListViewModel>();
        public GroupListViewModel GroupList => SimpleIoc.Default.GetInstance<GroupListViewModel>();
        public PlayRecordViewModel PlayRecord => SimpleIoc.Default.GetInstance<PlayRecordViewModel>();
        public SettingsViewModel Settings => SimpleIoc.Default.GetInstance<SettingsViewModel>();
    }
}