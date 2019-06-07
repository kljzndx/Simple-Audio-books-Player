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
        }

        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
        public MusicListViewModel MusicList => SimpleIoc.Default.GetInstance<MusicListViewModel>();
    }
}