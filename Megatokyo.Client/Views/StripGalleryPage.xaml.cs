using Megatokyo.Client.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Megatokyo.Client.Views
{
    public sealed partial class StripGalleryPage : Page
    {
        public StripGalleryViewModel ViewModel { get; } = new StripGalleryViewModel();

        public StripGalleryPage()
        {
            InitializeComponent();
            Loaded += StripGalleryPage_Loaded;
        }

        private async void StripGalleryPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadDataAsync();
        }
    }
}
