
using Megatokyo.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Megatokyo.Views
{
    public sealed partial class StripsPage : Page
    {
        public StripsViewModel ViewModel { get; } = new StripsViewModel();

        public StripsPage()
        {
            InitializeComponent();
            Loaded += StripsPage_Loaded;
        }

        private async void StripsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadDataAsync();
        }
    }
}
