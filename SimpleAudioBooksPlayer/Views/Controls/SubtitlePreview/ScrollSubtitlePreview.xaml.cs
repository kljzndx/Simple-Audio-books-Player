using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using HappyStudio.Parsing.Subtitle.Interfaces;
using HappyStudio.Subtitle.Control.UWP;
using HappyStudio.Subtitle.Control.UWP.Models;
using SimpleAudioBooksPlayer.Views.ItemTemplates;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace SimpleAudioBooksPlayer.Views.Controls.SubtitlePreview
{
    public sealed partial class ScrollSubtitlePreview : SubtitlePreviewControlBase
    {
        public ScrollSubtitlePreview()
        {
            this.InitializeComponent();
            base.Refreshed += ScrollLyricsPreview_Refreshed;
        }

        public event ItemClickEventHandler ItemClick;
        public event TypedEventHandler<ScrollSubtitlePreview, string> SelectionChanged;

        private double GetItemPosition(SubtitleLineUi line)
        {
            ListViewItem container = Main_ListView.ContainerFromItem(line) as ListViewItem;
            if (container == null)
                return 0;

            var transform = container.TransformToVisual(Main_ListView);

            Point position = transform.TransformPoint(new Point());
            double result = (position.Y) + (container.ActualHeight / 2D) - (Root_ScrollViewer.ActualHeight / 2D);

            return result > 0 ? result : 0;
        }

        private async void ScrollLyricsPreview_Refreshed(object sender, ISubtitleLine args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var theLine = args as SubtitleLineUi;

                foreach (var line in Source.Where(l => l.IsSelected))
                    line.IsSelected = false;

                if (theLine == null)
                {
                    Main_ListView.SelectedIndex = -1;
                    Root_ScrollViewer.ChangeView(null, 0, null);
                    return;
                }

                Main_ListView.SelectedItem = theLine;
                theLine.IsSelected = true;
                Root_ScrollViewer.ChangeView(null, GetItemPosition(theLine), null);
            });
        }

        private void Main_ListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ItemClick?.Invoke(sender, e);
        }

        private void ScrollPreviewItemTemplate_OnSelectionChanged(ScrollPreviewItemTemplate sender, string args)
        {
            SelectionChanged?.Invoke(this, args);
        }
    }
}
